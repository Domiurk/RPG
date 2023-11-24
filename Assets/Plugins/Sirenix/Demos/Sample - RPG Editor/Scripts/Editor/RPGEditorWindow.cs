#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using Editor;
    using Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Linq;

    public class RPGEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/RPG Editor")]
        private static void Open()
        {
            var window = GetWindow<RPGEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true){
                DefaultMenuStyle ={
                    IconSize = 28.00f
                },
                Config ={
                    DrawSearchToolbar = true
                }
            };

            CharacterOverview.Instance.UpdateCharacterOverview();
            tree.Add("Characters", new CharacterTable(CharacterOverview.Instance.AllCharacters));

            tree.AddAllAssetsAtPath("Characters", "Assets/Plugins/Sirenix", typeof(Character), true, true);

            tree.AddAllAssetsAtPath("", "Assets/Plugins/Sirenix/Demos/SAMPLE - RPG Editor/Items", typeof(Item), true)
                .ForEach(AddDragHandles);

            tree.EnumerateTree().Where(x => x.Value as Item).ForEach(AddDragHandles);

            tree.EnumerateTree().AddIcons<Character>(x => x.Icon);
            tree.EnumerateTree().AddIcons<Item>(x => x.Icon);

            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += _ => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuItem selected = MenuTree.Selection.FirstOrDefault();
            int toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                {
                    ScriptableObjectCreator.ShowDialog<Item>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Items", obj =>
                    {
                        obj.Name = obj.name;
                        TrySelectMenuItemWithObject(obj);
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Character")))
                {
                    ScriptableObjectCreator.ShowDialog<Character>("Assets/Plugins/Sirenix/Demos/Sample - RPG Editor/Character", obj =>
                    {
                        obj.Name = obj.name;
                        TrySelectMenuItemWithObject(obj);
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
#endif
