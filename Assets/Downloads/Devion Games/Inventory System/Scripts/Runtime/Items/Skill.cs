using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public class Skill : UsableItem
    {
        [Range(0f,100f)]
        [SerializeField]
        protected float m_FixedSuccessChance = 50f;


        protected float m_CurrentValue;
        public float CurrentValue {
            get => m_CurrentValue;
            set {
                if (m_CurrentValue != value)
                {
                    m_CurrentValue = value;
                    if (Slot != null)
                        Slot.Repaint();
                }
            
            }
        }

        [SerializeField]
        protected SkillModifier m_GainModifier;
        public SkillModifier GainModifier {
            get => m_GainModifier;
            set => m_GainModifier = value;
        }

        public bool CheckSkill() {
            m_GainModifier.Modify(this);

            bool result = (CurrentValue + m_FixedSuccessChance) > Random.Range(0f, 100f);
            return result;
        }

        public override void GetObjectData(Dictionary<string, object> data)
        {
            base.GetObjectData(data);
            data.Add("SkillValue",CurrentValue);
        }

        public override void SetObjectData(Dictionary<string, object> data)
        {
            base.SetObjectData(data);
            if(data.ContainsKey("SkillValue"))
                CurrentValue = System.Convert.ToSingle(data["SkillValue"]);
        }
    }
}