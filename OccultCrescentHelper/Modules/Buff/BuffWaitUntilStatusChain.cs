using System;
using System.Linq;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using Ocelot.Chain;

namespace OccultCrescentHelper.Modules.Buff;

public static class BuffWaitUntilStatusChain
{
    private static TaskManagerTask WaitUntilStatusWithTimeRemaining(
        uint statusId, int timeRemaining = 1790, int timeout = 5000, int interval = 50)
    {
        return new(() => {
            if (EzThrottler.Throttle($"ChainStatus.WaitUntilStatus({statusId})", interval))
            {
                try
                {
                    Svc.Log.Debug($"[BUFF] Waiting for status {statusId} with time remaining {timeRemaining}");
                    var foundStatus = Svc.ClientState.LocalPlayer?.StatusList.First(s => s.StatusId == statusId);
                    return foundStatus != null && foundStatus.RemainingTime > timeRemaining;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }, new() { TimeLimitMS = timeout });
    }

    public static Chain CustomWaitUntilStatus(
        this Chain chain, uint statusId, int timeRemaining = 1790, int timeout = 5000, int interval = 50)
    {
        return chain.Then(WaitUntilStatusWithTimeRemaining(statusId, timeRemaining, timeout, interval));
    }
}
