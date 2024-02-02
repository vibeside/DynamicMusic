using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using DynamicMusic.Components;
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
        public static ManualLogSource mls;
        public void Awake()
        {
            mls = base.Logger;
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            bundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "dynamicmusicbundle"));
            musicController = new GameObject(modName);
            musicController.AddComponent<MusicController>();
            musicController.AddComponent<AudioSource>();
            //On.GameNetcodeStuff.PlayerControllerB.Update += PlayerControllerB_Update;
        }

        //private void PlayerControllerB_Update(On.GameNetcodeStuff.PlayerControllerB.orig_Update orig, GameNetcodeStuff.PlayerControllerB self)
        //{
        //    self.sprintMeter = 1f;
        //    orig(self);
        //}
    }
}
