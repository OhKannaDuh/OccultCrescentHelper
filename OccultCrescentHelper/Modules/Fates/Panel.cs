using System.Linq;
using ImGuiNET;
using OccultCrescentHelper.Data;
using OccultCrescentHelper.Modules.Teleporter;
using Ocelot;

namespace OccultCrescentHelper.Modules.Fates;

public class Panel
{
    public void Draw(FatesModule module)
    {
        OcelotUI.Title("Fates:");
        OcelotUI.Indent(() => {
            if (module.tracker.fates.Count <= 0)
            {
                ImGui.TextUnformatted("No active fates.");
                return;
            }

            foreach (var fate in module.fates.Values)
            {
                if (fate == null)
                {
                    continue;
                }

                if (!EventData.Fates.TryGetValue(fate.FateId, out var data))
                {
                    continue;
                }

                ImGui.TextUnformatted($"{data.Name} ({fate.Progress}%)");


                if (module.progress.TryGetValue(fate.FateId, out var progress))
                {
                    var estimate = progress.EstimateTimeToCompletion();
                    if (estimate != null)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted($"(Est. {estimate.Value:mm\\:ss})");
                    }
                }

                if (module.TryGetModule<TeleporterModule>(out var teleporter) && teleporter!.IsReady())
                {
                    teleporter.teleporter.Button(data.aethernet, fate.Position, data.Name, $"fate_{fate.FateId}", data);
                }

                OcelotUI.Indent(() => EventIconRenderer.Drops(data, module.plugin.config.EventDropConfig));

                if (!fate.Equals(module.fates.Values.Last()))
                {
                    OcelotUI.VSpace();
                }
            }
        });
    }
}
