using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CreateAssetMenu(fileName ="SimplePropertyModifier",menuName = "Devion Games/Inventory System/Modifiers/Property")]
    [System.Serializable]
    public class PropertyModifier : ItemModifier
    {
        [SerializeField]
        protected bool m_ApplyToAll = true;
        [SerializeField]
        protected List<string> m_Properties = new();
        [SerializeField]
        protected PropertyModifierType m_ModifierType = PropertyModifierType.Flat;
        [MinMaxSlider(-100,100)]
        [SerializeField]
        protected Vector2 m_Range= new(-10f,10f);

        public override void Modify(Item item)
        {
            List<ObjectProperty> properties = new List<ObjectProperty>();
            if (m_ApplyToAll) {
                properties.AddRange(item.GetProperties());
            }else{
                for (int i = 0; i < m_Properties.Count; i++) {
                    ObjectProperty property = item.FindProperty(m_Properties[i]);
                    if (property == null) {
                        property = new ObjectProperty();
                        property.Name = m_Properties[i];
                        property.floatValue = 0f;

                    }
                    properties.Add(property);
                }
            }

            for (int i = 0; i < properties.Count; i++)
            {
                ObjectProperty current = properties[i];
                object value = current.GetValue();
                if (!(UnityTools.IsNumeric(value) && current.show)) continue;

                float currentValue = System.Convert.ToSingle(value);   
                float newValue = currentValue;
                float random = Random.Range(m_Range.x, m_Range.y);

                newValue = m_ModifierType switch{
                    PropertyModifierType.Flat => currentValue + random,
                    PropertyModifierType.Percent => currentValue + currentValue * random * 0.01f,
                    _ => newValue
                };

                if (value is float)
                {
                    current.SetValue(newValue);
                }
                else if (value is int)
                {
                    current.SetValue(Mathf.RoundToInt(newValue));
                }
            }
        }

        public enum PropertyModifierType { 
            Flat,
            Percent
        }
    }
}