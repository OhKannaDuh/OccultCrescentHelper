﻿using BOCCHI.Data;
using ImGuiNET;
using Ocelot;
using Ocelot.Windows;

namespace BOCCHI.Modules.Debug;

#if DEBUG_BUILD
[OcelotWindow]
#endif
public class DebugWindow(Plugin priamryPlugin, Config config) : OcelotWindow(priamryPlugin, config)
{
    public override void Draw()
    {
        if (!ZoneData.IsInOccultCrescent())
        {
            ImGui.TextUnformatted(I18N.T("generic.label.not_in_zone"));
            return;
        }

        if (plugin.modules.TryGetModule<DebugModule>(out var module) && module != null)
        {
            module.DrawPanels();
        }
    }

    protected override string GetWindowName()
    {
        return "OCH Debug";
    }
}
