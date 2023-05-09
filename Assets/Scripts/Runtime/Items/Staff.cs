using UnityEngine;
using Utilities.Runtime.Attributes;

namespace Runtime.Items
{
    public class Staff : MonoBehaviour, IDamage
    {
        public float Damage => _damage;
        public Item ItemData => _itemData;

        [SerializeField] private Item _itemData;
        [SerializeField] private float _damage;
        [SerializeField] private Projectile _projective;
        [SerializeField, Enum] private KeyCode _activeKey;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private float _speedProjective = 10;

        private void Update()
        {
            if(Input.GetKeyDown(_activeKey) && _projective != null && _startPoint != null){
                Projectile projectile = Instantiate(_projective, _startPoint.position,Quaternion.identity);
                projectile.StartActive(this, _speedProjective);
            }
        }
    }
}