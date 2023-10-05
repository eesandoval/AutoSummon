using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AutoSummon.Windows;
public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Auto Summon Config", 
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(200, 175);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var summonerConfig = this.Configuration.Summoner;
        var scholarConfig = this.Configuration.Scholar;
        var retryConfig = this.Configuration.Retry;

        if (ImGui.Checkbox("Summoner", ref summonerConfig))
        {
            this.Configuration.Summoner = summonerConfig;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Scholar", ref scholarConfig))
        {
            this.Configuration.Scholar = scholarConfig;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Retry", ref retryConfig))
        {
            this.Configuration.Retry = retryConfig;
            this.Configuration.Save();
        }
    }
}