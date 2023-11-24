#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    public class CharacterStats
    {
        [HideInInspector]
        public StatList Stats = new();

        [ProgressBar(0, 100), ShowInInspector]
        public float Shooting
        {
            get => Stats[StatType.Shooting];
            set => Stats[StatType.Shooting] = value;
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Melee
        {
            get => Stats[StatType.Melee];
            set => Stats[StatType.Melee] = value;
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Social
        {
            get => Stats[StatType.Social];
            set => Stats[StatType.Social] = value;
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Animals
        {
            get => Stats[StatType.Animals];
            set => Stats[StatType.Animals] = value;
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Medicine
        {
            get => Stats[StatType.Medicine];
            set => Stats[StatType.Medicine] = value;
        }

        [ProgressBar(0, 100), ShowInInspector]
        public float Crafting
        {
            get => Stats[StatType.Crafting];
            set => Stats[StatType.Crafting] = value;
        }
    }
}
#endif
