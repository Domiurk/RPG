﻿using UnityEngine;

namespace DevionGames.StatSystem.Configuration
{
    [System.Serializable]
    public abstract class Settings : ScriptableObject, INameable
    {
        public virtual string Name
        {
            get => "Settings";
            set { }
        }
    }
}