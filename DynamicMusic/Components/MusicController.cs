using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DynamicMusic.Components
{
    internal class MusicController : MonoBehaviour
    {
        public AudioSource? audioPlayer;
        public static List<Music> musicList = [];
        public static readonly Dictionary<Type,Dictionary<MusicType,AudioClip>> musicDict = new();
        public List<Object> assetList = new();
        public void Awake()
        {
            if (MusicPlugin.bundle == null)
            {
                MusicPlugin.mls.LogInfo("bundle missing");
            }
            else
            {
                assetList = MusicPlugin.bundle.LoadAllAssets().ToList();
            }
            PopulateMusicDict();
            WriteMusicDict();
        }
        public void PopulateMusicDict()
        {
            MusicPlugin.mls.LogInfo("populating");
            AudioClip? clip = null;
            MusicType type = MusicType.Encounter;
            if (MusicPlugin.bundle == null) return;
            for (int i = 0; i < MusicPlugin.objects.Count; i++)
            {
                musicDict.Add(MusicPlugin.objects[i], new Dictionary<MusicType, AudioClip>());
                foreach(var b in assetList)
                {
                    if (MusicPlugin.objects[i].Name.Contains(b.name.Split('_')[0]))
                    {
                        if (b.name.ToLower().Contains("enc")) type = MusicType.Encounter;
                        if (b.name.ToLower().Contains("att")) type = MusicType.Attack;
                        if (b.name.ToLower().Contains("esc")) type = MusicType.Escape;
                       // MusicPlugin.mls.LogInfo(b.name);
                        clip = b as AudioClip;
                        musicDict[MusicPlugin.objects[i]].Add(type, clip);
                        continue;
                    }
                }
            }
        }
        public void WriteMusicDict()
        {
            MusicPlugin.mls.LogInfo("writing");
            foreach (var i in musicDict.Keys)
            {
                MusicPlugin.mls.LogInfo(i.Name);
                foreach (var j in musicDict[i].Keys)
                {
                    //MusicPlugin.mls.LogInfo(j.ToString());
                    MusicPlugin.mls.LogInfo(musicDict[i][j].name);
                }
            }
        }
        public static AudioClip PickEventByTypeAndEnemy(Type type, MusicType musicType)
        {
            return musicDict[type][musicType];
        }
    }
}
