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
        public static readonly Dictionary<string,Dictionary<MusicType,AudioClip>> musicDict = new();
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
            PopulateMusicList();
            WriteMusicList();
        }
        public void PopulateMusicDict()
        {
            AudioClip? clip;
            MusicType type = MusicType.Encounter;
            string name = "";
            Dictionary<MusicType, AudioClip>? dict;
            if (MusicPlugin.bundle == null) return;
            for (int i = 0; i < assetList.Count; i++)
            {
                clip = assetList[i] as AudioClip;
                name = clip.name.ToLower();
                if (clip == null) continue;
            }
        }
        public void PopulateMusicList()
        {
            AudioClip? clip;
            MusicType type = MusicType.Encounter;
            string name = "";
            if (MusicPlugin.bundle == null) return;
            string[] names = MusicPlugin.bundle.GetAllAssetNames();
            for (int i = 0; i < names.Count(); i++)
            {
                clip = assetList[i] as AudioClip;
                name = clip.name.ToLower();
                if(clip == null) continue;
                if (name.Contains("enc")) type = MusicType.Encounter;
                if (name.Contains("att")) type = MusicType.Attack;
                if (name.Contains("esc")) type = MusicType.Escape;
                musicList.Add(new Music(names[i],type,clip));
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
        public void WriteMusicList()
        {
            foreach(var i in musicList)
            {
                MusicPlugin.mls.LogInfo(i.Type.ToString());
            }
        }
    }
}
