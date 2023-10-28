using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Downloads.Invector_3rdPersonController_LITE.Scripts.CharacterController
{
    public class PersonNewInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }

        private InputAction MovementAction;
        private InputAction SprintAction;
        private InputAction CrouchAction;
        private InputAction JumpAction;
        private vThirdPersonController personController;
        private vThirdPersonCamera personCamera;
        private Camera cameraMain;

        private void Awake()
        {
            InitializeController();
            InitializeTpCamera();
        }

        private void FixedUpdate()
        {
            personController.UpdateMotor();           // updates the ThirdPersonMotor methods
            personController.ControlLocomotionType(); // handle the controller locomotion type and move speed
            personController.ControlRotationType();   // handle the controller rotation type
        }

        private void Update()
        {
            InputHandle();                     // update the input methods
            personController.UpdateAnimator(); // updates the Animator Parameters
        }

        private void OnEnable()
        {
            InputActions.Player.Enable();
            MovementAction.Enable();
            SprintAction.Enable();
            CrouchAction.Enable();
            JumpAction.Enable();
        }

        private void OnDisable()
        {
            InputActions.Player.Disable();
            MovementAction.Disable();
            SprintAction.Disable();
            CrouchAction.Disable();
            JumpAction.Disable();
        }

        public void OnAnimatorMove()
        {
            personController.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        private void InitializeController()
        {
            InputActions = new PlayerInputActions();
            personController = GetComponent<vThirdPersonController>();
            MovementAction = InputActions.Player.Moving;
            SprintAction = InputActions.Player.Sprint;
            CrouchAction = InputActions.Player.Crouch;
            JumpAction = InputActions.Player.Jump;
            //InputActions.Player.Jump.performed += _ => JumpInput();
            //InputActions.Player.Sprint.performed += _ => SprintInput(true);
            //InputActions.Player.Sprint.canceled += _ => SprintInput(false);
            //InputActions.Player.Crouch.performed += _ => StrafeInput();
            JumpAction.performed += _ => JumpInput();
            SprintAction.performed += _ => SprintInput(true);
            SprintAction.canceled += _ => SprintInput(false);
            CrouchAction.performed += _ => StrafeInput();

            if(personController != null)
                personController.Init();
        }

        private void InitializeTpCamera()
        {
            if(personCamera == null){
                personCamera = FindObjectOfType<vThirdPersonCamera>();
                if(personCamera == null)
                    return;

                if(personCamera){
                    personCamera.SetMainTarget(this.transform);
                    personCamera.Init();
                }
            }
        }

        private void InputHandle()
        {
            MoveInput();
            CameraInput();
        }

        public void MoveInput()
        {
            Vector2 input = InputActions.Player.Moving.ReadValue<Vector2>();
            personController.input.x = input.x;
            personController.input.z = input.y;
        }

        protected void CameraInput()
        {
            if(!cameraMain){
                if(!Camera.main)
                    Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else{
                    cameraMain = Camera.main;
                    if(cameraMain != null)
                        personController.rotateTarget = cameraMain.transform;
                }
            }

            if(cameraMain != null){
                personController.UpdateMoveDirection(cameraMain.transform);
            }

            if(personCamera == null)
                return;

            float y = Mouse.current.delta.value.normalized.y;
            float x = Mouse.current.delta.value.normalized.x;

            personCamera.RotateCamera(x, y);
        }

        private void SprintInput(bool started)
            => personController.Sprint(started);

        private void StrafeInput()
            => personController.Strafe();

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        private bool JumpConditions()
            => personController.isGrounded && personController.GroundAngle() < personController.slopeLimit &&
               !personController.isJumping && !personController.stopMove;

        /// <summary>
        /// InputActions to trigger the Jump 
        /// </summary>
        private void JumpInput()
        {
            if(JumpConditions())
                personController.Jump();
        }
    }
}