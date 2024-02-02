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
                musicDict.Add(MusicPlugin.objects[i].GetType(), new Dictionary<MusicType, AudioClip>());
                if (i >= assetList.Count) continue;
                foreach(var b in assetList)
                {
                    if(b.name =)
                }
                if (assetList[i].name.ToLower().Contains("enc")) type = MusicType.Encounter;
                if (assetList[i].name.ToLower().Contains("att")) type = MusicType.Attack;
                if (assetList[i].name.ToLower().Contains("esc")) type = MusicType.Escape;
                MusicPlugin.mls.LogInfo(assetList[i].name);
                clip = assetList[i] as AudioClip;
            }
            for(int j = 0; j < musicDict.Count; j++)
            {

                if (clip == null)
                {
                    continue;
                }
                musicDict[MusicPlugin.objects[j]].Add(type, clip);
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
                    MusicPlugin.mls.LogInfo(j.ToString());
                    MusicPlugin.mls.LogInfo(musicDict[i][j].name);
                }
            }
        }
        
        public static Music PickMusicByType(MusicType type)
        {
            foreach(Music music in musicList)
            {
                if(music.Type == type) return music;
            }
            MusicPlugin.mls.LogInfo("failed to find music by type! Returning first available music!");
            return musicList[0];
        }
        public static Music PickMusicByEnemyName(string name)
        {
            foreach(Music music in musicList)
            {
                if (name.Contains(music.Name.Split('_')[0])) return music;
            }
            MusicPlugin.mls.LogInfo("failed to find music by name! Returning first available music!");
            return musicList[0];
        }
        public static Music PickMusicByNameAndType(string name,MusicType type)
        {
            Music nameSearch = PickMusicByEnemyName(name);
            Music typeSearch = PickMusicByType(type);
            foreach(Music music in musicList)
            {
                if(music.Type == typeSearch.Type && music.Name == nameSearch.Name) return music;
            }
            MusicPlugin.mls.LogInfo("Failed to find music by name and type! Returning first available music!");
            return musicList[0];
        }
    }
}
