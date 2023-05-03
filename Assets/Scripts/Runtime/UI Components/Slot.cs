using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI_Components
{
    public class Slot : MonoBehaviour
    {
        public bool IsEmpty => _item == null;

        [SerializeField] private Item _item;
        [SerializeField] private Image Icon;
        
        private Button _button;
        private ItemContainer _container;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _container = GetComponentInParent<ItemContainer>();
        }
        public GameObject Drop(out Item item)
        {
            GameObject instance = null;
            if(_item is StaticItem staticItem){
                instance = staticItem.Prefab;
            }

            item = _item;
            _item = null;
            UpdateReference();
            return instance;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(()=>_container.Drop(this));
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(()=>_container.Drop(this));
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
            Icon.enabled = !Icon;

            if(_item.Icon != null && Icon != null){
                Icon.sprite = _item.Icon;
            }
        }
    }
}