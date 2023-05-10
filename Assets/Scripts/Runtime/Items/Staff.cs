using Runtime.Player;
using UnityEngine;

namespace Runtime.Items
{
    public class Staff : MonoBehaviour, IDamage
    {
        public float Damage => _damage;
        public Item ItemData => _itemData;

        [SerializeField] private Item _itemData;
        [SerializeField] private float _damage;
        [SerializeField] private Projectile _projective;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private float _speedProjective = 10;

        private void Start()
        {
            InputManager.InputController.Mover.Use.performed += _ => { Shoot();};
        }

        
        private void Shoot()
        {
            if(_projective != null && _startPoint != null){
                Projectile projectile = Instantiate(_projective, _startPoint.position,Quaternion.identity);
                projectile.StartActive(this, _speedProjective);
            }
        }
    }
}