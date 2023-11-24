using UnityEngine;

namespace DevionGames.InventorySystem.Configuration
{
    [System.Serializable]
    public class Default : Settings
    {
        public override string Name => "Default";

        public string playerTag = "Player";
        public float maxDropDistance = 3f;

        [Header("Physics")]
        public bool queriesHitTriggers;

        [Header("Debug")]
        public bool debugMessages = true;
        public bool showAllComponents;
    }
}