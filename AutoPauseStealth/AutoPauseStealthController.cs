﻿using System.Linq;
using UnityEngine;

namespace AutoPauseStealth
{
    public class AutoPauseStealthController : MonoBehaviour
    {
        public static AutoPauseStealthController instance { get; private set; }
        public static GamePause GamePause;
        public static bool StabilityPeriodActive;

        public void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene prevScene, UnityEngine.SceneManagement.Scene nextScene)
        {
            Logger.log?.Debug($"{name}: LoadingScene({nextScene.name})");

            if (!PluginSettings.instance.ConfigIntializationOk)
                return;

            if (StabilityPeriodActive) // because of fast restart/exit combined with long StabilityDurationCheck
            {
                CancelInvoke("StopStabilityCheckPeriod");
                StabilityPeriodActive = false;
            }

            if (nextScene.name == "GameCore")
            {
                GamePause = Resources.FindObjectsOfTypeAll<GamePause>().FirstOrDefault();
                if (GamePause == null)
                    Logger.log?.Error("Couldn't find GamePause object");
                b_inGame = true;
                f_stabilityTimer = 0.0f;
                Invoke("StopStabilityCheckPeriod", PluginSettings.instance.MaxWaitingTime);
            }
            else
            {
                GamePause = null;
                b_inGame = false;
            }
        }

        // Prevent game from unpausing if paused during StabilityCheck period
        public void OnPauseShowMenu()
        {
            if (StabilityPeriodActive)
            {
                Logger.log?.Debug($"Pause requested during StabilityCheck period => Turn off Stability check and cancel StopStabilityCheckPeriod routine");
                StabilityPeriodActive = false;
                CancelInvoke("StopStabilityCheckPeriod");
            }
        }



        private void Awake()
        {
            if (instance != null)
            {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            instance = this;
            Logger.log?.Debug($"{name}: Awake()");
        }

        private void Start()
        {
            b_inGame = false;
            StabilityPeriodActive = false;
            Logger.log?.Debug($"{name}: Start()");
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void StopStabilityCheckPeriod()
        {
            Logger.log?.Info($"StabilityCheckPeriod over, resuming game");
            GamePause.Resume();
            StabilityPeriodActive = false;
        }

        private void Update()
        {
            if (PluginSettings.instance.ConfigIntializationOk && b_inGame && StabilityPeriodActive) 
            {
                f_fps = 1.0f / Time.deltaTime;

                if (f_fps > PluginSettings.instance.FpsThresold)
                {
                    f_stabilityTimer += Time.deltaTime;
                    if (f_stabilityTimer >= PluginSettings.instance.StabilityDurationCheck)
                    {
                        Logger.log?.Info($"Initialization Lag finished, resuming game");
                        GamePause.Resume();
                        CancelInvoke("StopStabilityCheckPeriod");
                        StabilityPeriodActive = false;
                    }
                }
                else
                {
                    f_stabilityTimer = 0.0f;
                }
            }
        }

        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            instance = null;
        }



        private float f_fps;
        private float f_stabilityTimer;
        private bool b_inGame;
    }
}
