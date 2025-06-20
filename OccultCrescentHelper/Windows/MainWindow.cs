﻿using Dalamud.Interface;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Modules.Automator;
using Ocelot.IPC;
using Ocelot.Windows;

namespace OccultCrescentHelper.Windows;

[OcelotMainWindow]
public class MainWindow : OcelotMainWindow
{
    public MainWindow(Plugin plugin, Config config)
        : base(plugin, config) { }

    public override void PostInitialize()
    {
        base.PostInitialize();

        TitleBarButtons.Add(new() {
            Click = (m) => {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                if (plugin.modules.TryGetModule<AutomatorModule>(out var automator) && automator != null)
                {
                    automator.config.Enabled = false;
                }

                plugin.ipc.GetProvider<VNavmesh>()?.Stop();
                Plugin.Chain.Abort();


            },
            Icon = FontAwesomeIcon.Stop,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Emergency Stop"),
        });

        TitleBarButtons.Add(new() {
            Click = (m) => {
                if (m != ImGuiMouseButton.Left)
                {
                    return;
                }

                var automatorModule = plugin.modules.GetModule<AutomatorModule>();
                if (!automatorModule.config.Enabled)
                {
                    automatorModule.config.Enabled = true;
                }
                else
                {
                    automatorModule.config.Enabled = false;
                    plugin.ipc.GetProvider<VNavmesh>()?.Stop();
                    Plugin.Chain.Abort();
                }
            },
            Icon = FontAwesomeIcon.Skull,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Toggle Illegal Mode"),
        });
    }

    public override void Draw()
    {
        if (!ZoneData.IsInOccultCrescent())
        {
            ImGui.TextUnformatted("Not in Occult Crescent zone.");
            return;
        }

        plugin.modules?.DrawMainUi();
    }
}
