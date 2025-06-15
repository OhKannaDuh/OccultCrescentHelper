using Dalamud.Interface;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Ocelot;

namespace OccultCrescentHelper.Modules.Buff;

public class Panel
{
    public void Draw(BuffModule module)
    {
        OcelotUI.Title("Buff:");
        OcelotUI.Indent(() => {
            if (ImGui.BeginTable("BuffData##OCH", 2, ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.Redo, "SwitchAndBuff",
                                       enabled: module.tracker.IsNearCrystal()))
                {
                    module.tracker.SwitchJobAndBuff();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Refresh all buffs and switch back to your job.");
                }
            
                ImGui.EndTable();
            }
        });
    }
}
