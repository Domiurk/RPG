using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.InventorySystem
{
    public class SkillSlot : ItemSlot
    {
        [SerializeField]
        protected Text m_Value;

        public override void Repaint()
        {
            base.Repaint();

            Skill skill = ObservedItem as Skill;

            if (m_Value != null)
            {
                m_Value.text = (skill != null ? skill.CurrentValue.ToString("F1")+"%" : string.Empty);
            }
        }
    }
}