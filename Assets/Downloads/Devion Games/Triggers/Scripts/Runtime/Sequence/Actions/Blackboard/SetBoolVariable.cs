using UnityEngine;

namespace DevionGames
{
    [ComponentMenu("Blackboard/Set Bool Variable")]
    public class SetBoolVariable : Action
    {
        [SerializeField] private readonly string m_VariableName = "";
        [SerializeField] private readonly bool m_Value = true;

        public override ActionStatus OnUpdate()
        {
            blackboard.SetValue<bool>(m_VariableName, m_Value);
            return ActionStatus.Success;
        }
    }
}