using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DevionGames
{

    [ComponentMenu("Blackboard/Invoke With Variable")]
    public class InvokeWithVariable : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [Tooltip("The component to invoke the method on")]
        [SerializeField]        private readonly string m_ComponentName = string.Empty;
        [Tooltip("The name of the field")]
        [SerializeField] private readonly string m_MethodName = string.Empty;
        [SerializeField] private readonly List<string> m_VariableArguments=new();

        private GameObject m_TargetObject;

        public override void OnStart()
        {
            m_TargetObject = GetTarget(m_Target);
        }

        public override ActionStatus OnUpdate()
        {

            Type type = Utility.GetType(m_ComponentName);
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
            for (int i = 0; i < m_VariableArguments.Count; i++)
            {
                string argument = m_VariableArguments[i];

                object value = blackboard.GetVariable(argument).RawValue;
                parameterList.Add(value);
                typeList.Add(value.GetType());

            }

            MethodInfo methodInfo = component.GetType().GetMethod(m_MethodName, typeList.ToArray());

            if (methodInfo == null)
            {
                Debug.LogWarning("Unable to invoke method " + m_MethodName + " on component " + m_ComponentName);
                return ActionStatus.Failure;
            }
            bool? result = methodInfo.Invoke(component, parameterList.ToArray()) as bool?;
            if (result != null && !(bool)result)
            {
                return ActionStatus.Failure;
            }
            return ActionStatus.Success;
        }

    }
}
