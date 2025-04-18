﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    [CreateAssetMenu(fileName = "Audio Playlist", menuName = "Devion Games/Utilities/Audio Playlist")]
    [Serializable]
    public class AudioPlaylist : ScriptableObject
    {
        [SerializeField]
        protected List<AudioClip> m_Clips;

        public AudioClip this[int index] => m_Clips[index];

        public int Count => m_Clips.Count;
    }
}