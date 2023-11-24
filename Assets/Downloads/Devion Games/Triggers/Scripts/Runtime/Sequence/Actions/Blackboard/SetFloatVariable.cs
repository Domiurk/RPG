using UnityEngine;

namespace DevionGames
{
    [ComponentMenu("Blackboard/Set Float Variable")]
    public class SetFloatVariable : Action
    {
        [SerializeField] private readonly string m_VariableName = "Energy";
        [SerializeField] private readonly float m_Value = 0f;
        [SerializeField] private readonly float m_DampTime = 1f;

        public override ActionStatus OnUpdate()
        {
            if(m_DampTime > 0f)
                return ActionStatus.Success;

            blackboard.SetValue<float>(m_VariableName, m_Value);
            return ActionStatus.Success;
        }

        public override void Update()
        {
            if(m_DampTime <= 0f)
                return;

            float current = blackboard.GetValue<float>(m_VariableName);
            current = Mathf.Lerp(current, m_Value, Time.deltaTime * m_DampTime);
            blackboard.SetValue<float>(m_VariableName, current);
        }
    }
}