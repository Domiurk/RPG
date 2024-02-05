using UnityEngine;

namespace DevionGames.InventorySystem.Configuration
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