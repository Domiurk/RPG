using System;
using System.Collections;

namespace DevionGames
{
    [Serializable]
    public class ArrayListVariable : Variable
    {
        private ArrayList m_Value = new();

        public ArrayList Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override object RawValue
        {
            get => m_Value ??= new ArrayList();
            set => m_Value = (ArrayList)value;
        }

        public override Type type => typeof(ArrayList);

        public ArrayListVariable() { }

        public ArrayListVariable(string name) : base(name) { }

        public static implicit operator ArrayListVariable(ArrayList value)
        {
            return new ArrayListVariable{
                Value = value
            };
        }

        public static implicit operator ArrayList(ArrayListVariable value)
        {
            return value.Value;
        }
    }
}