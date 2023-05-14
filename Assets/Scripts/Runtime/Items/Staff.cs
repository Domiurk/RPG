using Runtime.Player;
using UnityEngine;

namespace Runtime.Items
{
    public class Staff : MonoBehaviour, IDamage
    {
        public float Damage => 5;

        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _point;
        [SerializeField] private float _speedProjectile = 10;

        private void Start()
            => InputManager.Current.InputController.Mover.Use.performed += _ => Shoot();

        public void Shoot()
        {
            Camera playerCamera = Camera.main;
            int maximumDistance = 500;
            int mask = LayerMask.GetMask("Ground");
            Quaternion rotation = Quaternion.LookRotation(playerCamera!.transform.forward * 1000.0f - _point.position);
            if(Physics.Raycast(new Ray(playerCamera!.transform.position, playerCamera!.transform.forward),
                               out RaycastHit hit, maximumDistance, mask))
                rotation = Quaternion.LookRotation(hit.point - _point.position);

            Projectile projectile = Instantiate(_projectile, _point.position, rotation);
            projectile.StartActive(this, _speedProjectile);
        }
    }
}