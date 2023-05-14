using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterController : MonoBehaviour, IControllable
    {
        [Header("Input Setting")] public InputAction _moveAction;
        public InputAction _crouchAction = new("Crouch", InputActionType.Button, "<Keyboard>/leftShift");
        public InputAction _sprintAction;

        [Header("Sprint")] public bool CanSprint = true;
        [SerializeField] private float _sprint = 1;

        [Header("Jump")] public bool canJump = true;
        [SerializeField] private float _jumpPowder;

        [Header("Walk")] public bool CanMove = true;
        public bool playerCanMove = true;
        public float walkSpeed = 5f;

        [Header("Crouch")] public bool CanCrouch = true;
        public float crouchHeight = .75f;
        public float speedReduction = .5f;

        private bool _isGrounded => CheckGround();
        private bool _isCrouched;
        private bool _isSprinting;
        private Vector3 _originalScale;
        private Vector3 _direction;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _moveAction.Enable();
            _sprintAction.Enable();
            _crouchAction.Enable();
            _crouchAction.started += _ => _isCrouched = true;
            _crouchAction.canceled += _ => _isCrouched = false;
            _sprintAction.started += _ => { Sprint(); };
            _sprintAction.canceled += _ => { Sprint(); };
        }

        private void FixedUpdate()
        {
            if(CanMove)
                Move();
        }

        public void Jump()
        {
            Debug.Log("Jump");
        }

        public void Sprint()
        {
            _isSprinting = _sprintAction.phase == InputActionPhase.Started;

            if(_isCrouched)
                walkSpeed += _sprint;
            else
                walkSpeed -= _sprint;
        }

        public void Crouch()
        {
            if(!CanCrouch)
                return;

            if(_isCrouched){
                transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
                walkSpeed /= speedReduction;
                _isCrouched = false;
            }
            else{
                transform.localScale = new Vector3(_originalScale.x, crouchHeight, _originalScale.z);
                walkSpeed *= speedReduction;
                _isCrouched = true;
            }
        }

        public void Move()
        {
            if(!playerCanMove)
                return;

            Vector3 targetVelocity = new Vector3(_moveAction.ReadValue<Vector2>().normalized.x, 0,
                                                 _moveAction.ReadValue<Vector2>().normalized.y);
            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;
            Vector3 velocity = _rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.y = 0;

            _rigidbody.velocity = new Vector3(velocityChange.x, 0, velocityChange.z);
            // _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        private bool CheckGround()
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            Vector3 origin = new Vector3(transform.position.x, transform.position.y - transform.localScale.y * .5f,
                                         transform.position.z);
            Vector3 direction = transform.TransformDirection(Vector3.down);
            float distance = .75f;

            return Physics.Raycast(origin, direction, distance, mask);
        }
    }
}