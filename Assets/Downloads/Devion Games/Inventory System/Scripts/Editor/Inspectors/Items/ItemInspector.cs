using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;
using UnityEditorInternal;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DevionGames.InventorySystem
{
	[CustomEditor (typeof(Item), true)]
	public class ItemInspector :  Editor
	{
        protected SerializedProperty m_ItemName;
        protected SerializedProperty m_UseItemNameAsDisplayName;
        protected AnimBool m_ShowItemDisplayNameOptions;
        protected SerializedProperty m_ItemDisplayName;
        protected SerializedProperty m_Icon;
        protected SerializedProperty m_Prefab;
        protected SerializedProperty m_Description;
        protected SerializedProperty m_Category;
        protected SerializedProperty m_BuyPrice;
        protected SerializedProperty m_BuyCurrency;
        protected SerializedProperty m_SellPrice;
        protected SerializedProperty m_SellCurrency;
        protected SerializedProperty m_Stack;
        protected SerializedProperty m_MaxStack;
        protected SerializedProperty m_IsDroppable;
        protected SerializedProperty m_DropSound;
        protected SerializedProperty m_OverridePrefab;
        protected SerializedProperty m_IsCraftable;
        protected SerializedProperty m_CraftingDuration;
        protected SerializedProperty m_CraftingAnimatorState;
        protected SerializedProperty m_Ingredients;
        protected SerializedProperty m_Properties;
        protected SerializedProperty m_IsSellable;
        protected SerializedProperty m_CanBuyBack;

        protected SerializedProperty m_UseCraftingSkill;
        protected SerializedProperty m_SkillWindow;
        protected SerializedProperty m_CraftingSkill;
        protected SerializedProperty m_MinCraftingSkillValue;
        protected SerializedProperty m_RemoveIngredientsWhenFailed;
        protected SerializedProperty m_CraftingModifier;
        protected ReorderableList m_CraftingModifierList;

        protected AnimBool m_ShowSellOptions;
        protected AnimBool m_ShowDropOptions;
        protected AnimBool m_ShowCraftOptions;
        protected AnimBool m_ShowSkillOptions;

        protected ReorderableList m_PropertyList;
        protected ReorderableList m_IngredientList;
        protected static readonly List<ObjectProperty> copy = new();


        protected SerializedProperty m_Script;

        private Dictionary<Type, string[]> m_ClassProperties;
        protected string[] m_PropertiesToExcludeForChildClasses;
        protected List<System.Action> m_DrawInspectors;
        protected string m_NameError;

        protected virtual void OnEnable ()
		{
            if (target == null)
                return;

            m_DrawInspectors = new List<System.Action>();
            m_ClassProperties = new Dictionary<Type, string[]>();

            m_Script = serializedObject.FindProperty("m_Script");
            m_ItemName = serializedObject.FindProperty("m_ItemName");
            m_ItemDisplayName = serializedObject.FindProperty("m_DisplayName");
            m_UseItemNameAsDisplayName = serializedObject.FindProperty("m_UseItemNameAsDisplayName");
            m_ShowItemDisplayNameOptions = new AnimBool(!m_UseItemNameAsDisplayName.boolValue);
            m_ShowItemDisplayNameOptions.valueChanged.AddListener(Repaint);

            m_Icon = serializedObject.FindProperty("m_Icon");
            m_Prefab = serializedObject.FindProperty("m_Prefab");
            m_Description = serializedObject.FindProperty("m_Description");

            m_Category = serializedObject.FindProperty("m_Category");

            #region BuySell
            m_IsSellable = serializedObject.FindProperty("m_IsSellable");
            m_ShowSellOptions = new AnimBool(m_IsSellable.boolValue);
            m_ShowSellOptions.valueChanged.AddListener(Repaint);
            m_CanBuyBack = serializedObject.FindProperty("m_CanBuyBack");
            m_BuyPrice = serializedObject.FindProperty("m_BuyPrice");
            m_BuyCurrency = serializedObject.FindProperty("m_BuyCurrency");
            m_SellPrice = serializedObject.FindProperty("m_SellPrice");
            m_SellCurrency = serializedObject.FindProperty("m_SellCurrency");
            #endregion

            m_Stack = serializedObject.FindProperty("m_Stack");
            m_MaxStack = serializedObject.FindProperty("m_MaxStack");

            #region Drop
            m_IsDroppable = serializedObject.FindProperty("m_IsDroppable");
            m_DropSound = serializedObject.FindProperty("m_DropSound");
            m_OverridePrefab = serializedObject.FindProperty("m_OverridePrefab");
            m_IsDroppable = serializedObject.FindProperty("m_IsDroppable");
            m_ShowDropOptions = new AnimBool(m_IsDroppable.boolValue);
            m_ShowDropOptions.valueChanged.AddListener(Repaint);
            #endregion

            m_Properties = serializedObject.FindProperty("properties");

            m_PropertyList = new ReorderableList (serializedObject,m_Properties, true, true, true, true);
			m_PropertyList.elementHeight = (EditorGUIUtility.singleLineHeight + 4f) * 3;
			m_PropertyList.drawHeaderCallback = (Rect rect) => {  
				EditorGUI.LabelField (rect, "Item Properties");
				Event ev = Event.current;
				if (ev.type == EventType.MouseDown && ev.button == 1 && rect.Contains (ev.mousePosition)) {
					GenericMenu menu = new GenericMenu ();
					menu.AddItem (new GUIContent ("Copy"), false, delegate {
						ObjectProperty[] properties = (target as Item).GetProperties ();
						foreach (ObjectProperty property in properties) {
							ObjectProperty clone = new ObjectProperty ();
							FieldInfo[] fields = typeof(ObjectProperty).GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							foreach (FieldInfo field in fields) {
								field.SetValue (clone, field.GetValue (property));
							}
							copy.Add (clone);
						}

					});
					if (copy != null && copy.Count > 0) {
						menu.AddItem (new GUIContent ("Paste"), false, delegate {
							(target as Item).SetProperties (copy.ToArray ());
						});
					} else {
						menu.AddDisabledItem (new GUIContent ("Paste"));
					}
					menu.ShowAsContext ();
				}
			};
			
			m_PropertyList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
				Rect mRect = new Rect (rect);
				SerializedProperty element = m_PropertyList.serializedProperty.GetArrayElementAtIndex (index);
				rect.y += 2;
				rect.height = EditorGUIUtility.singleLineHeight;
				rect.width -= 17;
				EditorGUI.PropertyField (rect, element.FindPropertyRelative ("name"));
				rect.x += rect.width + 2;
				rect.y -= 2f;
				EditorGUI.PropertyField (rect, element.FindPropertyRelative ("show"), GUIContent.none);
				rect.y += 2f;
				rect.x -= rect.width + 2;
				rect.width += 17;
				rect.y += EditorGUIUtility.singleLineHeight + 2;
				float width = rect.width;
				rect.width = EditorGUIUtility.labelWidth - 2f;
				SerializedProperty typeIndex = element.FindPropertyRelative ("typeIndex");
				typeIndex.intValue = EditorGUI.Popup (rect, typeIndex.intValue, ObjectProperty.DisplayNames);
				rect.x += rect.width + 2f;
				rect.width = width - EditorGUIUtility.labelWidth;
		
				EditorGUI.PropertyField (rect, element.FindPropertyRelative (ObjectProperty.GetPropertyName (ObjectProperty.SupportedTypes [typeIndex.intValue])), GUIContent.none);
				rect.y += EditorGUIUtility.singleLineHeight + 2;
				rect.x = mRect.x;
				rect.width = mRect.width;
				EditorGUI.PropertyField (rect, element.FindPropertyRelative ("color"));
			};


            #region Crafting
            m_IsCraftable = serializedObject.FindProperty("m_IsCraftable");
            m_CraftingDuration = serializedObject.FindProperty("m_CraftingDuration");
            m_CraftingAnimatorState = serializedObject.FindProperty("m_CraftingAnimatorState");
 
            m_ShowCraftOptions = new AnimBool(m_IsCraftable.boolValue);
            m_ShowCraftOptions.valueChanged.AddListener(Repaint);

            m_UseCraftingSkill = serializedObject.FindProperty("m_UseCraftingSkill");
            m_ShowSkillOptions = new AnimBool(m_UseCraftingSkill.boolValue);
            m_SkillWindow = serializedObject.FindProperty("m_SkillWindow");
            m_CraftingSkill = serializedObject.FindProperty("m_CraftingSkill");
            m_MinCraftingSkillValue = serializedObject.FindProperty("m_MinCraftingSkillValue");
            m_RemoveIngredientsWhenFailed = serializedObject.FindProperty("m_RemoveIngredientsWhenFailed");

            m_CraftingModifier = serializedObject.FindProperty("m_CraftingModifier");
            CreateModifierList("Crafting Item Modifers", serializedObject, m_CraftingModifier);

            m_Ingredients = serializedObject.FindProperty("ingredients");
            m_IngredientList = new ReorderableList(serializedObject, m_Ingredients, true, true, true, true);
            m_IngredientList.drawElementCallback = (Rect rect, int index, bool _, bool _) => {
                SerializedProperty element = m_IngredientList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty itemProperty = element.FindPropertyRelative("item");
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width -= 55f;
                EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
                rect.x += rect.width + 5;
                rect.width = 50f;
                SerializedProperty amount = element.FindPropertyRelative("amount");
                EditorGUI.PropertyField(rect, amount, GUIContent.none);
                amount.intValue = Mathf.Clamp(amount.intValue, 1, int.MaxValue);
            };
            m_IngredientList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Ingredients (Item, Amount)");
            };
            #endregion

            List<string> propertiesToExclude = new List<string>() {
                m_Script.propertyPath,
                m_ItemName.propertyPath,
                m_ItemDisplayName.propertyPath,
                m_UseItemNameAsDisplayName.propertyPath,
                m_Icon.propertyPath,
                m_Prefab.propertyPath,
                m_Description.propertyPath,
                m_Category.propertyPath,
                m_BuyPrice.propertyPath,
                m_BuyCurrency.propertyPath,
                m_SellPrice.propertyPath,
                m_SellCurrency.propertyPath,
                m_Stack.propertyPath,
                m_MaxStack.propertyPath,
                m_IsDroppable.propertyPath,
                m_DropSound.propertyPath,
                m_OverridePrefab.propertyPath,
                m_IsCraftable.propertyPath,
                m_CraftingDuration.propertyPath,
                m_CraftingAnimatorState.propertyPath,
                m_Ingredients.propertyPath,
                m_Properties.propertyPath,
                m_IsSellable.propertyPath,
                m_CraftingModifier.propertyPath,
                m_CraftingSkill.propertyPath,
                m_UseCraftingSkill.propertyPath,
                m_SkillWindow.propertyPath,
                m_RemoveIngredientsWhenFailed.propertyPath,
                m_MinCraftingSkillValue.propertyPath,
                m_CanBuyBack.propertyPath,
            };


            Type[] subInspectors = Utility.BaseTypesAndSelf(GetType()).Where(x => x.IsSubclassOf(typeof(ItemInspector))).ToArray();
            Array.Reverse(subInspectors);
            for (int i = 0; i < subInspectors.Length; i++)
            {
                MethodInfo method = subInspectors[i].GetMethod("DrawInspector", BindingFlags.NonPublic | BindingFlags.Instance);
                Type inspectedType = typeof(CustomEditor).GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(subInspectors[i].GetCustomAttribute<CustomEditor>()) as Type;
                FieldInfo[] fields = inspectedType.GetAllSerializedFields().Where(x => !x.HasAttribute(typeof(HideInInspector))).ToArray();
                string[] classProperties = fields.Where(x => x.DeclaringType == inspectedType).Select(x => x.Name).ToArray();
                if (!m_ClassProperties.ContainsKey(inspectedType))
                {
                    m_ClassProperties.Add(inspectedType, classProperties);
                }
                propertiesToExclude.AddRange(classProperties);
                if (method != null)
                {
                    m_DrawInspectors.Add(delegate { method.Invoke(this, null); });
                }
                else
                {
                    m_DrawInspectors.Add(delegate () {
                        for (int j = 0; j < classProperties.Length; j++)
                        {
                            SerializedProperty property = serializedObject.FindProperty(classProperties[j]);
                            EditorGUILayout.PropertyField(property);
                        }

                    });
                }
            }
            
            m_PropertiesToExcludeForChildClasses = propertiesToExclude.ToArray();
        }

        protected virtual void OnDisable() { }

        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();
            DrawBaseInspector();
            for (int i = 0; i < m_DrawInspectors.Count; i++)
            {
               m_DrawInspectors[i].Invoke();
            }
            DrawPropertiesExcluding(serializedObject, m_PropertiesToExcludeForChildClasses);
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawBaseInspector() {
            EditorGUILayout.PropertyField(m_ItemName, new GUIContent("Name"));

            EditorGUILayout.PropertyField(m_UseItemNameAsDisplayName, new GUIContent("Use name as display name"));
            m_ShowItemDisplayNameOptions.target = !m_UseItemNameAsDisplayName.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowItemDisplayNameOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_ItemDisplayName);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Icon);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_Prefab);
            SetupPrefab(m_Prefab);
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(m_Description);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Properties:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Properties can be used to define item specific information like stats or any custom information you want to change and save at runtime.", MessageType.Info);
            m_PropertyList.elementHeight = m_PropertyList.count == 0 ? (EditorGUIUtility.singleLineHeight + 4f) : (EditorGUIUtility.singleLineHeight + 4f) * 3;
            m_PropertyList.DoLayoutList();

            EditorGUILayout.PropertyField(m_Category);


            EditorGUILayout.PropertyField(m_IsSellable);
            m_ShowSellOptions.target = m_IsSellable.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowSellOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_CanBuyBack);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_BuyPrice);
                EditorGUILayout.PropertyField(m_BuyCurrency, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_SellPrice);
                EditorGUILayout.PropertyField(m_SellCurrency, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Stack);
            if (m_MaxStack.intValue == 0)
            {
                EditorGUILayout.HelpBox("Maximum stack of 0 ~ unlimited", MessageType.Info);
            }
            EditorGUILayout.PropertyField(m_MaxStack);


            EditorGUILayout.PropertyField(m_IsDroppable);
            m_ShowDropOptions.target = m_IsDroppable.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowDropOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_OverridePrefab);
                EditorGUILayout.PropertyField(m_DropSound);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_IsCraftable);
            m_ShowCraftOptions.target = m_IsCraftable.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowCraftOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(m_CraftingDuration);
                EditorGUILayout.PropertyField(m_CraftingAnimatorState);

                EditorGUILayout.PropertyField(m_UseCraftingSkill);
                m_ShowSkillOptions.target = m_UseCraftingSkill.boolValue;
                if (EditorGUILayout.BeginFadeGroup(m_ShowSkillOptions.faded))
                {
                    EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                    EditorGUILayout.PropertyField(m_SkillWindow);
                    EditorGUILayout.PropertyField(m_CraftingSkill, new GUIContent("Skill"));
                    EditorGUILayout.PropertyField(m_MinCraftingSkillValue, new GUIContent("Min Skill Value"));
                    EditorGUILayout.PropertyField(m_RemoveIngredientsWhenFailed);
                    EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
                GUILayout.Space(3f);
                GUILayout.BeginHorizontal();
                GUILayout.Space(16f);
                GUILayout.BeginVertical();
                EditorGUILayout.HelpBox("Crafting item modifiers can be used to randomize the item when crafting.", MessageType.Info);
                m_CraftingModifierList.DoLayoutList();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Required ingredients to craft this item.", MessageType.Info);
                m_IngredientList.DoLayoutList();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
        }


        protected virtual void DrawBuySellGUI() {
            EditorGUILayout.PropertyField(m_IsSellable);
            m_ShowSellOptions.target = m_IsSellable.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_ShowSellOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_BuyPrice);
                EditorGUILayout.PropertyField(m_BuyCurrency, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(m_SellPrice);
                EditorGUILayout.PropertyField(m_SellCurrency, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void CreateModifierList(string title, SerializedObject serializedObject, SerializedProperty property)
        {

            m_CraftingModifierList = new ReorderableList(serializedObject, property.FindPropertyRelative("modifiers"), true, true, true, true);
            m_CraftingModifierList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, title);
            };
            m_CraftingModifierList.drawElementCallback = (Rect rect, int index, bool _, bool _) =>
            {
                float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y = rect.y + verticalOffset;
                SerializedProperty element = m_CraftingModifierList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none, true);
            };

            m_CraftingModifierList.onRemoveCallback = (ReorderableList list) =>
            {
                list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            };
        }

        protected void ScriptGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();
        }

        private void SetupPrefab(SerializedProperty prefabProperty)
        {
            if (prefabProperty.objectReferenceValue != null)
            {
                GameObject mPrefab = prefabProperty.objectReferenceValue as GameObject;
                if (mPrefab.GetComponent<Trigger>() == null ||
                   mPrefab.GetComponent<Collider>() == null ||
                   mPrefab.GetComponent<Rigidbody>() == null)
                {
                    Color color = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Setup", GUILayout.Width(70)))
                    {
                        GameObject prefab = (GameObject)Instantiate(mPrefab);
                        if (prefab.GetComponent<Trigger>() == null)
                        {
                            Trigger trigger=prefab.AddComponent<Trigger>();
                        }

                        if (prefab.GetComponent<ItemCollection>() == null) {
                            ItemCollection collection=prefab.AddComponent<ItemCollection>();
                            collection.Add((Item)target);
                        }

                        if (prefab.GetComponent<Collider>() == null)
                        {
                            MeshCollider collider = prefab.AddComponent<MeshCollider>();
                            collider.convex = true;
                        }
                        if (prefab.GetComponent<Rigidbody>() == null)
                        {
                            prefab.AddComponent<Rigidbody>();
                        }
#if PUN
						if (prefab.GetComponent<PhotonView> () == null) {
							prefab.AddComponent<PhotonView> ();
						}
#endif

                        string mPath = EditorUtility.SaveFilePanelInProject(
                                           "Create Prefab" + prefab.name,
                                           "New " + prefab.name + ".prefab",
                                           "prefab", "");
                        if (!string.IsNullOrEmpty(mPath))
                        {
                            GameObject mGameObject = PrefabUtility.SaveAsPrefabAsset(prefab, mPath);
                            AssetDatabase.SaveAssets();
                            prefabProperty.objectReferenceValue = mGameObject;
                        }
                        DestroyImmediate(prefab);
                    }
                    GUI.backgroundColor = color;
                }
            }
        }
    }
}