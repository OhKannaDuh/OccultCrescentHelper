using System.Collections.Generic;
using System.Linq;
using BOCCHI.Modules.Buff;
using BOCCHI.Modules.Debug;
using ECommons.DalamudServices;
using Ocelot;
using Ocelot.Commands;
using Ocelot.Modules;

namespace BOCCHI.Commands;

[OcelotCommand]
public class MainCommand(Plugin plugin) : OcelotCommand
{
    public override string command
    {
        get => "/bocchi";
    }

    public override string description
    {
        get => @"
Opens Occult Crescent Helper main ui
 - /bocchi : Opens the main ui
 - /bocchi config : opens the config ui
 - /bocchi cfg : opens the config ui
--------------------------------
".Trim();
    }

    public override IReadOnlyList<string> aliases
    {
        get => ["/och", "/occultcrescenthelper"];
    }

    private readonly IReadOnlyList<string> languageCodes =
    [
        "en", "de", "fr", "jp", "uwu",
    ];

    public override void Command(string command, string arguments)
    {
        if (arguments == "config" || arguments == "cfg")
        {
            plugin.windows?.ToggleConfigUI();
            return;
        }

#if DEBUG_BUILD
        if (arguments == "debug")
        {
            var debug = plugin.modules.GetModule<DebugModule>();
            var window = plugin.windows.GetWindow<DebugWindow>();
            if (debug != null && window != null)
            {
                window.Toggle();
                return;
            }
        }
#endif

        if (arguments == "buff")
        {
            var buffs = plugin.modules.GetModule<BuffModule>();
            if (buffs != null)
            {
                buffs.buffs.QueueBuffs();
                return;
            }
        }

        if (arguments == "tp")
        {
            Svc.Chat.Print("Press the button nerd.");
            return;
        }

        if (arguments.StartsWith("language"))
        {
            var parts = arguments.Split(' ', 2);
            if (parts.Length == 2)
            {
                var code = parts[1].Trim().ToLowerInvariant();
                if (languageCodes.Contains(code))
                {
                    I18N.SetLanguage(code);
                    Svc.Chat.Print($"Language set to: {code}");
                    return;
                }
                else
                {
                    Svc.Log.Error($"Unknown language code: {code}");
                    return;
                }
            }
            else
            {
                Svc.Chat.Print("Usage: /bocchi language <code>");
                return;
            }
        }

        plugin.windows?.ToggleMainUI();
    }
}
