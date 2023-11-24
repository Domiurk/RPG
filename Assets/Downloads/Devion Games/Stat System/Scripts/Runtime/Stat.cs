using DevionGames.Graphs;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class Stat : ScriptableObject, INameable, IGraphProvider, IJsonSerializable
    {
        public System.Action onValueChange;
        private System.Action onValueChangeInternal;

        [InspectorLabel("Name")]
        [SerializeField]
        protected string m_StatName = "New Stat";
        public string Name { get => m_StatName; set => m_StatName=value; }

        [SerializeField]
        protected float m_BaseValue;

        [SerializeField]
        protected FormulaGraph m_FormulaGraph;
        [SerializeField]
        protected float m_Cap = -1;
        [SerializeReference]
        protected List<StatCallback> m_Callbacks = new();

        [System.NonSerialized]
        protected float m_Value;
        public float Value { get => m_Value; }

        protected List<StatModifier> m_StatModifiers= new();
        protected StatsHandler m_StatsHandler;

        public virtual void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            m_StatsHandler = handler;
            
            if (statOverride.overrideBaseValue)
                m_BaseValue = statOverride.baseValue;

            List<StatNode> statNodes = m_FormulaGraph.FindNodesOfType<StatNode>();

            for (int i = 0; i < statNodes.Count; i++)
            {
                Stat referencedStat = handler.GetStat(statNodes[i].stat.Trim());
                statNodes[i].statValue = referencedStat;
                referencedStat.onValueChangeInternal += CalculateValue;
            }
        
            for (int i = 0; i < m_Callbacks.Count; i++)
            {
                m_Callbacks[i].Initialize(handler, this);
            }
        }

        public virtual void ApplyStartValues() {
            CalculateValue();
        }

        public void Add(float amount) {
            m_BaseValue += amount;
            m_BaseValue = Mathf.Clamp(m_BaseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void Subtract(float amount) {
            m_BaseValue -= amount;
            m_BaseValue = Mathf.Clamp(m_BaseValue, 0, float.MaxValue);
            CalculateValue();
        }

        public void CalculateValue() {
            CalculateValue(true);
        }

        public void CalculateValue(bool invokeCallbacks) {
            float finalValue = m_BaseValue + m_FormulaGraph;
            float sumPercentAdd = 0f;
            m_StatModifiers.Sort((x, y) => x.Type.CompareTo(y.Type));

            for (int i = 0; i < m_StatModifiers.Count; i++)
            {
                StatModifier mod = m_StatModifiers[i];
                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;

                    if (i + 1 >= m_StatModifiers.Count || m_StatModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1f + sumPercentAdd;
                        sumPercentAdd = 0f;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1f + mod.Value;
                }
            }
            if (m_Cap >= 0)
                finalValue = Mathf.Clamp(finalValue, 0, m_Cap);



            if (m_Value != finalValue)
            {
                m_Value = finalValue;
                if(invokeCallbacks)
                    onValueChange?.Invoke();

                onValueChangeInternal?.Invoke();
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            m_StatModifiers.Add(modifier);
            CalculateValue();
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (m_StatModifiers.Remove(modifier))
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public bool RemoveModifiersFromSource(object source)
        {
            int numRemovals = m_StatModifiers.RemoveAll(mod => mod.source == source);

            if (numRemovals > 0)
            {
                CalculateValue();
                return true;
            }
            return false;
        }

        public Graph GetGraph()
        {
            return m_FormulaGraph;
        }

        public override string ToString()
        {
            return m_StatName+": "+Value.ToString();
        }

        public virtual void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", m_StatName);
            data.Add("BaseValue", m_BaseValue);
        }

        public virtual void SetObjectData(Dictionary<string, object> data)
        {
            m_BaseValue = System.Convert.ToSingle(data["BaseValue"]);
            CalculateValue(false);
        }
    }

    [System.Serializable]
    public class StatOverride
    {
        public bool overrideBaseValue;
        public float baseValue;

    }
}