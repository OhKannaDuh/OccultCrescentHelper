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
            if (ImGui.BeginTable("BuffData##OCH", 4, ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.Redo, "GetNearbyCrystal",
                                       enabled: module.tracker.IsNearCrystal()))
                {
                    module.tracker.ResetBuff();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowCircleUp, "OpenSupportJob"))
                {
                    module.tracker.OpenSupportJob();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Open support Job");
                }

                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowCircleUp, "OpenSupportList"))
                {
                    module.tracker.OpenSupportJobList();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Open support List");
                }
                
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowCircleUp, "SwitchToBardo"))
                {
                    module.tracker.SwitchToBard();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Switch to Bard");
                }
                
                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowCircleUp, "SwitchToBardoReal"))
                {
                    module.tracker.SwitchToClassId(6);
                }
                
                ImGui.TableNextColumn();
                if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowCircleUp, "SwitchToPaladine"))
                {
                    module.tracker.SwitchToClassId(1);
                }

                ImGui.EndTable();
            }
        });
    }
}
