using System;
using ECommons.DalamudServices;
using OccultCrescentHelper.Modules.EventDrop;
using OccultCrescentHelper.Modules.Teleporter;
using OccultCrescentHelper.Modules.Treasure;
using OccultCrescentHelper.Modules.Carrots;
using OccultCrescentHelper.Modules.Currency;
using OccultCrescentHelper.Modules.CriticalEncounters;
using OccultCrescentHelper.Modules.Fates;
using Ocelot;
using OccultCrescentHelper.Modules.Exp;
using OccultCrescentHelper.Modules.WindowManager;
using OccultCrescentHelper.Modules.StateManager;
using OccultCrescentHelper.Modules.InstanceIdentifier;
using OccultCrescentHelper.Modules.Automator;
using OccultCrescentHelper.Modules.Buff;
using OccultCrescentHelper.Modules.Mount;

namespace OccultCrescentHelper;

[Serializable]
public class Config : IOcelotConfig
{
    public int Version { get; set; } = 1;

    public TreasureConfig TreasureConfig { get; set; } = new();

    public CarrotsConfig CarrotsConfig { get; set; } = new();

    public CurrencyConfig CurrencyConfig { get; set; } = new();
    
    public BuffConfig BuffConfig { get; set; } = new();

    public EventDropConfig EventDropConfig { get; set; } = new();

    public FatesConfig FatesConfig { get; set; } = new();

    public CriticalEncountersConfig CriticalEncountersConfig { get; set; } = new();

    public ExpConfig ExpConfig { get; set; } = new();

    public TeleporterConfig TeleporterConfig { get; set; } = new();

    public WindowManagerConfig WindowManagerConfig { get; set; } = new();

    public StateManagerConfig StateManagerConfig { get; set; } = new();

    public InstanceIdentifierConfig InstanceIdentifierConfig { get; set; } = new();

    public AutomatorConfig AutomatorConfig { get; set; } = new();

    public MountConfig MountConfig { get; set; } = new();

    public void Save() => Svc.PluginInterface.SavePluginConfig(this);
}
