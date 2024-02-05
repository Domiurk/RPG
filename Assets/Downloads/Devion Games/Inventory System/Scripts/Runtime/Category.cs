using UnityEngine;

namespace DevionGames.InventorySystem{
	[System.Serializable]
	public class Category : ScriptableObject,INameable {
		[CategoryPicker(true)]
		[SerializeField]
		protected Category m_Parent;

		public Category Parent
        {
			get => m_Parent;
			set => m_Parent = value;
        }

		[SerializeField]
		private new string name="";
		public string Name{
			get => name;
			set => name = value;
		}


		[SerializeField]
		protected Color m_EditorColor = Color.clear;
		public Color EditorColor => m_EditorColor;

		[SerializeField]
        protected float m_Cooldown = 1f;
        public float Cooldown => m_Cooldown;

        public bool IsAssignable(Category other) {
			if (other == null)
				return false;

			if (Name == other.Name) {
				return true;
			}

			if (other.Parent != null) {
				return IsAssignable(other.Parent);
			}
			return false;
		}
	}
}