using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public class MenuItem : Selectable, IPointerClickHandler
    {
        [SerializeField] private UnityEvent m_Trigger = new ();

        public UnityEvent onTrigger
        {
            get{
                return this.m_Trigger ??= new UnityEvent();
            }
            set => this.m_Trigger = value;
        }

        private void Press()
        {
            if(!IsActive() || !IsInteractable())
                return;

            onTrigger.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Press();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            DoStateTransition(SelectionState.Highlighted, false);
        }
    }
}