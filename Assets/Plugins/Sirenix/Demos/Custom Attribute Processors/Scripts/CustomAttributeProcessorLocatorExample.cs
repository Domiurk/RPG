#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Editor;
    using UnityEngine;
    using UnityEditor;
    using Sirenix.Utilities.Editor;
    using Utilities;

    [TypeInfoBox("This example demonstrate how it possible to create a custom AttributeProcessorLocator to complete change how an entire PropertyTree resolves attributes, and therefore how all objects in the tree are displayed.")]
    public class CustomAttributeProcessorLocatorExample : MonoBehaviour
    {
        [Button(ButtonSizes.Large)]
        private void OpenEditorWindow()
        {
            var window = ScriptableObject.CreateInstance<SomeCustomEditorWindow>();
            window.Show();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 300);
        }
    }

    public class SomeCustomEditorWindow : EditorWindow
    {
        private PropertyTree defaultPropertyTree;
        private PropertyTree customPropertyTree;

        private void OnEnable()
        {
            wantsMouseMove = true;
        }

        private void OnGUI()
        {
            DrawWithDefaultLocator();
            DrawWithCustomLocator();

            this.RepaintIfRequested();
        }

        private void DrawWithDefaultLocator()
        {
            if (defaultPropertyTree == null)
            {
                defaultPropertyTree = PropertyTree.Create(new SomeClass());
            }

            SirenixEditorGUI.BeginBox("Default Locator");
            defaultPropertyTree.Draw(false);
            SirenixEditorGUI.EndBox();
        }

        private void DrawWithCustomLocator()
        {
            if (customPropertyTree == null)
            {
                customPropertyTree = PropertyTree.Create(new SomeClass());
                customPropertyTree.AttributeProcessorLocator = new CustomMinionAttributeProcessorLocator();
            }

            SirenixEditorGUI.BeginBox("Custom Locator");
            customPropertyTree.Draw(false);
            SirenixEditorGUI.EndBox();
        }
    }

    public class SomeClass
    {
        [HorizontalGroup("Split", LabelWidth = 80)]
        [BoxGroup("Split/$Name", showLabel: false)]
        [BoxGroup("Split/$Name/NameId", showLabel: false)]
        public string Name, Id;

        [HideLabel, PropertyOrder(5)]
        [PreviewField(Height = 105), HorizontalGroup("Split", width: 105)]
        public Texture2D Icon;

        [BoxGroup("Split/$Name/Properties", showLabel: false)]
        public int Health, Damage, Speed;
    }

    [OdinDontRegister]
    public class CustomMinionAttributeProcessor : OdinAttributeProcessor<SomeClass>
    {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            attributes.Clear();

            switch (member.Name)
            {
                case "Icon":
                    attributes.Add(new HorizontalGroupAttribute("Split", width: 70));
                    attributes.Add(new PreviewFieldAttribute(70, OdinInspector.ObjectFieldAlignment.Left));
                    attributes.Add(new PropertyOrderAttribute(-5));
                    attributes.Add(new HideLabelAttribute());
                    break;

                case "Name":
                case "Id":
                    attributes.Add(new BoxGroupAttribute("Split/$Name"));
                    attributes.Add(new VerticalGroupAttribute("Split/$Name/Vertical"));
                    attributes.Add(new HorizontalGroupAttribute("Split/$Name/Vertical/NameId"));
                    attributes.Add(new LabelWidthAttribute(40));
                    break;

                default:
                    attributes.Add(new FoldoutGroupAttribute("Split/$Name/Vertical/Properties", expanded: false));
                    attributes.Add(new LabelWidthAttribute(60));
                    break;
            }
        }
    }

    public class CustomMinionAttributeProcessorLocator : OdinAttributeProcessorLocator
    {
        private static readonly CustomMinionAttributeProcessor Processor = new();

        public override List<OdinAttributeProcessor> GetChildProcessors(InspectorProperty parentProperty, MemberInfo member)
        {
            return new List<OdinAttributeProcessor>() { Processor };
        }

        public override List<OdinAttributeProcessor> GetSelfProcessors(InspectorProperty property)
        {
            return new List<OdinAttributeProcessor>() { Processor };
        }
    }
}
#endif
