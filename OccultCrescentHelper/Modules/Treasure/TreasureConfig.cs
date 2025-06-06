using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Treasure;

[Title("Treasure Config")]
public class TreasureConfig : ModuleConfig
{
    [Checkbox]
    [Label("Enabled")]
    public bool Enabled { get; set; } = true;

    [Checkbox]
    [RenderIf(nameof(Enabled))]
    [Label("Draw lines to bronze coffers")]
    [Tooltip("Render a line from your characters position to nearby bronze coffers.")]
    public bool DrawLineToBronzeChests { get; set; } = true;

    [Checkbox]
    [RenderIf(nameof(Enabled))]
    [Label("Draw lines to silver coffers")]
    [Tooltip("Render a line from your characters position to nearby silver coffers.")]
    public bool DrawLineToSilverChests { get; set; } = true;
}
