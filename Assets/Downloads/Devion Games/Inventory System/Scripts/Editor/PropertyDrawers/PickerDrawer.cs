using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace DevionGames.InventorySystem
{
	[CustomPropertyDrawer (typeof(PickerAttribute), true)]
	public abstract class PickerDrawer<T> : PropertyDrawer where T: ScriptableObject, INameable
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			T current = (T)property.GetValue();
			position = EditorGUI.PrefixLabel(position, label);
			DoSelection (position, property, label, current);
			EditorGUI.EndProperty();
		}

		protected virtual void DoSelection (Rect buttonRect, SerializedProperty property, GUIContent label, T current)
		{

			GUIStyle buttonStyle = EditorStyles.objectField;
			GUIContent buttonContent = new GUIContent(current != null ? current.Name : "Null");
			if (GUI.Button(buttonRect, buttonContent, buttonStyle))
			{
				ObjectPickerWindow.ShowWindow(buttonRect, typeof(ItemDatabase), BuildSelectableObjects(),
					(Object obj) => {
						property.serializedObject.Update();
						property.objectReferenceValue = obj;
						property.serializedObject.ApplyModifiedProperties();
					},
					() => {
						ItemDatabase db = EditorTools.CreateAsset<ItemDatabase>(true);
					});
			}
		}

		protected abstract List<T> GetItems(ItemDatabase database);

		protected Dictionary<Object,List<Object>> BuildSelectableObjects()
		{
			Dictionary<Object,List<Object>> selectableObjects = new Dictionary<Object, List<Object>>();

			string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase));
				List<Object> items = GetItems(obj as ItemDatabase).Cast<Object>().ToList();
				for (int j = 0; j < items.Count; j++){
					items[j].name = (items[j] as INameable).Name; 
				}
				selectableObjects.Add(obj, items);
			}
			return selectableObjects;
		}
	}
}