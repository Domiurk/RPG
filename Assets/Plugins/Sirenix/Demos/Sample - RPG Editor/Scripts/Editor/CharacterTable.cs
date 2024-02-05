#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class CharacterTable
    {
        [FormerlySerializedAs("allCharecters")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private readonly List<CharacterWrapper> allCharacters;

        public Character this[int index] => allCharacters[index].Character;

        public CharacterTable(IEnumerable<Character> characters)
        {
            allCharacters = characters.Select(x => new CharacterWrapper(x)).ToList();
        }

        private class CharacterWrapper
        {
            public Character Character { get; }

            public CharacterWrapper(Character character)
            {
                Character = character;
            }

            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
            public Texture Icon { get => Character.Icon;
                set { Character.Icon = value; EditorUtility.SetDirty(Character); } }

            [TableColumnWidth(120)]
            [ShowInInspector]
            public string Name { get => Character.Name;
                set { Character.Name = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Shooting { get => Character.Skills.Shooting;
                set { Character.Skills.Shooting = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Melee { get => Character.Skills.Melee;
                set { Character.Skills.Melee = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Social { get => Character.Skills.Social;
                set { Character.Skills.Social = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Animals { get => Character.Skills.Animals;
                set { Character.Skills.Animals = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Medicine { get => Character.Skills.Medicine;
                set { Character.Skills.Medicine = value; EditorUtility.SetDirty(Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Crafting { get => Character.Skills.Crafting;
                set { Character.Skills.Crafting = value; EditorUtility.SetDirty(Character); } }
        }
    }
}
#endif
