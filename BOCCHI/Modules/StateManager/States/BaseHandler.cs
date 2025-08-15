using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using Ocelot.States;

namespace BOCCHI.Modules.StateManager.States;

public abstract class BaseHandler(StateManagerModule module) : StateHandler<State, StateManagerModule>(module)
{
    protected bool IsInCombat()
    {
        return Svc.Condition[ConditionFlag.InCombat];
    }

    protected unsafe bool IsInFate()
    {
        return FateManager.Instance()->CurrentFate is not null;
    }

    protected unsafe bool IsInCriticalEncounter()
    {
        var dec = DynamicEventContainer.GetInstance();
        return dec != null && dec->CurrentEventId != 0;
    }
}
