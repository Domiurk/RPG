#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Serialization;

    //
    // This class is used by the RPGEditorWindow to render an overview of all characters using the TableList attribute.
    // All characters are Unity objects though, so they are rendered in the inspector as single Unity object field,
    // which is not exactly what we want in our table. We want to show the members of the unity object.
    //
    // So in order to render the members of the Unity object, we'll create a class that wraps the Unity object
    // and displays the relevant members through properties, which works with the TableList, attribute.
    //

    public class CharacterTable
    {
        [FormerlySerializedAs("allCharecters")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private readonly List<CharacterWrapper> allCharacters;

        public Character this[int index] => this.allCharacters[index].Character;

        public CharacterTable(IEnumerable<Character> characters)
        {
            this.allCharacters = characters.Select(x => new CharacterWrapper(x)).ToList();
        }

        private class CharacterWrapper
        {
            // field if drawn in the inspector, which is not what we want.

            public Character Character { get; }

            public CharacterWrapper(Character character)
            {
                this.Character = character;
            }

            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
            public Texture Icon { get => this.Character.Icon;
                set { this.Character.Icon = value; EditorUtility.SetDirty(this.Character); } }

            [TableColumnWidth(120)]
            [ShowInInspector]
            public string Name { get => this.Character.Name;
                set { this.Character.Name = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Shooting { get => this.Character.Skills.Shooting;
                set { this.Character.Skills.Shooting = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Melee { get => this.Character.Skills.Melee;
                set { this.Character.Skills.Melee = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Social { get => this.Character.Skills.Social;
                set { this.Character.Skills.Social = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Animals { get => this.Character.Skills.Animals;
                set { this.Character.Skills.Animals = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Medicine { get => this.Character.Skills.Medicine;
                set { this.Character.Skills.Medicine = value; EditorUtility.SetDirty(this.Character); } }

            [ShowInInspector, ProgressBar(0, 100)]
            public float Crafting { get => this.Character.Skills.Crafting;
                set { this.Character.Skills.Crafting = value; EditorUtility.SetDirty(this.Character); } }
        }
    }
}
#endif
