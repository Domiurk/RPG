#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using UnityEngine;

    public class Character : SerializedScriptableObject
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture Icon;

        [VerticalGroup("Split/Meta")]
        public string Name;

        [VerticalGroup("Split/Meta")]
        public string Surname;

        [VerticalGroup("Split/Meta"), Range(0, 100)]
        public int Age;

        [HorizontalGroup("Split", 290), EnumToggleButtons, HideLabel]
        public CharacterAlignment CharacterAlignment;

        [TabGroup("Starting Inventory")]
        public ItemSlot[,] Inventory = new ItemSlot[12, 6];

        [TabGroup("Starting Stats"), HideLabel]
        public CharacterStats Skills = new();

        [HideLabel]
        [TabGroup("Starting Equipment")]
        public CharacterEquipment StartingEquipment;
    }
}
#endif
