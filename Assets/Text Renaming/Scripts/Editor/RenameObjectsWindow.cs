using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Text_Renaming.Scripts.Editor
{
    public class RenameObjectsWindow : EditorWindow
    {
        private string _prefix = "mixamorig:";
        private string _change = string.Empty;

        private readonly List<Object> _objects = new();

        [MenuItem("Tools/Rename Object Child")]
        public static void ShowExample()
        {
            Vector2 size = new Vector2(320, 185);
            RenameObjectsWindow window = GetWindow<RenameObjectsWindow>(true, "Rename");
            window.minSize = size;
            window.maxSize = size;
        }

        private Color defaultColor;

        private void OnGUI()
        {
            defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.gray;

            Header();

            GUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = defaultColor;
            Body();

            GUILayout.EndVertical();
            GUI.backgroundColor = defaultColor;
        }

        private static void Header()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Rename Mixamo",
                            new GUIStyle(GUI.skin.label){
                                alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold,
                                fontSize = 20
                            });
            GUILayout.EndVertical();
        }

        private void DragAndDropField(GUIStyle style = null)
        {
            style = style ?? GUI.skin.box;
            GUI.backgroundColor = Color.gray;
            Rect pos = EditorGUILayout.GetControlRect(false, 100);
            GUIContent contentBox = new GUIContent("Objects:");

            GUI.Box(pos, contentBox, new GUIStyle(style){ alignment = TextAnchor.MiddleCenter });

            CheckDragAndDrop(pos);
        }

        private void Body()
        {
            _prefix =
                EditorGUILayout.TextField(new GUIContent("Prefix", "Text which want to replace"), _prefix);
            _change =
                EditorGUILayout.TextField(new GUIContent("Replace", "Empty delete this prefix"), _change);

            if(string.IsNullOrEmpty(_prefix))
                EditorGUILayout.HelpBox("Write Prefix!!!", MessageType.Error);
            else
                DragAndDropField();
        }

        private void RenameTransform()
        {
            List<Transform> child = new List<Transform>();

            foreach(Object o in _objects){
                child.AddRange(((GameObject)o).GetComponentsInChildren<Transform>()
                                              .Where(t => t.name.Contains(_prefix)));
                Rename<Transform>(child);
            }
        }

        private void CheckDragAndDrop(Rect pos)
        {
            Event ev = Event.current;

            if(pos.Contains(ev.mousePosition)){
                switch(ev.type){
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        ev.Use();
                        break;
                    case EventType.DragPerform:{
                        _objects.Clear();

                        Object[] objects = DragAndDrop.objectReferences;

                        foreach(Object draggedObj in objects)
                            _objects.Add(draggedObj);
                        switch(_objects[0]){
                            case GameObject:
                                RenameTransform();
                                break;
                            case AnimationClip:
                                Rename<AnimationClip>(_objects);
                                break;
                        }

                        DragAndDrop.AcceptDrag();
                        ev.Use();
                        break;
                    }
                }
            }
        }

        private void Rename<TObject>(IEnumerable<Object> list) where TObject : Object
        {
            int count = 0;

            foreach(Object o in list){
                var obj = (TObject)o;

                if(obj == null || !obj.name.Contains(_prefix))
                    continue;
                string newName = obj.name.Replace(_prefix, _change);

                if(obj is AnimationClip){
                    string path = AssetDatabase.GetAssetPath(obj);
                    AssetDatabase.RenameAsset(path, newName);
                }
                else
                    obj.name = newName;

                count++;
            }

            Debug.Log($"You successful renamed {count} Objects.");
        }
    }
}