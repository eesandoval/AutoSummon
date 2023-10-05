using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
namespace AutoSummon
{
    public class ServiceHandler
    {
        [PluginService]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService]
        public static IDataManager DataManager { get; private set; } = null!;
        [PluginService]
        public static IClientState ClientState { get; private set; } = null!;
        [PluginService]
        public static IFramework Framework { get; private set; } = null!;
        [PluginService]
        public static IChatGui ChatGui { get; private set; } = null!;
    }
}