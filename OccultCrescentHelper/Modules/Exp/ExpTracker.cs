using System;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using ClientLanguage = Dalamud.Game.ClientLanguage;

namespace OccultCrescentHelper.Modules.Exp;

public class ExpTracker
{
    private float exp = 0f;
    private readonly string pattern;

    private DateTime startTime = DateTime.UtcNow;

    public ExpTracker()
    {
        Reset();
        pattern = getExpMessagePattern(Svc.ClientState.ClientLanguage);
    }

    public void OnTerritoryChange(ushort _) => Reset();

    public void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
    {
        var match = Regex.Match(message.ToString(), pattern);
        if (match.Success)
        {
            exp += int.Parse(match.Groups[1].Value);
        }
    }

    public void Reset()
    {
        exp = 0f;
        startTime = DateTime.UtcNow;
    }

    public float GetExpPerHour()
    {
        var elapsed = (float)(DateTime.UtcNow - startTime).TotalHours;
        if (elapsed <= 0)
            return 0;

        return (exp) / elapsed;
    }

    private string getExpMessagePattern(ClientLanguage clientLanguage)
    {
        return clientLanguage switch {
            ClientLanguage.English => @"You gain (\d+) Phantom .+? experience points\.",
            ClientLanguage.French =>  @"Vous gagnez (\d+) points d'expérience de soutien en .+? fantôme",
            ClientLanguage.German =>  @"Du erhältst (\d+) Phantomroutine als Phantom",
            ClientLanguage.Japanese =>  @".+?」に(\d+)ポイントのサポート経験値を得た。",
        };
    }
}
