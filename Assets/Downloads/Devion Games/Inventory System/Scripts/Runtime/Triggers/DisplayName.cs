using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public class DisplayName : MonoBehaviour, ITriggerCameInRange, ITriggerUsedHandler, ITriggerUnUsedHandler, ITriggerWentOutOfRange
    {
        [SerializeField]
        [EnumFlags]
        protected DisplayNameType m_DisplayType = DisplayNameType.Raycast;
        [SerializeField]
        protected Color m_Color = Color.white;
        [SerializeField]
        protected Vector3 m_Offset = Vector3.zero;

        protected BaseTrigger m_Trigger;

        protected virtual void DoDisplayName(bool state)
        {
            if (state)
            {
                FloatingTextManager.Add(gameObject, gameObject.name.Replace("(Clone)", ""), m_Color, m_Offset);
            }
            else
            {
                FloatingTextManager.Remove(gameObject);
            }
        }

        private void Start()
        {
            m_Trigger = GetComponent<BaseTrigger>();
            EventHandler.Register(gameObject, "OnPointerEnterTrigger", OnPointerEnterTrigger);
            EventHandler.Register(gameObject, "OnPointerExitTrigger", OnPointerExitTrigger);


            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always))
                DoDisplayName(true);
        }

        private void OnDestroy()
        {
            DoDisplayName(false);
        }

        public void OnPointerEnterTrigger()
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Raycast) &&   
                !(m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) && m_Trigger != null && m_Trigger.InRange ||
                m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always) ||   
                m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) && m_Trigger != null && m_Trigger.InUse))
            {
                DoDisplayName(true);
            }
        }

        public void OnPointerExitTrigger()
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Raycast) &&
                !(m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) && m_Trigger != null && m_Trigger.InRange || 
                m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always) ||   
                m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) && m_Trigger != null && m_Trigger.InUse))
            {
                DoDisplayName(false);
            }
        }

        public void OnCameInRange(GameObject player)
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange))
                DoDisplayName(true);
        }

        public void OnTriggerUsed(GameObject player)
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse))
                DoDisplayName(true);
        }

        public void OnTriggerUnUsed(GameObject player)
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InUse) &&
               !(m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always)))
            {
                DoDisplayName(false);
            }
        }

        public void OnWentOutOfRange(GameObject player)
        {
            if (m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.InRange) &&
                 !(m_DisplayType.HasFlag<DisplayNameType>(DisplayNameType.Always)))
            {
                DoDisplayName(false);
            }
        }


        [System.Flags]
        public enum DisplayNameType
        {
            Always = 1,
            InRange = 2,
            InUse = 4,
            Raycast = 8,
        }
    }
}