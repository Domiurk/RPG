using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class StatEffect : ScriptableObject, INameable
    {
        [InspectorLabel("Name")]
        [SerializeField]
        protected string m_StatEffectName="New Effect";
        public string Name { get => m_StatEffectName; set => m_StatEffectName = value; }
        [SerializeField]
        protected int m_Repeat = -1;

        [SerializeReference]
        protected List<Action> m_Actions = new();

        protected Sequence m_Sequence;
        [System.NonSerialized]
        protected int m_CurrentRepeat;
        protected StatsHandler m_Handler;

        public void Initialize(StatsHandler handler)
        {
            m_Handler = handler;
            m_Sequence = new Sequence(handler.gameObject, new PlayerInfo("Player"), handler.GetComponent<Blackboard>(), m_Actions.ToArray());
            m_Sequence.Start();
           
        }

        public void Execute() {
            if (!m_Sequence.Tick()) {
                m_Sequence.Stop();
                m_Sequence.Start();
                m_CurrentRepeat += 1;
            }
            m_Sequence.Update();

            if (m_Repeat > 0 && m_CurrentRepeat >= m_Repeat)
               m_Handler.RemoveEffect(this);
            
        }
    }
}