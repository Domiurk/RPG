#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using Editor;
    using System.Linq;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using Serialization;
    using UnityEditor;
    using Utilities;

    public class OdinMenuEditorWindowExample : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/Odin Editor Window Demos/Odin Menu Editor Window Example")]
        private static void OpenWindow()
        {
            var window = GetWindow<OdinMenuEditorWindowExample>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [SerializeField]
        private SomeData someData = new();

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "Home",                           this,                           EditorIcons.House                       },
                { "Odin Settings",                  null,                           SdfIconType.GearFill                    },
                { "Odin Settings/Color Palettes",   ColorPaletteManager.Instance,   SdfIconType.PaletteFill                 },
                { "Odin Settings/AOT Generation",   AOTGenerationConfig.Instance,   EditorIcons.SmartPhone                  },
                { "Player Settings",                Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault()       },
                { "Some Class",                     someData                                                           }
            };

            tree.AddAllAssetsAtPath("Odin Settings/More Odin Settings", "Plugins/Sirenix", typeof(ScriptableObject), true)
                .AddThumbnailIcons();

            tree.AddAssetAtPath("Odin Getting Started", "Plugins/Sirenix/Getting Started With Odin.asset");

            tree.MenuItems.Insert(2, new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle));

            tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
            tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

            tree.SortMenuItemsByName();

            return tree;
        }
    }
}
#endif
