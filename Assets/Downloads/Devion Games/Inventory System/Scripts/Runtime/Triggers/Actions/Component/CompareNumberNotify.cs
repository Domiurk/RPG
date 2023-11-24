using DevionGames.UIWidgets;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon("Component")]
    [ComponentMenu("Component/Compare Notify")]
    public class CompareNumberNotify : Action, ICondition
    {
        [SerializeField]
        private readonly TargetType m_Target = TargetType.Player;
        [Tooltip("The component to invoke the method on")]
        [SerializeField]
        private readonly string m_ComponentName = string.Empty;
        [Tooltip("The name of the field")]
        [SerializeField]
        private readonly string m_MethodName = string.Empty;
        [Tooltip("Arguments for method.")]
        [SerializeField]
        private readonly List<ArgumentVariable> m_Arguments = new();
        [SerializeField]
        private readonly Condition m_Condition = Condition.Greater;
        [SerializeField]
        private readonly float m_Number = 0f;
        [SerializeField]
        private readonly NotificationOptions m_FailureNotification = null;

        private GameObject m_TargetObject;

        public override void OnStart()
        {
            m_TargetObject = GetTarget(m_Target);
        }

        public override ActionStatus OnUpdate()
        {
            Type type =Utility.GetType(m_ComponentName);
            if (type == null)
            {
                Debug.LogWarning("Unable to invoke - type is null");
                return ActionStatus.Failure;
            }

            Component component = m_TargetObject.GetComponent(type);
            if (component == null)
            {
                Debug.LogWarning("Unable to invoke with component " + m_ComponentName);
                return ActionStatus.Failure;
            }

            List<object> parameterList = new List<object>();
            List<Type> typeList = new List<Type>();
            for (int i = 0; i < m_Arguments.Count; i++)
            {
                ArgumentVariable argument = m_Arguments[i];
                if (!argument.IsNone)
                {
                    object value = argument.GetValue();
                    parameterList.Add(value);
                    typeList.Add(value.GetType());
                }
            }

            MethodInfo methodInfo = component.GetType().GetMethod(m_MethodName, typeList.ToArray());

            if (methodInfo == null)
            {
                Debug.LogWarning("Unable to invoke method " + m_MethodName + " on component " + m_ComponentName);
                return ActionStatus.Failure;
            }


            float result = Convert.ToSingle(methodInfo.Invoke(component, parameterList.ToArray()));
            if (m_Condition == Condition.Greater && result < m_Number || m_Condition == Condition.Less && result > m_Number) {
                m_FailureNotification.Show();
                return ActionStatus.Failure;
            }
            return ActionStatus.Success;
        }

       private enum Condition { 
            Greater,
            Less,
       }
    }
}