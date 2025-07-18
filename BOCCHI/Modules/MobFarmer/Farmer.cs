﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using BOCCHI.ActionHelpers;
using BOCCHI.Modules.ForkedTower;
using BOCCHI.Modules.MobFarmer.Chains;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Reflection;
using ECommons.Throttlers;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;
using Ocelot.IPC;
using Ocelot.Modules;
using Ocelot.Windows;
using Chain = Ocelot.Chain.Chain;

namespace BOCCHI.Modules.MobFarmer;

public class Farmer : IDisposable
{
    private MobFarmerModule module;

    private ChainQueue ChainQueue
    {
        get => ChainManager.Get("MobFarmer+Farmer");
    }

    public bool Running { get; private set; } = false;


    private bool HasRunBuff = false;

    private bool HasRunStack = false;

    private Vector3 StartingPoint = Vector3.Zero;

    public FarmerPhase Phase { get; private set; } = FarmerPhase.Waiting;

    private IRotationPlugin RotationPlugin;

    private Dictionary<string, Func<IModule, IRotationPlugin>> rotationPlugins = new()
    {
        { "WrathCombo", m => new Wrath(m) },
    };

    public IEnumerable<IBattleNpc> Mobs
    {
        get => module.scanner.Mobs;
    }

    public IEnumerable<IBattleNpc> InCombat
    {
        get => Mobs.Where(o => o.IsTargetingPlayer());
    }

    public IEnumerable<IBattleNpc> NotInCombat
    {
        get => Mobs.Where(o => !o.HasTarget());
    }

    private Dictionary<FarmerPhase, Func<FarmerPhase?>> Handlers;

    public Farmer(MobFarmerModule module)
    {
        this.module = module;

        Handlers = new Dictionary<FarmerPhase, Func<FarmerPhase?>>
        {
            { FarmerPhase.Waiting, HandleWaitingPhase },
            { FarmerPhase.Buffing, HandleBuffingPhase },
            { FarmerPhase.Gathering, HandleGatheringPhase },
            { FarmerPhase.Stacking, HandleStackingPhase },
            { FarmerPhase.Fighting, HandleFightingPhase },
        };


        RotationPlugin = new BlankRotationPlugin();
        foreach (var (plugin, factory) in rotationPlugins)
        {
            if (!DalamudReflector.TryGetDalamudPlugin(plugin, out _, false, true))
            {
                continue;
            }

            RotationPlugin = factory(module);
            break;
        }
    }

    public void Tick()
    {
        if (!Running || !Mobs.Any())
        {
            return;
        }

        if (!Handlers.TryGetValue(Phase, out var handler))
        {
            return;
        }

        var transition = handler();
        if (transition != null)
        {
            Phase = transition.Value;
        }
    }

    private FarmerPhase? HandleWaitingPhase()
    {
        if (Svc.Condition[ConditionFlag.InCombat])
        {
            return FarmerPhase.Fighting;
        }

        return Mobs.Count() >= module.Config.MinimumMobsToStartLoop ? FarmerPhase.Buffing : null;
    }

    private FarmerPhase? HandleBuffingPhase()
    {
        if (!module.Config.ApplyBattleBell)
        {
            return FarmerPhase.Gathering;
        }

        if (Plugin.Chain.IsRunning)
        {
            return null;
        }

        if (HasRunBuff)
        {
            HasRunBuff = false;
            Plugin.Chain.Submit(Actions.Sprint.GetCastChain());
            return FarmerPhase.Gathering;
        }

        Plugin.Chain.Submit(new BattleBellChain(module));
        HasRunBuff = true;

        return null;
    }

    private FarmerPhase? HandleGatheringPhase()
    {
        var vnav = module.GetIPCProvider<VNavmesh>();

        if (InCombat.Count() >= module.Config.MinimumMobsToStartFight || !NotInCombat.Any())
        {
            vnav.Stop();
            ChainQueue.Abort();
            return FarmerPhase.Stacking;
        }

        if (Svc.Targets.Target?.IsTargetingPlayer() == true)
        {
            Svc.Targets.Target = null;
            ChainQueue.Abort();
        }

        Svc.Targets.Target = NotInCombat.First();

        if (!ChainQueue.IsRunning && Svc.Targets.Target != null)
        {
            var target = Svc.Targets.Target;

            if (target.IsTargetingPlayer() || EzThrottler.Throttle("Repath", 500))
            {
                Task<List<Vector3>>? task = null;
                List<Vector3> path = [];
                ChainQueue.Submit(() =>
                    Chain.Create()
                        .Then(_ => task = vnav.Pathfind(Player.Position, target.Position, false))
                        .Then(_ => task!.IsCompleted)
                        .Then(_ => path = task!.Result)
                        .BreakIf(() => path.Count <= 1)
                        .Then(_ => path.RemoveAt(0))
                        .Then(_ => vnav.MoveToPath(path, false))
                );
            }
        }

        return null;
    }

    private FarmerPhase? HandleStackingPhase()
    {
        var vnav = module.GetIPCProvider<VNavmesh>();

        if (HasRunStack && !vnav.IsRunning())
        {
            HasRunStack = false;
            // Chat.ExecuteCommand("/wrath set 110058");
            RotationPlugin.PhantomJobOn();
            return FarmerPhase.Fighting;
        }

        var furthest = InCombat.Where(o => o.Address != Svc.Targets.Target?.Address).OrderBy(Player.DistanceTo).LastOrDefault();
        if (furthest == null)
        {
            return FarmerPhase.Fighting;
        }

        vnav.PathfindAndMoveTo(furthest.Position, false);
        HasRunStack = true;

        return null;
    }

    private FarmerPhase? HandleFightingPhase()
    {
        var anyInCombat = InCombat.Any();
        if (anyInCombat && EzThrottler.Throttle("Targetter"))
        {
            Svc.Targets.Target = InCombat.Centroid();
        }

        var shouldReturnHome = module.Config.ReturnToStartInWaitingPhase && Player.DistanceTo(StartingPoint) >= module.Config.MinEuclideanDistanceToReturnHome;
        if (shouldReturnHome && !anyInCombat)
        {
            var vnav = module.GetIPCProvider<VNavmesh>();
            if (!vnav.IsRunning())
            {
                vnav.PathfindAndMoveTo(StartingPoint, false);
            }

            return Player.DistanceTo(StartingPoint) <= 2f ? FarmerPhase.Waiting : null;
        }

        if (!anyInCombat && !Svc.Condition[ConditionFlag.InCombat])
        {
            // Svc.Commands.ProcessCommand("/wrath unset 110058");
            RotationPlugin.PhantomJobOff();
            return FarmerPhase.Waiting;
        }

        return null;
    }

    public void Draw(RenderContext context)
    {
        if (!Mobs.Any())
        {
            return;
        }

        if (!Running && !module.Config.ShouldRenderDebugLinesWhileNotRunning)
        {
            return;
        }

        if (!module.Config.RenderDebugLines)
        {
            return;
        }

        foreach (var mob in NotInCombat)
        {
            var color = new Vector4(0.9f, 0.1f, 0.1f, 1f);
            if (mob.NameId != (uint)module.Config.Mob)
            {
                color = new Vector4(0.9f, 0.1f, 0.9f, 1f);
            }

            context.DrawLine(mob.Position, color);
        }

        foreach (var mob in InCombat)
        {
            context.DrawLine(mob.Position, new Vector4(0.1f, 0.9f, 0.1f, 1f));
        }
    }

    public void Toggle()
    {
        Running = !Running;
        Phase = FarmerPhase.Waiting;
        if (Running)
        {
            StartingPoint = Player.Position;
            // Svc.Commands.ProcessCommand("/wrath unset 110058");
            RotationPlugin.PhantomJobOff();
        }
    }

    public void Dispose()
    {
        RotationPlugin.Dispose();
    }
}
