using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DynamicMusic.Components
{
    internal class Music
    {
        public string Name;
        public MusicType Type;
        public AudioClip Clip;
        public Music(string _name, MusicType _type, AudioClip _clip)
        {
            Name = _name;
            Type = _type;
            Clip = _clip;
        }
    }
}
