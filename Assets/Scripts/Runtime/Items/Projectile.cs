using System.Collections;
using UnityEngine;

namespace Runtime.Items
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField] private LayerMask _ignoreLayer;
        [SerializeField] private float _timer = 10;
        [SerializeField] private bool _destroyWhenTriggerEnter = true;
        [SerializeField] private bool _destroyWhenTimer = true;

        private Rigidbody _rigidbody;
        private float _speedProjectile;
        private IDamage _damage;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_timer);
            if(_destroyWhenTimer)
                Destroy(gameObject);
        }

        public void StartActive(IDamage damage, float speedProjectile)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _damage = damage;
            _speedProjectile = speedProjectile;
            _rigidbody.velocity = _direction * _speedProjectile;
        }

        /*
        private void FixedUpdate()
            => _rigidbody.AddForce(_direction * _speedProjectile, ForceMode.Impulse);
            */

        private void OnTriggerEnter(Collider other)
        {
            ITakeDamage takeDamage = other.GetComponent<ITakeDamage>();

            if(other.gameObject.layer != _ignoreLayer && takeDamage != null){
                takeDamage.TakeDamage(_damage);
            }

            if(_destroyWhenTriggerEnter)
                Destroy(gameObject);
        }
    }
}