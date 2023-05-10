using Runtime.Player;
using UnityEngine;

namespace Runtime.Items
{
    public class StaffV2 : MonoBehaviour, IDamage
    {
        public float Damage => 5;
        
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _point;
        [SerializeField] private float _speedProjectile = 10;

        private void Start()
        {
            InputManager.InputController.Mover.Use.performed += _ => { Shoot();};
        }

        public void Shoot()
        {
            if(Physics.Raycast(new Ray(_point.position, Vector3.forward), out RaycastHit hit)){
                Vector3 endPoint = hit.point;
                Projectile projectile = Instantiate(this._projectile, _point.position, _point.rotation);
                projectile.transform.LookAt(endPoint);
                projectile.StartActive(this, _speedProjectile);
                // Rigidbody rigidBody = projectile.GetComponent<Rigidbody>();
                // rigidBody.velocity = _point.forward * projectileSpeed;
            }
        }
    }
}