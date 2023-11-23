using System.Collections.Generic;
using UnityEditor;

namespace DevionGames
{
    [System.Serializable]
    public abstract class ObjectCollectionEditor<T> : CollectionEditor<T> where T : INameable
    {
        protected string m_ToolbarName;

        protected int m_TargetInstanceID;
        protected string m_SerializedPropertyPath;
        protected SerializedObject m_SerializedObject;
        protected SerializedProperty m_SerializedProperty;

        public override string ToolbarName => m_ToolbarName;

        protected override List<T> Items => m_SerializedProperty.GetValue() as List<T>;


        public ObjectCollectionEditor(SerializedObject serializedObject, SerializedProperty serializedProperty) : this(ObjectNames.NicifyVariableName(typeof(T).Name+"s"), serializedObject, serializedProperty)
        {
        }

        public ObjectCollectionEditor(string toolbar, SerializedObject serializedObject, SerializedProperty serializedProperty) {
            m_SerializedObject = serializedObject;
            m_SerializedProperty = serializedProperty;
            m_TargetInstanceID = serializedObject.targetObject.GetInstanceID();
            m_SerializedPropertyPath = serializedProperty.propertyPath;
            m_ToolbarName = toolbar;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected override void DrawItem(T item)
        {
            int index = Items.IndexOf(item);
            m_SerializedObject.Update();

            SerializedProperty element = m_SerializedProperty.GetArrayElementAtIndex(index);
            object value = element.GetValue();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", value != null ? EditorTools.FindMonoScript(value.GetType()) : null, typeof(MonoScript), true);
            EditorGUI.EndDisabledGroup();
         
            foreach (var child in element.EnumerateChildProperties())
            {
                EditorGUILayout.PropertyField(
                    child,
                    includeChildren: true
                );
                EditorGUI.EndDisabledGroup();
            }

            m_SerializedObject.ApplyModifiedProperties();
        }

        protected override void Create()
        {
            T value = (T)System.Activator.CreateInstance(typeof(T));
            m_SerializedObject.Update();
            m_SerializedProperty.arraySize++;
            m_SerializedProperty.GetArrayElementAtIndex(m_SerializedProperty.arraySize - 1).managedReferenceValue = value;
            m_SerializedObject.ApplyModifiedProperties();
        }

        protected override void Remove(T item)
        {
            m_SerializedObject.Update();
            int index = Items.IndexOf(item);
            m_SerializedProperty.DeleteArrayElementAtIndex(index);
            m_SerializedObject.ApplyModifiedProperties();
        }

        protected override void Duplicate(T item)
        {
            T duplicate = (T)EditorTools.Duplicate(item);
            m_SerializedObject.Update();
            m_SerializedProperty.arraySize++;
            m_SerializedProperty.GetArrayElementAtIndex(m_SerializedProperty.arraySize - 1).managedReferenceValue = duplicate;
            m_SerializedObject.ApplyModifiedProperties();
        }

        protected override string GetSidebarLabel(T item)
        {
            return item.Name;
        }

        protected override bool MatchesSearch(T item, string search)
        {
            return (item.Name.ToLower().Contains(search.ToLower()) || search.ToLower() == item.GetType().Name.ToLower());
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            m_SerializedObject = new SerializedObject(EditorUtility.InstanceIDToObject(m_TargetInstanceID));
            m_SerializedProperty = m_SerializedObject.FindProperty(m_SerializedPropertyPath);
        }

        private void OnAfterAssemblyReload()
        {
            m_SerializedObject = new SerializedObject(EditorUtility.InstanceIDToObject(m_TargetInstanceID));
            m_SerializedProperty = m_SerializedObject.FindProperty(m_SerializedPropertyPath);
        }

    }
}