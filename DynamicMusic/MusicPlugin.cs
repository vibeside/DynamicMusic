using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using DynamicMusic.Components;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace DynamicMusic
{
    public enum MusicType
    {
        Encounter,
        Attack,
        Escape
    }
    [BepInPlugin(modGUID,modName,"1.0.0.0")]
    public class MusicPlugin : BaseUnityPlugin
    {
        public const string modGUID = "grugjosh.lethalcompany.DynamicMusic";
        public const string modName = "DynamicMusic";
        public static AssetBundle? bundle;
        public static GameObject? musicController;
        internal static MusicController? controller;
        public static AudioSource? musicSource;
        public static ManualLogSource mls;
        public static Hook? enemyHook;
        public void Awake()
        {
            mls = base.Logger;
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            bundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "dynamicmusicbundle"));
            musicController = new GameObject(modName);
            controller = musicController.AddComponent<MusicController>();
            musicSource = musicController.AddComponent<AudioSource>();
            enemyHook = new(
            typeof(SandSpiderAI).GetMethod(nameof(SandSpiderAI.Update), (BindingFlags)int.MaxValue),
            (Action<SandSpiderAI> original, SandSpiderAI self) =>
            {
                if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
                {
                    original(self);
                    return;
                }
                if (GameNetworkManager.Instance.localPlayerController.HasLineOfSightToPosition(self.transform.position))
                {
                    if(self.TryGetComponent(out EnemyData data))
                    {
                        data.encountered = true;
                        mls.LogInfo("spotted");
                        musicSource.clip = MusicController.PickMusicByNameAndType(self.enemyType.enemyName,MusicType.Encounter).Clip;
                    }
                }
                original(self);
            });
            //On.GameNetcodeStuff.PlayerControllerB.Update += PlayerControllerB_Update;
        }

        //private void PlayerControllerB_Update(On.GameNetcodeStuff.PlayerControllerB.orig_Update orig, GameNetcodeStuff.PlayerControllerB self)
        //{
        //    self.sprintMeter = 1f;
        //    orig(self);
        //}
    }
}
