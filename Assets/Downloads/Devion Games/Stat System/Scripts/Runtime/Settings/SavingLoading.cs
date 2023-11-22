﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem.Configuration
{
    [System.Serializable]
    public class SavingLoading : Settings
    {
        public override string Name => "Saving & Loading";
        public bool autoSave = true;
        public string savingKey = "Player";
        public float savingRate = 60f;
        public SavingProvider provider;


        public enum SavingProvider
        {
            PlayerPrefs,
        }
    }
}