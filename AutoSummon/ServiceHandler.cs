using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace AutoSummon
{
    public class ServiceHandler
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [PluginService]
        static internal DalamudPluginInterface PluginInterface { get; private set; }
        [PluginService]
        static internal DataManager DataManager { get; private set; }
        [PluginService]
        static internal ClientState ClientState { get; private set; }
        [PluginService]
        static internal Framework Framework { get; private set; }
        [PluginService]
        static internal ChatGui ChatGui { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
