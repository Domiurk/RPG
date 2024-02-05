using UnityEngine;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class Rarity : ScriptableObject, INameable {
		[SerializeField]
		private new string name="";
		public string Name{
			get => name;
			set => name = value;
		}

		[SerializeField]
		private bool m_UseAsNamePrefix;
		public bool UseAsNamePrefix => m_UseAsNamePrefix;

		[SerializeField]
		private Color color=Color.white;
		public Color Color{
			get => color;
			set => color = value;
		}

		[SerializeField]
		private int chance = 100;
		public int Chance
		{
			get => chance;
			set => chance = value;
		}

		[InspectorLabel("Property Multiplier")]
		[SerializeField]
		private float multiplier = 1.0f;
		public float Multiplier
		{
			get => multiplier;
			set => multiplier = value;
		}

		[InspectorLabel("Price Multiplier")]
		[SerializeField]
		private float m_PriceMultiplier = 1.0f;
		public float PriceMultiplier
		{
			get => m_PriceMultiplier;
			set => m_PriceMultiplier = value;
		}
	}
}