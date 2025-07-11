using System.Numerics;
using BOCCHI.Data;
using Dalamud.Interface;
using ImGuiNET;
using Ocelot;
using Ocelot.Windows;

namespace BOCCHI.Modules.Automator;

[OcelotWindow]
public class AutomatorWindow(Plugin _plugin, Config _config) : OcelotWindow(_plugin, _config)
{
    public override void PostInitialize()
    {
        base.PostInitialize();

        TitleBarButtons.Add(new TitleBarButton
        {
            Click = (m) =>
            {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                AutomatorModule.ToggleIllegalMode(plugin);
            },
            Icon = FontAwesomeIcon.Skull,
            IconOffset = new Vector2(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Toggle Illegal Mode"),
        });
    }

    public override void Draw()
    {
        if (!ZoneData.IsInOccultCrescent())
        {
            ImGui.TextUnformatted(I18N.T("generic.label.not_in_zone"));
            return;
        }

        var automator = plugin.modules.GetModule<AutomatorModule>();
        if (!automator.enabled)
        {
            ImGui.TextUnformatted("Illegal Mode is not enabled.");
            return;
        }

        automator.panel.Draw(automator);
    }

    protected override string GetWindowName()
    {
        return plugin.modules.GetModule<AutomatorModule>().T("panel.lens.title");
    }
}
