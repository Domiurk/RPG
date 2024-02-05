using UnityEngine;

namespace DevionGames
{
    [ComponentMenu("Blackboard/Delete Variable")]
    public class DeleteVariable : Action
    {
        [SerializeField] private readonly string m_VariableName = "";

        public override ActionStatus OnUpdate()
        {
            blackboard.DeleteVariable(m_VariableName);
            return ActionStatus.Success;
        }
    }
}