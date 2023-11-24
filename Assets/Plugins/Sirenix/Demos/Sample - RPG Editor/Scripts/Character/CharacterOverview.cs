#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using Utilities;
    using System.Linq;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [GlobalConfig("Plugins/Sirenix/Demos/RPG Editor/Characters")]
    public class CharacterOverview : GlobalConfig<CharacterOverview> 
    {
        [ReadOnly]
        [ListDrawerSettings(ShowFoldout =  true)]
        public Character[] AllCharacters;

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateCharacterOverview()
        {
            AllCharacters = AssetDatabase.FindAssets("t:Character")
                                         .Select(guid => AssetDatabase.LoadAssetAtPath<Character>(AssetDatabase.GUIDToAssetPath(guid)))
                                         .ToArray();
        }
#endif
    }
}
#endif
