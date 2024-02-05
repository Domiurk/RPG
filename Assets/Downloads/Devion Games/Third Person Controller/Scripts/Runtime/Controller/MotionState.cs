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

        public string FriendlyName => m_FriendlyName;

        [SerializeField] private InputActionReference m_InputAction;

        public InputActionReference InputAction
        {
            get => m_InputAction;
            set => m_InputAction = value;
        }

        [SerializeField] private StartType m_StartType = StartType.Automatic;

        public StartType StartType
        {
            get => m_StartType;
            set => m_StartType = value;
        }

        [SerializeField]
        private StopType m_StopType = StopType.Automatic;

        public StopType StopType
        {
            get => m_StopType;
            set => m_StopType = value;
        }

        [SerializeField]
        private bool m_ConsumeInputOverUI;

        public bool ConsumeInputOverUI => m_ConsumeInputOverUI;

        [SerializeField]
        private bool m_PauseItemUpdate = true;

        public bool PauseItemUpdate => m_PauseItemUpdate;

        [SerializeField]
        private float m_TransitionDuration = 0.2f;

        public float TransitionDuration => m_TransitionDuration;

        [SerializeField]
        private string m_State;

        public string State
        {
            get => m_State;
            set => m_State = value;
        }

        [SerializeField]
        private string m_CameraPreset = "Default";
        public string CameraPreset
        {
            get => m_CameraPreset;
            set => m_CameraPreset = value;
        }

        public bool IsActive { get; private set; }

        public int Index { get; set; }

        public int Layer { get; set; }

        public ThirdPersonController Controller
        {
            get => m_Controller;
            set => m_Controller = value;
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
            m_Transform = transform;
            m_Animator = m_Transform.GetComponent<Animator>();
            m_Rigidbody = m_Transform.GetComponent<Rigidbody>();
            m_CapsuleCollider = m_Transform.GetComponent<CapsuleCollider>();
            m_Camera = Camera.main?.GetComponent<ThirdPersonCamera>();

            ThirdPersonController[] controllers = m_Transform.GetComponents<ThirdPersonController>();

            foreach(ThirdPersonController control in controllers){
                if(control.enabled){
                    m_Controller = control;
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
            if(!IsActive || !force && !CanStop()){
                return;
            }

            if(PauseItemUpdate)
                SendMessage("PauseItemUpdate", false, SendMessageOptions.DontRequireReceiver);
            IsActive = false;
            OnStop();
            if(!string.IsNullOrEmpty(GetDestinationState()))
                m_Controller.CheckDefaultAnimatorStates();
            CameraSettings preset = m_Camera.Presets.FirstOrDefault(x => x.Name == CameraPreset);

            if(preset != null && preset.Name != "Default"){
                preset.IsActive = false;
            }
        }

        public void StartMotion()
        {
            if(PauseItemUpdate)
                SendMessage("PauseItemUpdate", true, SendMessageOptions.DontRequireReceiver);
            IsActive = true;

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
            int layers = m_Animator.layerCount;
            string destinationState = GetDestinationState();

            for(int i = 0; i < layers; i++){
                AnimatorStateInfo info = m_Animator.GetCurrentAnimatorStateInfo(i);
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
            return m_State;
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
            m_InPosition = false;
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;
            Quaternion startingRotation = transform.rotation;

            while(elapsedTime < time){
                transform.position = Vector3.Lerp(startingPosition, position, (elapsedTime / time));
                transform.rotation = Quaternion.Slerp(startingRotation, rotation, (elapsedTime / time));
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            m_InPosition = true;

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