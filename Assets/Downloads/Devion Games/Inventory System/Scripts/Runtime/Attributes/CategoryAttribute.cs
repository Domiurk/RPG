﻿using System;

namespace DevionGames.InventorySystem{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CategoryAttribute : Attribute
	{
		private readonly string category;
		
		public string Category => category;

		public CategoryAttribute(string category)
		{
			this.category = category;
		}
	}
}