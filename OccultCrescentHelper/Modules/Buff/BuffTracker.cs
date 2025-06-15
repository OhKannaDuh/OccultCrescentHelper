using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Common.Math;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Enums;
using Ocelot.Chain;
using Ocelot.Chain.ChainEx;

namespace OccultCrescentHelper.Modules.Buff;

public class BuffTracker
{
    private byte startingJobId = 0;

    public bool IsNearCrystal()
    {
        return NearbyCrystalCount() > 0;
    }

    public int NearbyCrystalCount()
    {
        var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;
        var crystalDataId = 2007457;

        return Svc.Objects
                  .Where(o => o.ObjectKind == ObjectKind.EventObj)
                  .Where(o => o.DataId == crystalDataId)
                  .Count(o => Vector3.Distance(o.Position, playerPos) <= 4.5f);
    }


    private Chain BardChain()
    {
        return Chain.Create("Buffs:Bard")
                    .RunIf(() => !HastatusWithRemainingTimeGreaterThan(PlayerStatus.RomeosBallad))
                    .Debug($"[BUFF] Starting Bard chain")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Bard))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomBard)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 32)
                    .CustomWaitUntilStatus((uint)PlayerStatus.RomeosBallad)
                    .WaitGcd();
    }

    private Chain MonkChain()
    {
        return Chain.Create("Buffs:Monk")
                    .RunIf(() => !HastatusWithRemainingTimeGreaterThan(PlayerStatus.Fleetfooted))
                    .Debug($"[BUFF] Starting Monk chain")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Monk))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomMonk)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 33)
                    .CustomWaitUntilStatus((uint)PlayerStatus.Fleetfooted)
                    .WaitGcd();
    }

    private Chain KnightChain()
    {
        return Chain.Create("Buffs:Knight")
                    .RunIf(() => !HastatusWithRemainingTimeGreaterThan(PlayerStatus.EnduringFortitude))
                    .Debug($"[BUFF] Starting Knight chain")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob((byte)JobId.Knight))
                    .WaitUntilStatus((uint)PlayerStatus.PhantomKnight)
                    .WaitGcd()
                    .UseAction(ActionType.GeneralAction, 32)
                    .CustomWaitUntilStatus((uint)PlayerStatus.EnduringFortitude)
                    .WaitGcd();
    }

    private Chain BackToStartingJob()
    {
        return Chain.Create("Buffs:StartingJob")
                    .Then(_ => PublicContentOccultCrescent.ChangeSupportJob(startingJobId))
                    .WaitGcd();
    }

    public void SwitchJobAndBuff()
    {
        startingJobId = StartingJobId();
        Svc.Log.Debug($"[BUFF] Switching from Job Id{startingJobId}");

        var manager = ChainManager.Get("OCH##BuffManager");
        if (manager.IsRunning)
        {
            return;
        }


        Chain Factory()
        {
            return BardChain().Then(MonkChain).Then(KnightChain).Then(BackToStartingJob);
        }

        manager.Submit(Factory);
    }

    private unsafe byte StartingJobId()
    {
        return PublicContentOccultCrescent.GetState()->CurrentSupportJob;
    }
    
    // Check if the status is already here AND with at least 29:50 remaining time by default.
    private bool HastatusWithRemainingTimeGreaterThan(PlayerStatus playerStatus, int remainingTime = 1790)
    {
        try
        {
            return Svc.ClientState.LocalPlayer?.StatusList.First(s => s.StatusId == (uint)playerStatus)
                      .RemainingTime > remainingTime;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
