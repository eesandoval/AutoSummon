using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using AutoSummon.Windows;
using Dalamud.Game.Gui;
using Dalamud;
using Dalamud.Game.ClientState;

namespace AutoSummon
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Auto Summon";
        private const string CommandName = "/autosum";
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("AutoSummon");
        private ConfigWindow ConfigWindow { get; init; }
        //private MainWindow MainWindow { get; init; }
        public static bool AutoSummon = false;
        private unsafe static ActionManager* AM;
        private uint summonerID = 25798;
        private uint[] scholarIDs = {17215, 17216}; // eos, selene

        public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface, [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterface.Create<ServiceHandler>(this);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            ConfigWindow = new ConfigWindow(this);
            //MainWindow = new MainWindow(this);
            WindowSystem.AddWindow(ConfigWindow);
            //WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Auto summons carbuncle"
            });
            unsafe
            {
                LoadUnsafe();
            }
            DutyStatus.Instance.OnDutyReset += OnDutyReset;
            DutyStatus.Instance.OnEnterDuty += OnDutyReset;
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
            ServiceHandler.ChatGui.Print("Opening Config Window");
            ConfigWindow.IsOpen = true;
        }

        private unsafe void OnDutyReset(Duty d)
        {
            bool summonResult = true; // by default assume we summoned since we haven't tried yet
            uint actionID = summonerID;
            // Immediate exit if anything is null/player not logged int
            if (ServiceHandler.ClientState == null || !ServiceHandler.ClientState.IsLoggedIn || ServiceHandler.ClientState.LocalPlayer == null)
            {
                return;
            }

            var classJobID = ServiceHandler.ClientState.LocalPlayer.ClassJob.Id; // 27 == summoner, 28 == scholar
            if (classJobID == 27 && this.Configuration.Summoner)
            {
                actionID = summonerID;
                summonResult = AM->UseAction(ActionType.Spell, actionID); 
            }
            else if (classJobID == 28 && this.Configuration.Scholar)
            {
                actionID = scholarIDs[this.Configuration.ScholarPet];
                summonResult = AM->UseAction(ActionType.Spell, actionID);
            }

            if (!summonResult && this.Configuration.Retry)
            {
                ServiceHandler.ChatGui.Print("Failed to summon, retrying...");
                summonResult = AM->UseAction(ActionType.Spell, actionID);
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