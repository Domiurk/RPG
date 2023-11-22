using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames
{
    public abstract class MotionState : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] private string m_FriendlyName = string.Empty;

        public string FriendlyName => this.m_FriendlyName;

        [SerializeField] private InputActionReference m_InputAction;

        public InputActionReference InputAction
        {
            get => this.m_InputAction;
            set => this.m_InputAction = value;
        }

        [SerializeField] private StartType m_StartType = StartType.Automatic;

        public StartType StartType
        {
            get => this.m_StartType;
            set => this.m_StartType = value;
        }

        [SerializeField]
        private StopType m_StopType = StopType.Automatic;

        public StopType StopType
        {
            get => this.m_StopType;
            set => this.m_StopType = value;
        }

        [SerializeField]
        private bool m_ConsumeInputOverUI = false;

        public bool ConsumeInputOverUI => this.m_ConsumeInputOverUI;

        [SerializeField]
        private bool m_PauseItemUpdate = true;

        public bool PauseItemUpdate => this.m_PauseItemUpdate;

        [SerializeField]
        private float m_TransitionDuration = 0.2f;

        public float TransitionDuration => this.m_TransitionDuration;

        [SerializeField]
        private string m_State;

        public string State
        {
            get => this.m_State;
            set => this.m_State = value;
        }

        [SerializeField]
        private string m_CameraPreset = "Default";
        public string CameraPreset
        {
            get => this.m_CameraPreset;
            set => this.m_CameraPreset = value;
        }

        public bool IsActive { get; private set; }

        public int Index { get; set; }

        public int Layer { get; set; }

        public ThirdPersonController Controller
        {
            get => this.m_Controller;
            set => this.m_Controller = value;
        }

        protected Animator m_Animator;
        protected ThirdPersonCamera m_Camera;
        protected Rigidbody m_Rigidbody;
        protected CapsuleCollider m_CapsuleCollider;
        protected ThirdPersonController m_Controller;
        protected Transform m_Transform;
        protected bool m_InPosition = true;

        private void Start()
        {
            //this.m_Transform = transform.root;
            this.m_Transform = transform;
            this.m_Animator = this.m_Transform.GetComponent<Animator>();
            this.m_Rigidbody = this.m_Transform.GetComponent<Rigidbody>();
            this.m_CapsuleCollider = this.m_Transform.GetComponent<CapsuleCollider>();
            this.m_Camera = Camera.main?.GetComponent<ThirdPersonCamera>();

            ThirdPersonController[] controllers = this.m_Transform.GetComponents<ThirdPersonController>();

            foreach(ThirdPersonController control in controllers){
                if(control.enabled){
                    this.m_Controller = control;
                }
            }
        }

        private void StopMotion()
        {
            if(IsActive){
                StopMotion(true);
            }
        }

        public void StopMotion(bool force)
        {
            if(!this.IsActive || !force && !this.CanStop()){
                return;
            }

            if(PauseItemUpdate)
                SendMessage("PauseItemUpdate", false, SendMessageOptions.DontRequireReceiver);
            this.IsActive = false;
            OnStop();
            if(!string.IsNullOrEmpty(GetDestinationState()))
                m_Controller.CheckDefaultAnimatorStates();
            CameraSettings preset = this.m_Camera.Presets.FirstOrDefault(x => x.Name == CameraPreset);

            if(preset != null && preset.Name != "Default"){
                preset.IsActive = false;
            }
        }

        public void StartMotion()
        {
            if(PauseItemUpdate)
                SendMessage("PauseItemUpdate", true, SendMessageOptions.DontRequireReceiver);
            this.IsActive = true;

            OnStart();

            CameraSettings preset = m_Camera.Presets.FirstOrDefault(x => x.Name == CameraPreset);

            if(preset != null){
                preset.IsActive = true;
            }

            string destinationState = GetDestinationState();

            if(!string.IsNullOrEmpty(destinationState)){
                m_Animator.CrossFadeInFixedTime(destinationState, TransitionDuration);
            }

        }

        protected bool IsPlaying()
        {
            int layers = this.m_Animator.layerCount;
            string destinationState = GetDestinationState();

            for(int i = 0; i < layers; i++){
                AnimatorStateInfo info = this.m_Animator.GetCurrentAnimatorStateInfo(i);
                if(info.IsName(destinationState))
                    return true;
            }

            return false;
        }

        public virtual bool CanStart()
        {
            return true;
        }

        public virtual void OnStart() { }

        public virtual bool UpdateVelocity(ref Vector3 velocity)
        {
            return true;
        }

        public virtual bool UpdateRotation()
        {
            return true;
        }

        public virtual bool UpdateAnimator()
        {
            return true;
        }

        public virtual bool UpdateAnimatorIK(int layer)
        {
            return true;
        }

        public virtual bool CheckGround()
        {
            return true;
        }

        public virtual bool CheckStep()
        {
            return true;
        }

        public virtual bool CanStop()
        {
            return true;
        }

        public virtual void OnStop() { }

        public virtual string GetDestinationState()
        {
            return this.m_State;
        }

        protected void MoveToTarget(Transform transform,
                                    Vector3 position,
                                    Quaternion rotation,
                                    float time,
                                    System.Action onComplete)
        {
            StartCoroutine(MoveToTargetInternal(transform, position, rotation, time, onComplete));
        }

        private IEnumerator MoveToTargetInternal(Transform transform,
                                                 Vector3 position,
                                                 Quaternion rotation,
                                                 float time,
                                                 System.Action onComplete)
        {
            this.m_InPosition = false;
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;
            Quaternion startingRotation = transform.rotation;

            while(elapsedTime < time){
                transform.position = Vector3.Lerp(startingPosition, position, (elapsedTime / time));
                transform.rotation = Quaternion.Slerp(startingRotation, rotation, (elapsedTime / time));
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            this.m_InPosition = true;

            if(onComplete != null){
                onComplete.Invoke();
            }
        }
    }

    public enum StartType
    {
        Automatic,
        Down,
        Press
    }

    public enum StopType
    {
        Automatic,
        Manual,
        Up,
        Toggle
    }
}