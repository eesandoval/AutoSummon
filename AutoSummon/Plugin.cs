using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using AutoSummon.Windows;

namespace AutoSummon
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Auto Summon";
        private const string CommandName = "/autosum";
        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("AutoSummon");
        private ConfigWindow ConfigWindow { get; init; }
        public static bool AutoSummon = false;
        private unsafe static ActionManager* AM;
        private uint summonerID = 25798;
        private uint scholarID = 17215; // eos RIP SELENE her id was 17216 ;-;

        public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface, [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterface.Create<ServiceHandler>(this);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            ConfigWindow = new ConfigWindow(this);
            WindowSystem.AddWindow(ConfigWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the config window"
            });
            unsafe
            {
                LoadUnsafe();
            }
            ServiceHandler.DutyState.DutyStarted += OnDutyReset;
            ServiceHandler.DutyState.DutyRecommenced += OnDutyReset;
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private unsafe void LoadUnsafe()
        {
            AM = ActionManager.Instance();
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }
        
        private void OnCommand(string command, string args)
        {
            ConfigWindow.IsOpen = true;
        }

        private unsafe void OnDutyReset(object? sender, ushort t)
        {
            bool summonResult = true; // by default assume we summoned since we haven't tried yet
            uint actionID = summonerID;
            // Immediate exit if anything is null/player not logged in
            if (ServiceHandler.ClientState == null || !ServiceHandler.ClientState.IsLoggedIn || ServiceHandler.ClientState.LocalPlayer == null)
            {
                return;
            }

            var classJobID = ServiceHandler.ClientState.LocalPlayer.ClassJob.Id; // 27 == summoner, 28 == scholar
            if (classJobID == 27 && this.Configuration.Summoner)
            {
                actionID = summonerID;
                summonResult = AM->UseAction(ActionType.Action, actionID); 
            }
            else if (classJobID == 28 && this.Configuration.Scholar)
            {
                actionID = scholarID;
                summonResult = AM->UseAction(ActionType.Action, actionID);
            }

            if (!summonResult && this.Configuration.Retry)
            {
                ServiceHandler.ChatGui.Print("Failed to summon, retrying...");
                summonResult = AM->UseAction(ActionType.Action, actionID);
            }

            // Supress for now
            //if (!result)
            //    ServiceHandler.ChatGui.PrintError("Failed to summon!");

        }

        // Below is never called, maybe in the future we do something with a UI if we ever want to auto summon eos?
        // The future is now old man
        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            this.ConfigWindow.IsOpen = true;
        }
    }
}