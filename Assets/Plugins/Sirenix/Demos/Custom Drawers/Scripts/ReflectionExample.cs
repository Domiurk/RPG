#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using System.Reflection;
    using Utilities;
    using Sirenix.Utilities.Editor;

#endif

    [TypeInfoBox(
                    "This example demonstrates how reflection can be used to extend drawers from what otherwise would be possible.\n\n" +
                    "In this case, a user can specify one of their own methods to receive a callback from the drawer chain.\n\n" +
                    "Note that this is a manual approach; it is recommended to use ValueResolver<T> and ActionResolver instead.")]
    public class ReflectionExample : MonoBehaviour
    {
        [OnClickMethod("OnClick")]
        public int InstanceMethod;

        [OnClickMethod("StaticOnClick")]
        public int StaticMethod;

        [OnClickMethod("InvalidOnClick")]
        public int InvalidMethod;

        private void OnClick()
        {
            Debug.Log("Hello?");
        }

        private static void StaticOnClick()
        {
            Debug.Log("Static Hello?");
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OnClickMethodAttribute : Attribute
    {
        public string MethodName { get; private set; }

        public OnClickMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }

#if UNITY_EDITOR

    public class OnClickMethodAttributeDrawer : OdinAttributeDrawer<OnClickMethodAttribute>
    {
        private string ErrorMessage;

        private MethodInfo Method;

        protected override void Initialize()
        {
            Method = Property.ParentType.GetMethod(Attribute.MethodName, Flags.StaticInstanceAnyVisibility, null, Type.EmptyTypes, null);

            if (Method == null)
            {
                ErrorMessage = "Could not find a parameterless method named '" + Attribute.MethodName + "' in the type '" + Property.ParentType + "'.";
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(ErrorMessage);

                CallNextDrawer(label);
            }
            else
            {
                bool clicked = Event.current.rawType == EventType.MouseDown && Event.current.button == 0 && Property.LastDrawnValueRect.Contains(Event.current.mousePosition);

                if (clicked)
                {
                    if (Method.IsStatic)
                    {
                        Method.Invoke(null, null);
                    }
                    else
                    {
                        Method.Invoke(Property.ParentValues[0], null);
                    }
                }

                CallNextDrawer(label);

                if (clicked)
                {
                    Event.current.Use();
                }
            }
        }
    }

#endif
}
#endif
