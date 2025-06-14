using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ECommons;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Common.Math;
using OccultCrescentHelper.Enums;

namespace OccultCrescentHelper.Modules.Buff;

public class BuffTracker
{
    public BuffTracker()
    {
        Reset();
    }

    public void Tick(IFramework _)
    {
        //     getNearbyCrystal();
    }

    public void ResetBuff()
    {
        SwitchToBard();
        SwitchToMonk();
        SwitchToPaladin();
    }

    public void Reset() { }
    
    public bool IsNearCrystal()
    {
        return NearbyCrystalCount() > 0;
    }

    public int NearbyCrystalCount()
    {
        var playerPos = Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero;

        return Svc.Objects
                  .Where(o => o.ObjectKind == ObjectKind.EventObj)
                  .Where(o => o.DataId == 2007457)
                  .Count(o => Vector3.Distance(o.Position, playerPos) <= 4.5f);
    }

    public void OpenSupportJob()
    {
        Svc.Log.Debug("Trying to open the support job GUI.");
        unsafe
        {
            var baseSupportGui = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDInfo");
            if (!IsSupportGuiVisible())
            {
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(baseSupportGui, true, 1, 0);
                });
                task.Wait();
                OpenSupportJob();
                Svc.Log.Debug("Support job callback sent");
            }
        }
    }

    private unsafe bool IsSupportGuiVisible()
    {
        var supportJobAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
        var isSupportGuiVisible = supportJobAddon != null && supportJobAddon->IsVisible;
        if (isSupportGuiVisible)
        {
            Svc.Log.Debug("Support job visible.");
        }
        else
        {
            Svc.Log.Debug("Support job not visible.");
        }

        return isSupportGuiVisible;
    }

    public void OpenSupportJobList()
    {
        Svc.Log.Debug("Trying to open support job list.");
        if (!IsSupportJobListVisible() && !IsSupportGuiVisible())
        {
            OpenSupportJob();
        }

        unsafe
        {
            var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJob");
            if (!IsSupportJobListVisible())
            {
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(supportJobListAddon, true, 0, 0, 0);
                    Svc.Log.Debug("Job List callback sent");
                });
                task.WaitSafely();
                OpenSupportJobList();
            }
            else
            {
                Svc.Log.Debug("Support job list already visible");
            }
        }
    }

    private unsafe bool IsSupportJobListVisible()
    {
        var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJobList");
        var isSupportJobListVisible = supportJobListAddon != null && supportJobListAddon->IsVisible;
        if (isSupportJobListVisible)
        {
            Svc.Log.Debug("Support job list visible.");
        }
        else
        {
            Svc.Log.Debug("Support job list not visible.");
        }

        return isSupportJobListVisible;
    }

    public void SwitchToBard()
    {
        if (IsBard())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(6);
            SwitchToBard();
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToBard();
            }
            else
            {
                OpenSupportJob();
                SwitchToBard();
            }
        }
    }

    public void SwitchToMonk()
    {
        if (IsMonk())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(3);
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToMonk();
            }
            else
            {
                OpenSupportJob();
                SwitchToMonk();
            }
        }
    }

    public void SwitchToPaladin()
    {
        if (IsPaladin())
        {
            return;
        }

        if (IsSupportJobListVisible())
        {
            SwitchToClassId(1);
        }
        else
        {
            if (IsSupportGuiVisible())
            {
                OpenSupportJobList();
                SwitchToPaladin();
            }
            else
            {
                OpenSupportJob();
                SwitchToPaladin();
            }
        }
    }

    public void SwitchToClassId(uint classId)
    {
        Svc.Log.Debug($"Switching to class {classId}");
        unsafe
        {
            if (IsSupportJobListVisible())
            {
                var supportJobListAddon = (AtkUnitBase*)Svc.GameGui.GetAddonByName("MKDSupportJobList");
                var task = Svc.Framework.RunOnFrameworkThread(() => {
                    Callback.Fire(supportJobListAddon, true, 0, classId);
                });
                task.WaitSafely();
            }
        }
    }

    public bool IsBard()
    {
        return HasBuff((uint)JobStatus.Bard);
    }

    public bool IsMonk()
    {
        return HasBuff((uint)JobStatus.Monk);
    }

    public bool IsPaladin()
    {
        return HasBuff((uint)JobStatus.Paladin);
    }

    public bool HasBardBuff()
    {
        return HasBuff((uint)JobBuffStatus.Bard);
    }

    public bool HasMonkBuff()
    {
        return HasBuff((uint)JobBuffStatus.Monk);
    }

    public bool HadPaladinBuff()
    {
        return HasBuff((uint)JobBuffStatus.Paladin);
    }

    private bool HasBuff(uint buffId)
    {
        var currentStatusList = Svc.ClientState.LocalPlayer!.StatusList;
        return currentStatusList.Any(status => status.StatusId == buffId);
    }
}
