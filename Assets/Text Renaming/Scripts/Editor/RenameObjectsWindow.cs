using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Text_Renaming.Scripts.Editor
{
    public class RenameObjectsWindow : EditorWindow
    {
        private string prefix = "mixamorig:";
        private string change = string.Empty;

        private readonly List<Object> dragObjects = new();

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
            prefix =
                EditorGUILayout.TextField(new GUIContent("Prefix", "Text which want to replace"), prefix);
            change =
                EditorGUILayout.TextField(new GUIContent("Replace", "Empty delete this prefix"), change);

            if(string.IsNullOrEmpty(prefix))
                EditorGUILayout.HelpBox("Write Prefix!!!", MessageType.Error);
            else
                DragAndDropField();
        }

        private void RenameTransform()
        {
            List<Transform> child = new List<Transform>();

            foreach(Object o in dragObjects){
                child.AddRange(((GameObject)o).GetComponentsInChildren<Transform>()
                                              .Where(t => t.name.Contains(prefix)));
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
                        this.dragObjects.Clear();

                        Object[] objects = DragAndDrop.objectReferences;

                        foreach(Object draggedObj in objects)
                            this.dragObjects.Add(draggedObj);
                        switch(this.dragObjects[0]){
                            case GameObject:
                                RenameTransform();
                                break;
                            case AnimationClip:
                                Rename<AnimationClip>(this.dragObjects);
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

                if(obj == null || !obj.name.Contains(prefix))
                    continue;
                string newName = obj.name.Replace(prefix, change);

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