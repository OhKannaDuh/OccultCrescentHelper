using System;
using System.Linq;
using BOCCHI.Data;
using BOCCHI.Modules.Teleporter;
using ImGuiNET;
using Ocelot;

namespace BOCCHI.Modules.Fates;

public class Panel
{
    public void Draw(FatesModule module)
    {
        OcelotUI.Title($"{module.T("panel.title")}:");
        OcelotUI.Indent(() =>
        {
            if (module.tracker.Fates.Count <= 0)
            {
                ImGui.TextUnformatted(module.T("panel.none"));
                return;
            }

            foreach (var fate in module.fates.Values)
            {
                if (!ZoneData.IsInOccultCrescent())
                {
                    module.fates.Clear();
                    return;
                }

                if (!EventData.Fates.TryGetValue(fate.FateId, out var data))
                {
                    continue;
                }

                try
                {
                    ImGui.TextUnformatted($"{fate.Name} ({fate.Progress}%)");
                }
                catch (AccessViolationException)
                {
                    continue;
                }


                if (module.progress.TryGetValue(fate.FateId, out var progress))
                {
                    var estimate = progress.EstimateTimeToCompletion();
                    if (estimate != null)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"({module.T("panel.estimated")} {estimate.Value:mm\\:ss})");
                    }
                }

                if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
                {
                    teleporter.teleporter.Button(data.aethernet, data.start ?? fate.Position, data.Name, $"fate_{fate.FateId}", data);
                }

                OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.PluginConfig.EventDropConfig));

                if (!fate.Equals(module.fates.Values.Last()))
                {
                    OcelotUI.VSpace();
                }
            }
        });
    }
}
