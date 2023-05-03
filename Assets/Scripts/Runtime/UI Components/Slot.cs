using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI_Components
{
    public class Slot : MonoBehaviour
    {
        public bool IsEmpty => _item == null;
        public Item Item => _item;

        [SerializeField] private Item _item;
        [SerializeField] private Image _icon;

        private Button _button;
        private ItemContainer _container;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _container = GetComponentInParent<ItemContainer>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(() => _container.Drop(this));
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(() => _container.Drop(this));
        }

        public void Clear()
        {
            _item = null;
            _icon.enabled = false;
            UpdateReference();
        }

        public void SetItem(Item item)
        {
            _item = item;
            _icon.enabled = true;
            UpdateReference();
        }

        private void UpdateReference()
        {
            if(_icon != null && _item != null && _item.Icon != null){
                _icon.sprite = _item.Icon;
            }

            // _icon.enabled = _icon  && _item && _item.Icon;
        }
    }
}