﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using BepInEx;
using BepInEx.Logging;
using DynamicMusic.Components;
using HarmonyLib;
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
        public static Hook? enemyStartHook;
        public static List<Type> objects = new();
        public void Awake()
        {
            mls = base.Logger;
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            bundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "dynamicmusicbundle"));

            foreach (Type type in
            Assembly.GetAssembly(typeof(EnemyAI)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(EnemyAI))))
            {
                objects.Add(type);
            }
            BepInEx.Bootstrap.Chainloader.Plugins.Do(plugin =>
            {
                objects.AddRange(Assembly.GetAssembly(plugin.GetType()).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(EnemyAI))));
            });


            musicController = new GameObject(modName);
            musicController.hideFlags = HideFlags.HideAndDontSave;
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
                    //if (self.TryGetComponent(out EnemyData data))
                    {
                        if (EnemyData.encountered == false)
                        {
                            EnemyData.encountered = true;
                            musicSource.clip = MusicController.PickMusicByEnemyAndType(self.GetType(), MusicType.Encounter);
                            musicSource.PlayOneShot(MusicController.PickMusicByEnemyAndType(self.GetType(), MusicType.Encounter));
                        }
                    }
                }
                if((self.movingTowardsTargetPlayer || self.moveTowardsDestination) && self.targetPlayer == GameNetworkManager.Instance.localPlayerController)
                {
                    mls.LogInfo("YOUR THE TARGET");
                }
                original(self);
            });
            enemyHook = new(
            typeof(SandSpiderAI).GetMethod(nameof(SandSpiderAI.Start), (BindingFlags)int.MaxValue),
            (Action<SandSpiderAI> original, SandSpiderAI self) =>
            {
                original(self);
                self.gameObject.AddComponent<EnemyData>();
            });
        }
    }
}
