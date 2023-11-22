using UnityEditor;
using System.Collections.Generic;

namespace DevionGames.InventorySystem{
	[CustomPropertyDrawer(typeof(RarityPickerAttribute))]
	public class RarityPickerDrawer : PickerDrawer<Rarity> {

		protected override List<Rarity> GetItems(ItemDatabase database) {
			return database.raritys;
		}
	}
}