using UnityEngine;

namespace DevionGames.StatSystem
{
    [System.Serializable]
    public class Level : Stat
    {
        [StatPicker]
        [SerializeField]
        protected Attribute m_Experience;

        public override void Initialize(StatsHandler handler, StatOverride statOverride)
        {
            base.Initialize(handler, statOverride);
            m_Experience = handler.GetStat(m_Experience.Name) as Attribute;
            m_Experience.onCurrentValueChange += () =>
            {
                if (m_Experience.CurrentValue >= m_Experience.Value)
                {
                    m_Experience.CurrentValue = 0f;
                    Add(1f);
                }
            };
        }
    }
}