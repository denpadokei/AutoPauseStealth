using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace AutoPauseStealth.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual float FpsThresold { get; set; } = 0.0f;
        public virtual float StabilityDurationCheck { get; set; } = 0.3f;
        public virtual float MaxWaitingTime { get; set; } = 5.0f;
        public virtual bool ReloadOnFailStab { get; set; } = false;

        public event Action<PluginConfig> OnReloaded;
        public event Action<PluginConfig> OnConfigChanged;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
            this.OnReloaded?.Invoke(this);
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            this.OnConfigChanged?.Invoke(this);
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
            this.FpsThresold = other.FpsThresold;
            this.StabilityDurationCheck = other.StabilityDurationCheck;
            this.MaxWaitingTime = other.MaxWaitingTime;
            this.ReloadOnFailStab = other.ReloadOnFailStab;
        }
    }
}