namespace DevionGames.StatSystem
{
    public enum StatModType
    {
        Flat,
        PercentAdd,
        PercentMult,
    }

    public class StatModifier
    {
        public readonly object source;

        private readonly float m_Value;
        public float Value
        {
            get
            {
                return m_Value;
            }
        }

        private readonly StatModType m_Type;
        public StatModType Type
        {
            get
            {
                return m_Type;
            }
        }

        public StatModifier() : this(0f, StatModType.Flat, null)
        {
        }

        public StatModifier(float value, StatModType type, object source)
        {
            m_Value = value;
            m_Type = type;
            this.source = source;
        }
    }
}