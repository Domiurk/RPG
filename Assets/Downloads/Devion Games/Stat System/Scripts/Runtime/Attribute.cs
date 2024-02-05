using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    public class Attribute : Stat
    {
        public System.Action onCurrentValueChange;

        [SerializeField]
        [Range(0f, 1f)]
        protected float m_StartCurrentValue = 1f;

        protected float m_CurrentValue;
        public float CurrentValue
        {
            get => m_CurrentValue;
            set
            {
                float single = Mathf.Clamp(value, 0, Value);
                if (m_CurrentValue != single)
                {
                    m_CurrentValue = single;
                    onCurrentValueChange?.Invoke();
                }
            }
        }

        public override void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            base.Initialize(handler, statOverride);
            onValueChange += () =>
            {
                CurrentValue = Mathf.Clamp(CurrentValue, 0f, Value);
            };
        }

        public override void ApplyStartValues()
        {
            base.ApplyStartValues();
            m_CurrentValue = m_Value * m_StartCurrentValue;
        }

        public override string ToString()
        {
            return m_StatName + ": " + CurrentValue.ToString() + "/" + Value.ToString();
        }

        public override void GetObjectData(Dictionary<string, object> data)
        {
            base.GetObjectData(data);
            data.Add("CurrentValue", m_CurrentValue);
        }

        public override void SetObjectData(Dictionary<string, object> data)
        {
            base.SetObjectData(data);
            m_CurrentValue = System.Convert.ToSingle(data["CurrentValue"]);
            CalculateValue(false);
        }
    }
}