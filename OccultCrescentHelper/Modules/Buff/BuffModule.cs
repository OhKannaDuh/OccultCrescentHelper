
using Dalamud.Plugin.Services;
using Ocelot.Modules;

namespace OccultCrescentHelper.Modules.Buff;

[OcelotModule(5, 2)]
public class BuffModule : Module<Plugin, Config>
{
    public override BuffConfig config {
        get => _config.BuffConfig;
    }

    public override bool enabled => config.IsPropertyEnabled(nameof(config.Enabled));

    public readonly BuffTracker tracker = new();

    private Panel panel = new();

    public BuffModule(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override bool DrawMainUi()
    {
        panel.Draw(this);
        return true;
    }
}
