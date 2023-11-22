using UnityEngine;

namespace DevionGames
{
	[System.Serializable]
	public class GameObjectVariable : Variable
	{
		[SerializeField]
		private GameObject m_Value = null;

		public GameObject Value {
			get => this.m_Value;
			set => this.m_Value = value;
		}

		public override object RawValue {
			get => this.m_Value;
			set => this.m_Value = (GameObject)value;
		}

		public override System.Type type => typeof(GameObject);

		public GameObjectVariable ()
		{
		}

		public GameObjectVariable (string name) : base (name)
		{
		}

		public static implicit operator GameObjectVariable(GameObject value)
		{
			return new GameObjectVariable()
			{
				Value = value
			};
		}

		public static implicit operator GameObject(GameObjectVariable value)
		{
			return value.Value;
		}
	}
}