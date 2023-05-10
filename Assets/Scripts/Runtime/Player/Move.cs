using System;
using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Move : MonoBehaviour
    {
        [Header("Налаштування")]
        [SerializeField, Tooltip("Щвидкість")]
        private float _speed;

        [SerializeField, Tooltip("Прискорення")]
        private float _acceleration;

        private Rigidbody _rigidbody;
        private Input.MoverActions _inputAction;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _inputAction = InputManager.InputController.Mover;
        }

        private void FixedUpdate()
        {
            MoveDirection();
        }

        private void MoveDirection()
        {
            Vector2 direction = _inputAction.Movement.ReadValue<Vector2>();
            /*if(Input.GetKey(_accelerationKey)){
                x += _acceleration;
                z += _acceleration;
            }*/
            if(direction.x != 0 && direction.y != 0)
                _rigidbody.velocity = new Vector3(direction.x * _speed + _acceleration * Time.fixedDeltaTime, 0,
                                                  direction.y * _speed + _acceleration * Time.fixedDeltaTime);
        }
    }
}