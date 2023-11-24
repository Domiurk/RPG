using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevionGames.InventorySystem
{
    public class ItemReferenceEditor : EditorWindow
    {

		public static void ShowWindow()
		{

			ItemReferenceEditor window = GetWindow<ItemReferenceEditor>(true, "Item Reference Updater");
			Vector2 size = new Vector2(450f, 270f);
			window.minSize = size;
			window.wantsMouseMove = true;
		}

		
		[SerializeField]
		private List<SceneAsset> m_Scenes= new();
		private ItemDatabase m_Database;
		private string m_PrefabsPath;
		private int m_ChangedReferences;

		private void OnGUI()
        {
			EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
			SceneSelection();
			SelectPrefabsFolder();
			SelectDatabase();
			EditorGUILayout.Space(7);

			GUILayout.Label("Changed References: "+ m_ChangedReferences);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(m_Database == null);
			if (GUILayout.Button("Update", "AC Button"))
			{
				UpdateScenes();

				UpdateGameObjects(GetPrefabs());

			}
			EditorGUI.EndDisabledGroup();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		private List<GameObject> GetPrefabs()
		{
			List<GameObject> result = new List<GameObject>();
			if (!string.IsNullOrEmpty(m_PrefabsPath))
			{
				string[] temp = AssetDatabase.GetAllAssetPaths();
				foreach (string s in temp)
				{
					if (s.Contains(".prefab") && s.Contains(m_PrefabsPath.Substring(m_PrefabsPath.IndexOf("Assets/"))))
					{
						result.Add(AssetDatabase.LoadAssetAtPath<GameObject>(s));
					}
				}
			}
			return result;
		}

		private void UpdateScenes() {
			m_ChangedReferences = 0;
			for (int i = 0; i < m_Scenes.Count; i++)
			{
				
				List<GameObject> rootObjectsInScene = new List<GameObject>();
				
				string scenePath = AssetDatabase.GetAssetPath(m_Scenes[i]);
				if (EditorPrefs.GetBool("ItemReference.Scene." + scenePath, true))
				{
					Scene scene = SceneManager.GetSceneByPath(scenePath);
					bool isLoaded = scene.isLoaded;

					if (!isLoaded)
					{
						scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
					}
					scene.GetRootGameObjects(rootObjectsInScene);
					UpdateGameObjects(rootObjectsInScene);
					EditorSceneManager.SaveScene(scene);
					if (!isLoaded)
						EditorSceneManager.CloseScene(scene, false);
				}
			}
		}

        private void UpdateGameObjects(List<GameObject> gameObjects)
        {
			for (int j = 0; j < gameObjects.Count; j++)
			{
				Component[] allComponents = gameObjects[j].GetComponentsInChildren<Component>(true);
				for (int k = 0; k < allComponents.Length; k++)
				{
					Debug.Log(allComponents[k]);
					UpdateComponent(allComponents[k]);
				}
				EditorUtility.SetDirty(gameObjects[j]);
			
				if (EditorUtility.IsPersistent(gameObjects[j]))
				{
					
					GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(gameObjects[j]);
					PrefabUtility.ApplyPrefabInstance(go, InteractionMode.AutomatedAction);
					AssetDatabase.SaveAssets();
					DestroyImmediate(go);
				}
				
			}
		}

		private void UpdateComponent(Component component) {
			List<INameable> items = new List<INameable>();
			items.AddRange(m_Database.items);
			items.AddRange(m_Database.categories);
			items.AddRange(m_Database.raritys);
			items.AddRange(m_Database.equipments);
			items.AddRange(m_Database.currencies);
			items.AddRange(m_Database.itemGroups);
			UpdateReference(component, items);
		}

		private void UpdateReference(object source, List<INameable> destItems)
		{
			if (source == null || !string.IsNullOrEmpty(source.GetType().Namespace) && source.GetType().Namespace.Contains("UnityEngine"))
			{
				return;
			}
			
			FieldInfo[] fields = source.GetType().GetAllSerializedFields();
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo fieldInfo = fields[j];
				Debug.Log(fieldInfo.FieldType.FullName);
				if (!string.IsNullOrEmpty(fieldInfo.FieldType.Namespace) && fieldInfo.FieldType.Namespace.Contains("UnityEngine")) {
					continue;
				}

				if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
				{
					Type elementType = Utility.GetElementType(fieldInfo.FieldType);
					
					if (ShouldReference(elementType))
					{
						IList array = fieldInfo.GetValue(source) as IList;

						Type targetType = typeof(List<>).MakeGenericType(Utility.GetElementType(fieldInfo.FieldType));
						IList items = (IList)Activator.CreateInstance(targetType);
						for (int i = 0; i < array.Count; i++)
						{
							if (array[i] == null) {
								Debug.LogWarning("Item on source is null: "+source);
								continue;
							}
							INameable replacement = destItems.Find(x => x.Name == (array[i] as INameable).Name);
							if (replacement != null)
							{
								items.Add(replacement);
								if (array[i] != replacement)
								{
									m_ChangedReferences += 1;
								}
							}
						}
						if (fieldInfo.FieldType.IsArray)
						{
							Array arr = Array.CreateInstance(Utility.GetElementType(fieldInfo.FieldType), items.Count);
							items.CopyTo(arr, 0);
							items = arr;
						}

						fieldInfo.SetValue(source, items);
					}
					else
					{
						IList list = fieldInfo.GetValue(source) as IList;
						foreach (object o in list)
						{
							UpdateReference(o, destItems);
						}
					}
				}
				else if (ShouldReference(fieldInfo.FieldType))
				{
					INameable item = (INameable)fieldInfo.GetValue(source);
					if (item != null)
					{
						INameable replacement = destItems.Find(x => x.Name == item.Name);
						if (replacement != null)
						{
							if (item != replacement)
							{
								m_ChangedReferences += 1;
							}
							fieldInfo.SetValue(source, replacement);
						}
					}
				}
				else
				{
					object subSource = fieldInfo.GetValue(source);
					try
					{
						UpdateReference(subSource, destItems);
					}catch {
						Debug.LogWarning("This should not happen. If it happens please contact me with setup details.");
					};

				}
			}
		}

		private bool ShouldReference(Type type) {
			return typeof(Item).IsAssignableFrom(type) || 
				typeof(Category).IsAssignableFrom(type) || 
				typeof(Rarity).IsAssignableFrom(type) || 
				typeof(EquipmentRegion).IsAssignableFrom(type) || 
				typeof(ItemGroup).IsAssignableFrom(type);
		}

		private void SelectPrefabsFolder()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Prefabs", GUILayout.Width(70f));
			if (GUILayout.Button(string.IsNullOrEmpty(m_PrefabsPath) ? "Empty" : m_PrefabsPath, EditorStyles.objectField))
			{

				m_PrefabsPath = EditorUtility.OpenFolderPanel("Root Prfab Folder", Application.dataPath, "");
				m_PrefabsPath = m_PrefabsPath.Substring(m_PrefabsPath.IndexOf("Assets/"));
			}
			EditorGUILayout.EndHorizontal();
		}

		private void SelectDatabase()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Database", GUILayout.Width(70f));
			if (GUILayout.Button(m_Database != null ? m_Database.name : "Null", EditorStyles.objectField))
			{
				string searchString = "Search...";
				ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

				UtilityInstanceWindow.ShowWindow("Select Database", delegate ()
				{
					searchString = EditorTools.SearchField(searchString);

					for (int i = 0; i < databases.Length; i++)
					{
						if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("Search...") && !databases[i].name.Contains(searchString))
						{
							continue;
						}
						GUIStyle style = new GUIStyle("button");
						style.wordWrap = true;
						if (GUILayout.Button(AssetDatabase.GetAssetPath(databases[i]), style))
						{
							m_Database = databases[i];
							UtilityInstanceWindow.CloseWindow();
						}
					}
				});
			}
			GUILayout.EndHorizontal();

		}
		
		private void SceneSelection() {
		
			EditorGUILayout.LabelField("Scenes To Update", EditorStyles.boldLabel);

			Event evt = Event.current;
			Rect dropArea = GUILayoutUtility.GetRect(0.0f, 120.0f, GUILayout.ExpandWidth(true));
			GUI.Box(dropArea, GUIContent.none, (GUIStyle)"AvatarMappingBox");
			for (int i = 0; i < m_Scenes.Count; i++) {
				string assetPath = AssetDatabase.GetAssetPath(m_Scenes[i]);
				Rect rect = new Rect(dropArea.x+2f, dropArea.y + i * EditorGUIUtility.singleLineHeight, dropArea.width, EditorGUIUtility.singleLineHeight);
				bool state = EditorPrefs.GetBool("ItemReference.Scene." + assetPath, true);
				bool flag = GUI.Toggle(rect,state, assetPath);
				if (state != flag) {
					EditorPrefs.SetBool("ItemReference.Scene." + assetPath, flag);
				}

			}

			switch (evt.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!dropArea.Contains(evt.mousePosition))
						return;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					if (evt.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
						{
							if (draggedObject.GetType() == typeof(SceneAsset)) {
								m_Scenes.Add(draggedObject as SceneAsset);
							}
						}
					}
					break;
			}
	
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Open Scenes")) {
	
					for (int i = 0; i < SceneManager.sceneCount; i++)
					{
						Scene scene = SceneManager.GetSceneAt(i);
						SceneAsset sceneAsset=AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
						if (sceneAsset != null && !m_Scenes.Contains(sceneAsset)) {
							m_Scenes.Add(sceneAsset);
						}
					}
				
			}
			EditorGUILayout.EndHorizontal();
		}
    }
}