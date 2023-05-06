using System;
using UnityEngine;

namespace Runtime
{
    public class Staff : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _startProjectile;

        public void Shoot()
        {
           Projectile projectile = Instantiate(_projectile, _startProjectile.position, Quaternion.identity);
           projectile.Start(Vector3.forward);
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IDamage
{
    public float Damage => _damage;
    [SerializeField] private int _damage = 5;

    private Rigidbody _rigidbody;
    
    public void Start(Vector3 dir)
    { }
}

public interface IDamage
{
    float Damage { get; }
}