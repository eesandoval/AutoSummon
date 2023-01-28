using Dalamud.Configuration;
using Dalamud.Plugin;

namespace AutoSummon
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool Summoner { get; set; } = true;
        public bool Scholar {get; set; } = true;
        public int ScholarPet {get; set; } = 0; // 0 = Eos, 1 = Selene
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
