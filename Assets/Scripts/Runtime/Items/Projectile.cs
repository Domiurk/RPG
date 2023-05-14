using System.Collections;
using UnityEngine;

namespace Runtime.Items
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField] private float _timer = 10;
        [SerializeField] private bool _destroyWhenTriggerEnter = true;
        [SerializeField] private bool _destroyWhenTimer = true;
        // [SerializeField] private float _multiplySpeed = 1;

        private Rigidbody _rigidbody;
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
            _rigidbody.velocity = transform.forward * speedProjectile;
        }

        private void OnCollisionEnter(Collision other)
        {
            var obj = other.gameObject.GetComponent<ITakeDamage>();

            if(obj != null){
                obj.TakeDamage(_damage);
                if(_destroyWhenTriggerEnter)
                    Destroy(gameObject);
            }
        }
    }
}