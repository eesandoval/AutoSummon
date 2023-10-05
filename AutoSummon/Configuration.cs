using Dalamud.Configuration;
using Dalamud.Plugin;

namespace AutoSummon
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 2;

        public bool Summoner { get; set; } = true;
        public bool Scholar {get; set; } = true;
        public bool Retry {get; set;} = false;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
