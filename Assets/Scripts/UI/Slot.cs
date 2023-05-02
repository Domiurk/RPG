using Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Slot : MonoBehaviour
    {
        public bool IsEmpty => _item == null;
    
        [SerializeField] private Image _icon;
        [SerializeField] private Item _item;

        private Button _button;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Drop);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Drop);
        }

        private void Drop()
        {
            if(_item == null)
                return;
            Debug.Log("Drop");
        }

        public void SetItem(Item item)
        {
            _item = item;
            UpdateReference();
        }

        private void UpdateReference()
        {
            if(_item == null)
                return;

            if(_item.Icon != null && _icon != null){
                _icon.enabled = true;
                _icon.sprite = _item.Icon;
            }
        }
    }
}