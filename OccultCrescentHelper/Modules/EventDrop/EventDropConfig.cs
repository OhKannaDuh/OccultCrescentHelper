using System;
using Ocelot.Config.Attributes;

namespace OccultCrescentHelper.Modules.EventDrop;

[Serializable]
[Title("Event Drop Config")]
public class EventDropConfig : Ocelot.Modules.ModuleConfig
{
    [Checkbox]
    [Label("Show Demiatma drops")]
    [Tooltip("Show Demiatma drops in the active fate/ce list.")]
    public bool ShowDemiatmaDrops { get; set; } = true;

    [Checkbox]
    [Label("Show Notes drops")]
    [Tooltip("Show Notes drops in the active fate/ce list.")]
    public bool ShowNoteDrops { get; set; } = true;

    [Checkbox]
    [Label("Show Soul Shard drops")]
    [Tooltip("Show Soul Shard drops in the active ce list.")]
    public bool ShowSoulShardDrops { get; set; } = true;
}
