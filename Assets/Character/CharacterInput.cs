using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;
        [SerializeField] private InputActionReference crouchInput;
        [SerializeField] private InputActionReference cameraFOVInput;

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            jumpInput.action.Enable();
            moveInput.action.Enable();
            crouchInput.action.Enable();
            cameraFOVInput.action.Enable();
        }

        private void OnDisable()
        {
            jumpInput.action.Disable();
            moveInput.action.Disable();
            crouchInput.action.Disable();
            cameraFOVInput.action.Disable();
        }
    }
}