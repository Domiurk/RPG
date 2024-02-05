using System.Collections.Generic;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using Editor;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;
    using Utilities;
    using Sirenix.Utilities.Editor;

    public class OdinMenuStyleExample : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/Odin Editor Window Demos/Odin Menu Style Example")]
        private static void OpenWindow()
        {
            var window = GetWindow<OdinMenuStyleExample>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);

            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 23,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;

            tree.Config.DrawSearchToolbar = true;

            tree.AddObjectAtPath("Menu Style", customMenuStyle);

            for (int i = 0; i < 5; i++)
            {
                var customObject = new SomeCustomClass() { Name = i.ToString() };
                var customMenuItem = new MyCustomMenuItem(tree, customObject);
                tree.AddMenuItemAtPath("Custom Menu Items", customMenuItem);
            }

            tree.AddAllAssetsAtPath("Scriptable Objects in Plugins Tree", "Plugins", typeof(ScriptableObject), true, false);
            tree.AddAllAssetsAtPath("Scriptable Objects in Plugins Flat", "Plugins", typeof(ScriptableObject), true, true);
            tree.AddAllAssetsAtPath("Only Configs has Icons", "Plugins/Sirenix", true, false);

            tree.EnumerateTree()
                .AddThumbnailIcons()
                .SortMenuItemsByName();

            return tree;
        }

        private class MyCustomMenuItem : OdinMenuItem
        {
            private readonly SomeCustomClass instance;

            public MyCustomMenuItem(OdinMenuTree tree, SomeCustomClass instance) : base(tree, instance.Name, instance)
            {
                this.instance = instance;
            }

            protected override void OnDrawMenuItem(Rect rect, Rect labelRect)
            {
                labelRect.x -= 16;
                instance.Enabled = GUI.Toggle(labelRect.AlignMiddle(18).AlignLeft(16), instance.Enabled, GUIContent.none);

                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
                {
                    IEnumerable<SomeCustomClass> selection = MenuTree.Selection
                                                                     .Select(x => x.Value)
                                                                     .OfType<SomeCustomClass>();

                    if (selection.Any())
                    {
                        bool enabled = !selection.FirstOrDefault().Enabled;
                        selection.ForEach(x => x.Enabled = enabled);
                        Event.current.Use();
                    }
                }
            }

            public override string SmartName => instance.Name;
        }

        private class SomeCustomClass
        {
            public bool Enabled = true;
            public string Name;
        }
    }
}
#endif
