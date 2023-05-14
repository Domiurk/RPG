using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    public class CameraController : MonoBehaviour
    {
        [Header("Input Setting")]
        [SerializeField]
        private InputAction _lookAction;
        [SerializeField] private InputAction _changeLockModeAction;

        [Header("Setting")][SerializeField] private Vector2 lookAngleHorizontal = new Vector2(-60, 75);
        [SerializeField] private float mouseSensitivity = 1;
        [SerializeField] private float _smooth = 1;
        [SerializeField] private bool invertCamera;
        [SerializeField] private Transform player;

        private float yaw;
        private float pitch;
        private bool _isSprinting;
        private CursorLockMode _cursorLockMode;

        private void Start()
        {
            _lookAction.Enable();
            _changeLockModeAction.Enable();
            _changeLockModeAction.performed += ChangeLockModeAction;
            Cursor.lockState = _cursorLockMode;
        }

        private void Update()
            => Look();

        private void ChangeLockModeAction(InputAction.CallbackContext obj)
        {
            _cursorLockMode = _cursorLockMode switch{
                CursorLockMode.None => CursorLockMode.Locked,
                CursorLockMode.Locked => CursorLockMode.None,
                CursorLockMode.Confined => CursorLockMode.None,
                _ => throw new ArgumentOutOfRangeException()
            };
            Cursor.lockState = _cursorLockMode;
        }

        private void Look()
        {
            Vector2 look = _lookAction.ReadValue<Vector2>();
            yaw = player.localEulerAngles.y + look.x * mouseSensitivity / _smooth;

            if(!invertCamera)
                pitch -= mouseSensitivity * look.y / _smooth;
            else
                pitch += mouseSensitivity * look.y / _smooth;

            pitch = Mathf.Clamp(pitch, lookAngleHorizontal.x, lookAngleHorizontal.y);

            player.localEulerAngles = new Vector3(0, yaw, 0);
            transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }
}