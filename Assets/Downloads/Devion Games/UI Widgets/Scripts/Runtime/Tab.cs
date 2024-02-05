using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DevionGames.UIWidgets
{
    [RequireComponent(typeof(Button))]
    public class Tab : MonoBehaviour
    {
        public Color selectedColor;
        public bool isOn;
        public TabEvent onSelect = new();
        public TabEvent onDeselect = new();

        protected Button m_Button;
        protected Image m_Image;
        protected Color m_DefaultColor;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
            m_Image = GetComponent<Image>();
            m_DefaultColor = m_Image.color;
            m_Button.onClick.AddListener(Select);
        }

        private void Start()
        {
            if(isOn){
                Select();
            }
        }

        public void Select()
        {
            m_Button.transform.parent.BroadcastMessage("Deselect", this, SendMessageOptions.DontRequireReceiver);
            onSelect.Invoke();
            m_Image.color = selectedColor;
        }

        private void Deselect(Tab exceptTab)
        {
            if(this != exceptTab){
                onDeselect.Invoke();
                m_Image.color = m_DefaultColor;
            }
        }

        [System.Serializable]
        public class TabEvent : UnityEvent { }
    }
}