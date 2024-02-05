using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public class StatCallback
    {
        [SerializeField]
        protected ValueType m_ValueType = ValueType.CurrentValue;
        [SerializeField]
        protected ConditionType m_Condition;
        [SerializeField]
        protected float m_Value;
        [SerializeField]
        protected Actions m_Actions;

        protected Stat m_Stat;
        protected StatsHandler m_Handler;
        protected Sequence m_Sequence;

        public virtual void Initialize(StatsHandler handler, Stat stat) {
            m_Handler = handler;
            m_Stat = stat;
            switch (m_ValueType)
            {
                case ValueType.Value:
                    stat.onValueChange += OnValueChange;
                    break;
                case ValueType.CurrentValue:
                    if (stat is Attribute attribute)
                    {
                        attribute.onCurrentValueChange += OnCurrentValueChange;
                    }
                    break;
            }
            m_Sequence = new Sequence(handler.gameObject, new PlayerInfo("Player"),handler.GetComponent<Blackboard>(), m_Actions.actions.ToArray());
            m_Handler.onUpdate += Update;
        }

        private void Update() {
            if (m_Sequence != null)
            {
                m_Sequence.Tick();
            }
        }

        private void OnValueChange()
        {
            if (TriggerCallback(m_Stat.Value))
            {
                m_Sequence.Start();
            }
        }

        private void OnCurrentValueChange()
        {
            if (TriggerCallback((m_Stat as Attribute).CurrentValue))
            {
                m_Sequence.Start();
            }
        }



        private bool TriggerCallback(float value)
        {
            return m_Condition switch{
                ConditionType.Greater => value > m_Value,
                ConditionType.GreaterOrEqual => value >= m_Value,
                ConditionType.Less => value < m_Value,
                ConditionType.LessOrEqual => value <= m_Value,
                _ => false
            };
        }

    }

    public enum ValueType
    {
        Value, CurrentValue
    }

    public enum ConditionType
    {
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
    }
}