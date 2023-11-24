using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Compare")]
    [System.Serializable]
    public class Compare : Action, ICondition
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName = "Health";

        [InspectorLabel("Type")]
        [SerializeField]
        protected ValueType m_ValueType = ValueType.CurrentValue;

        [SerializeField]
        protected ConditionType m_Condition = ConditionType.Greater;

        [SerializeField]
        protected float m_Value;

        private StatsHandler m_Handler;

        public override void OnStart()
        {
            m_Handler = m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            Stat stat = m_Handler.GetStat(m_StatName) as Stat;
            if (stat == null) return ActionStatus.Failure;

            float value = stat.Value;
            if (m_ValueType == ValueType.CurrentValue)
                value = (stat as Attribute).CurrentValue;

            return m_Condition switch{
                ConditionType.Greater => value > m_Value ? ActionStatus.Success : ActionStatus.Failure,
                ConditionType.GreaterOrEqual => value >= m_Value ? ActionStatus.Success : ActionStatus.Failure,
                ConditionType.Less => value < m_Value ? ActionStatus.Success : ActionStatus.Failure,
                ConditionType.LessOrEqual => value <= m_Value ? ActionStatus.Success : ActionStatus.Failure,
                _ => ActionStatus.Failure
            };
        }
    }
}