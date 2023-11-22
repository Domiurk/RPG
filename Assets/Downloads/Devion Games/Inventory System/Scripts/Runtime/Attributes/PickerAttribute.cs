﻿using UnityEngine;

namespace DevionGames.InventorySystem{
	public class PickerAttribute : PropertyAttribute {
		public bool utility;

		public PickerAttribute():this(false){}
		
		public PickerAttribute(bool utility){
			this.utility = utility; 
		}
	}
}