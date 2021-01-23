using AutoPauseStealth.Configuration;
using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;

namespace AutoPauseStealth
{
    public class PluginSettings : PersistentSingleton<PluginSettings>
    {
        private float DetermineMinFPSSub()
        {
            float hrmFrameRate = UnityEngine.XR.XRDevice.refreshRate;
            if (hrmFrameRate == 0.0f)
            {
                Logger.log?.Error("Couldn't get HRM FrameRate, assuming it's 80 fps");
                hrmFrameRate = 80.0f;
            }
            return UnityEngine.Mathf.Round(hrmFrameRate) - 5.0f;
        }

        private void Awake()
        {
            if (FpsThresold == 0.0f)
                FpsThresold = DetermineMinFPSSub();
            RecommendedFpsThresold = "Recommended Min FPS value for your headset is " + DetermineMinFPSSub();
        }

        [UIValue("fpsThresold")]
        public float FpsThresold
        {
            get => PluginConfig.Instance.FpsThresold;
            set => PluginConfig.Instance.FpsThresold = value;
        }

        [UIValue("stabilityDurationCheck")]
        public float StabilityDurationCheck
        {
            get => PluginConfig.Instance.StabilityDurationCheck;
            set => PluginConfig.Instance.StabilityDurationCheck = value;
        }

        [UIValue("maxWaitingTime")]
        public float MaxWaitingTime
        {
            get => PluginConfig.Instance.MaxWaitingTime;
            set => PluginConfig.Instance.MaxWaitingTime = value;
        }

        [UIValue("reloadOnFailStab")]
        public bool ReloadOnFailStab
        {
            get => PluginConfig.Instance.ReloadOnFailStab;
            set => PluginConfig.Instance.ReloadOnFailStab = value;
        }

        [UIValue("RecommendedFpsThresold")]
        public string RecommendedFpsThresold;
    }
}