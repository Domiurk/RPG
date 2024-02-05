using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatDatabase : ScriptableObject
    {
        public List<Stat> items = new();
        public List<StatEffect> effects = new();
        public List<Configuration.Settings> settings = new();
    }
}