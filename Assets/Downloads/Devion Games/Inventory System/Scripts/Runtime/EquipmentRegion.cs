using UnityEngine;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class EquipmentRegion : ScriptableObject, INameable{
		[SerializeField]
		private new string name="";
		public string Name{
			get => this.name;
			set => this.name = value;
		}
	}
}