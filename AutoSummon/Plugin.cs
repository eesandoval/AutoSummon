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
        public static bool AutoSummon = false;
        private unsafe static ActionManager* AM;

        public Plugin([RequiredVersion("1.0")] DalamudPluginInterface pluginInterface, [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterface.Create<ServiceHandler>(this);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            WindowSystem.AddWindow(new ConfigWindow(this));
            WindowSystem.AddWindow(new MainWindow(this));

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
            if (!this.Configuration.AutoSummon)
                ServiceHandler.ChatGui.Print("Enabled Auto Summoning");
            else
                ServiceHandler.ChatGui.Print("Disabled Auto Summoning");
            this.Configuration.AutoSummon = !this.Configuration.AutoSummon;
            AutoSummon = this.Configuration.AutoSummon;
            this.Configuration.Save();
        }

        private unsafe static void OnDutyReset(Duty d)
        {
            // Must have enabled autosummon, and the clientstate cannot be null. Must be logged in, player can't be null, and player must be 27 (summoner)
            if (!AutoSummon || ServiceHandler.ClientState == null || !ServiceHandler.ClientState.IsLoggedIn 
                || ServiceHandler.ClientState.LocalPlayer == null || ServiceHandler.ClientState.LocalPlayer.ClassJob.Id != 27)
                return;
            var result = AM->UseAction(ActionType.Spell, 25798);
            // Supress for now
            //if (!result)
            //    ServiceHandler.ChatGui.PrintError("Failed to summon!");

        }

        // Below is never called, maybe in the future we do something with a UI if we ever want to auto summon eos?
        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("A Wonderful Configuration Window").IsOpen = true;
        }
    }
}