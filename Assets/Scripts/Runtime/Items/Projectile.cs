using System.Collections;
using UnityEngine;

namespace Runtime.Items
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField] private float _speedProjectile = 500;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private float _timer = 10;

        private Rigidbody _rigidbody;
        private float speedProjectile;
        private IDamage _damage;

        public void StartActive(IDamage damage, float speedProjectile = 10)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _damage = damage;
            this.speedProjectile = speedProjectile;
            StartCoroutine(Timer());
        }

        private void FixedUpdate()
            => _rigidbody.AddForce(_direction * speedProjectile, ForceMode.Impulse);

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<ITakeDamage>()?.TakeDamage(_damage);
            if(other.gameObject.layer == _layer)
                Destroy(gameObject);
        }

        private IEnumerator Timer()
        {

            while(_timer > 0){
                _timer --;
                yield return new WaitForSeconds(1);
            }

            Destroy(gameObject);
        }
    }
}