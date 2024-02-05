using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
	[Serializable]
	public class Item : ScriptableObject, INameable, IJsonSerializable
	{
		[SerializeField]
		[HideInInspector]
		private string m_Id;

		public string Id {
			get => m_Id;
            set => m_Id = value;
        }

        [Tooltip("Unique name of the item. It can be used to display the item name in UI.")]
		[SerializeField]
		private string m_ItemName = "New Item";

		public string Name {
			get => m_ItemName;
            set => m_ItemName = value;
        }

        [Tooltip("If set to true the Name setting will be used to display the items name in UI.")]
        [SerializeField]
        private bool m_UseItemNameAsDisplayName = true;
        [Tooltip("Items name to display in UI.")]
        [SerializeField]
        private string m_DisplayName = "New Item";

        public string DisplayName
        {
            get {
                string displayName = m_UseItemNameAsDisplayName ? m_ItemName : m_DisplayName;
                if (Rarity.UseAsNamePrefix)
                    displayName = Rarity.Name + " " + displayName;
                return displayName; 
            
            }
            set => m_DisplayName = value;
        }

        [Tooltip("The icon that can be shown in various places of the UI. Tooltip, vendor and many more. ")]
        [SerializeField]
		private Sprite m_Icon;

		public Sprite Icon {
			get => m_Icon;
            set => m_Icon = value;
        }

        [Tooltip("The prefab to instantiate when an item is draged out of a container. This prefab is also used to place the item in scene, so the player can pickup the item.")]
		[SerializeField]
		private GameObject m_Prefab;

		public GameObject Prefab {
			get => m_Prefab;
            set => m_Prefab = value;
        }

        [Tooltip("Item description is used in the UI. Tooltip, vendor, spells, crafting...")]
		[SerializeField]
		[Multiline (4)]
		private string m_Description = string.Empty;

		public string Description => m_Description;

        [Tooltip("The category the item belongs to. Used to sort the items collection in editor or also at runtime in the UI.")]
		[Header ("Behaviour:")]
        [SerializeField]
        [CategoryPicker]
        private Category m_Category;

        public Category Category
        {
            get => m_Category;
            set => m_Category = value;
        }

        private static Rarity m_DefaultRarity;
        private static Rarity DefaultRarity {
            get {
                if (m_DefaultRarity is null) {
                    m_DefaultRarity = CreateInstance<Rarity>();
                    m_DefaultRarity.Name = "None";
                    m_DefaultRarity.Color = Color.grey;
                    m_DefaultRarity.Chance = 100;
                    m_DefaultRarity.Multiplier = 1.0f;
                }
                return m_DefaultRarity;
             }
        }

        private Rarity m_Rarity;
		public Rarity Rarity {
			get{
                if (m_Rarity == null ) {
                    m_Rarity = DefaultRarity;
                }
                return m_Rarity; 
            }
            set => m_Rarity = value;
        }

        [Tooltip("Is this item sellable to a vendor? More options will appear if it is sellable.")]
        [SerializeField]
        private bool m_IsSellable = true;
        public bool IsSellable {
            get => m_IsSellable;
            set => m_IsSellable = true;
        }

        [Tooltip("Items buy price. This value will be multiplied with the rarities price multiplier.")]
		[SerializeField]
		private int m_BuyPrice;

		public int BuyPrice => Mathf.RoundToInt(m_BuyPrice*Rarity.PriceMultiplier);
        [Tooltip("If set to true, this item will be added to the vendors inventory and the player can buy it back.")]
        [SerializeField]
        private bool m_CanBuyBack = true;
        public bool CanBuyBack => m_CanBuyBack;

        [Tooltip("The buy currency. You can also use a lower currency, it will be auto converted. 120 Copper will be converted to 1 Silver and 20 Copper.")]
        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_BuyCurrency;

        public Currency BuyCurrency
        {
            get => m_BuyCurrency;
            set => m_BuyCurrency = value;
        }
        [Tooltip("Items sell price. This value will be multiplied with the rarities price multiplier.")]
        [SerializeField]
		private int m_SellPrice;

		public int SellPrice => Mathf.RoundToInt(m_SellPrice*Rarity.PriceMultiplier);

        [Tooltip("The sell currency. You can also use a lower currency, it will be auto converted. 120 Copper will be converted to 1 Silver and 20 Copper.")]
        [CurrencyPicker(true)]
        [SerializeField]
        private Currency m_SellCurrency;

        public Currency SellCurrency
        {
            get => m_SellCurrency;
            set => m_SellCurrency = value;
        }

        [Tooltip("Items stack definition. New created items will have this stack. Use a stack modifier to randomize the stack.")]
        [SerializeField]
        [Range(1, 100)]
        private int m_Stack = 1;

        public virtual int Stack
        {
            get => m_Stack;
            set
            {
                m_Stack = value;
                if (Slot != null){
                    if (m_Stack <= 0 && !typeof(Currency).IsAssignableFrom(GetType())){
                        ItemContainer.RemoveItemCompletely(this);
                    }
                    Slot.Repaint();
                    for (int i = 0; i < ReferencedSlots.Count; i++){
                        ReferencedSlots[i].Repaint();
                    }
                }
            }
        }

        [Tooltip("Maximum stack amount. Items stack can't be higher then this value. If the stack is bigger then the maximum stack, the item will be splitted into multiple stacks.")]
        [SerializeField]
		[Range (0, 100)]
		private int m_MaxStack = 1;

		public virtual int MaxStack {
			get{
                if (m_MaxStack > 0){
                    return m_MaxStack;
                }
                return int.MaxValue;
            }
		}

        [Tooltip("If set to true, the item is droppable from a container to the scene.")]
		[SerializeField]
		private bool m_IsDroppable = true;

		public bool IsDroppable => m_IsDroppable;

        [Tooltip("Sound that should be played when the item is dropped to the scene.")]
		[SerializeField]
		private AudioClip m_DropSound;

		public AudioClip DropSound => m_DropSound;

        [Tooltip("By default items prefab will be instantiated when dropped to the scene, you can override this option.")]
        [SerializeField]
		private GameObject m_OverridePrefab;

		public GameObject OverridePrefab => m_OverridePrefab;

        [Tooltip("Defines if the item is craftable.")][SerializeField]
        private bool m_IsCraftable;

        public bool IsCraftable => m_IsCraftable;

        [Tooltip("How long does it take to craft this item. This value is also used to display the progressbar in crafting UI.")]
        [SerializeField]
        private float m_CraftingDuration = 2f;

        public float CraftingDuration => m_CraftingDuration;

        [Tooltip("Should a skill be used when item is crafted?")]
        [SerializeField]
        private bool m_UseCraftingSkill;

        public bool UseCraftingSkill => m_UseCraftingSkill;

        [Tooltip("Name of the skill window. It is required if use crafting skill is set to true to be able to find the skill. ")]
        [SerializeField]
        private string m_SkillWindow = "Skills";
        public string SkillWindow => m_SkillWindow;

        [Tooltip("What skill should be used when crafting? The current players skill will be searched in skill window set above.")]
        [ItemPicker(true)]
        [SerializeField]
        private Skill m_CraftingSkill;
        public Skill CraftingSkill => m_CraftingSkill;

        [Tooltip("Remove the ingredients when crafting fails.")]
        [SerializeField]
        private bool m_RemoveIngredientsWhenFailed;
        public bool RemoveIngredientsWhenFailed => m_RemoveIngredientsWhenFailed;

        [Tooltip("Minimum required skill to craft this item. The player can only craft this item if his skill is high enough.")]
        [Range(0f,100f)]
        [SerializeField]
        private float m_MinCraftingSkillValue;

        public float MinCraftingSkillValue => m_MinCraftingSkillValue;

        [Tooltip("Animation to play when this item is crafted. If you don't want to play any animation, delete this value.")]
        [SerializeField]
        private string m_CraftingAnimatorState= "Blacksmithy";

        public string CraftingAnimatorState => m_CraftingAnimatorState;

        [SerializeField]
        private ItemModifierList m_CraftingModifier= new();
        public ItemModifierList CraftingModifier {
            get => m_CraftingModifier;
            set => m_CraftingModifier = value;
        }

        public List<Ingredient> ingredients = new();


        public ItemContainer Container
        {
            get {
                if (Slot != null) {
                    return Slot.Container;
                }
                return null;
            }
        }

        public Slot Slot {
			get{ 
                if(Slots.Count > 0)
                {
                    return Slots[0];
                }
                return null; 
            }
		}

        private List<Slot> m_Slots= new();
        public List<Slot> Slots
        {
            get => m_Slots;
            set => m_Slots = value;
        }

        private List<Slot> m_ReferencedSlots = new();

		public List<Slot> ReferencedSlots {
			get => m_ReferencedSlots;
            set => m_ReferencedSlots = value;
        }

		[SerializeField]
		private List<ObjectProperty> properties = new();

        public void AddProperty(string name, object value) {
            ObjectProperty property = new ObjectProperty();
            property.Name = name;
            property.SetValue(value);
            properties.Add(property);
        }

        public void RemoveProperty(string name)
        {
            properties.RemoveAll(x => x.Name == name);
        }

        public ObjectProperty FindProperty (string name)
		{
			return properties.FirstOrDefault (property => property.Name == name);
		}


		public ObjectProperty[] GetProperties ()
		{
			return properties.ToArray ();
		}

		public void SetProperties (ObjectProperty[] properties)
		{
			this.properties = new List<ObjectProperty> (properties);
		}

        protected virtual void OnEnable ()
		{
			if (string.IsNullOrEmpty (m_Id)) {
				m_Id = Guid.NewGuid ().ToString ();
			}
		}

        public List<KeyValuePair<string, string>> GetPropertyInfo()
        {
            List<KeyValuePair<string, string>> propertyInfo = new List<KeyValuePair<string, string>>();
            foreach (ObjectProperty property in properties)
            {
                if (property.show)
                {
                    propertyInfo.Add(new KeyValuePair<string, string>(UnityTools.ColorString(property.Name,property.color),FormatPropertyValue(property)));
                }
            }
            return propertyInfo;
        }

        private string FormatPropertyValue(ObjectProperty property) {
            string propertyValue = string.Empty;

            if (property.SerializedType == typeof(Vector2))
            {
                propertyValue = property.vector2Value.x + "-" + property.vector2Value.y;
            }
            else if (property.SerializedType== typeof(string)) {
                propertyValue = property.stringValue;
            }
            else {
                propertyValue = ((UnityTools.IsNumeric(property.GetValue()) && Convert.ToSingle(property.GetValue()) > 0f) ? "+" : "-");
                propertyValue += (UnityTools.IsNumeric(property.GetValue()) ? Mathf.Abs(Convert.ToSingle(property.GetValue())) : property.GetValue()).ToString();
            }
            propertyValue = UnityTools.ColorString(propertyValue, property.color);
            return propertyValue;
        }

        public virtual void Use() { }

        public virtual void GetObjectData(Dictionary<string, object> data)
        {
            data.Add("Name", Name);
            data.Add("Stack", Stack);
            data.Add("RarityIndex", InventoryManager.Database.raritys.IndexOf(Rarity));

            if (Slot != null)
            {
                data.Add("Index", Slot.Index);
            }

            List<object> objectProperties = new List<object>();

            foreach (ObjectProperty property in properties)
            {
                Dictionary<string, object> propertyData = new Dictionary<string, object>();
                if (!typeof(UnityEngine.Object).IsAssignableFrom(property.SerializedType))
                {
                    propertyData.Add("Name", property.Name);
                    propertyData.Add("Value", property.GetValue());
                    objectProperties.Add(propertyData);
                }
            }
            data.Add("Properties",objectProperties);

            if (Slots.Count > 0)
            {
                List<object> slots = new List<object>();
                for (int i = 0; i < Slots.Count; i++)
                {
                    slots.Add(Slots[i].Index);
                }
                data.Add("Slots", slots);
            }

            if (ReferencedSlots.Count > 0)
            {
                List<object> references = new List<object>();
                for (int i = 0; i < ReferencedSlots.Count; i++)
                {
                    Dictionary<string, object> referenceData = new Dictionary<string, object>{
                        { "Container", ReferencedSlots[i].Container.Name },
                        { "Slot", ReferencedSlots[i].Index }
                    };
                    references.Add(referenceData);
                }
                data.Add("Reference", references);
            }
        }

        public virtual void SetObjectData(Dictionary<string, object> data)
        {
            Stack = Convert.ToInt32(data["Stack"]);
            if (data.ContainsKey("RarityIndex")) {
                int rarityIndex =Convert.ToInt32(data["RarityIndex"]);
                if (rarityIndex > -1 && rarityIndex < InventoryManager.Database.raritys.Count) {
                    m_Rarity = InventoryManager.Database.raritys[rarityIndex];
                }
            }

            if (data.ContainsKey("Properties"))
            {
                List<object> objectProperties = data["Properties"] as List<object>;
                for (int i = 0; i < objectProperties.Count; i++)
                {
                    Dictionary<string, object> propertyData = objectProperties[i] as Dictionary<string, object>;
                    string propertyName = (string)propertyData["Name"];
                    object propertyValue = propertyData["Value"];
                    ObjectProperty property = FindProperty(propertyName);
                    if (property == null) {
                        property = new ObjectProperty();
                        property.Name = propertyName;
                        properties.Add(property);
                    }
                    property.SetValue(propertyValue);

                }
            }

            if (data.ContainsKey("Reference"))
            {
                List<object> references = data["Reference"] as List<object>;
                for (int i = 0; i < references.Count; i++)
                {
                    Dictionary<string, object> referenceData = references[i] as Dictionary<string, object>;
                    string container = (string)referenceData["Container"];
                    int slot = Convert.ToInt32(referenceData["Slot"]);
                    ItemContainer referenceContainer = WidgetUtility.Find<ItemContainer>(container);
                    if (referenceContainer != null)
                    {
                        referenceContainer.ReplaceItem(slot, this);
                    }
                }
            }
        }

        [Serializable]
        public class Ingredient
        {
            [ItemPicker(true)]
            public Item item;
            public int amount = 1;
        }
    }
}