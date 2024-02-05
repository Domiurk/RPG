#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using Sirenix.Utilities.Editor;

#endif

    [InfoBox("As of Odin 2.0, all drawers are now instanced per property. This means that the previous context system is now unnecessary as you can just make fields directly in the drawer.")]
    public class InstancedDrawerExample : MonoBehaviour
    {
        [InstancedDrawerExample]
        public int Field;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InstancedDrawerExampleAttribute : Attribute
    { }

#if UNITY_EDITOR

    public class InstancedDrawerExampleAttributeDrawer : OdinAttributeDrawer<InstancedDrawerExampleAttribute>
    {
        private int counter;
        private bool counterEnabled;

        protected override void Initialize()
        {
            counter = 0;
            counterEnabled = false;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Event.current.type == EventType.Layout && counterEnabled)
            {
                counter++;
                GUIHelper.RequestRepaint();
            }

            SirenixEditorGUI.BeginBox();
            {
                GUILayout.Label("Frame Count: " + counter);

                if (GUILayout.Button(counterEnabled ? "Stop" : "Start"))
                {
                    counterEnabled = !counterEnabled;
                }
            }
            SirenixEditorGUI.EndBox();

            CallNextDrawer(label);
        }
    }

#endif
}
#endif
