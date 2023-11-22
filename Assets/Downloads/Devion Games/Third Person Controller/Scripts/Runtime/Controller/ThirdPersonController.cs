using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace DevionGames
{
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_DontDestroyOnLoad = true;

        [HeaderLine("Input")]
        [SerializeField] private InputActionReference m_MoveAction;
        [SerializeField] private InputActionReference m_FireAction;

        [SerializeField] private float m_SpeedMultiplier = 1f;
        [EnumFlags]
        [SerializeField] private AimType m_AimType = AimType.Button | AimType.Selectable;

        [HeaderLine("Movement")]
        [SerializeField] private float m_AimRotation = 20f;
        [SerializeField] private float m_RotationSpeed = 10f;
        [SerializeField] private Vector3 m_AirSpeed = new Vector3(0.3f, 0f, 0.3f);
        [SerializeField] private float m_AirDampening = 0.15f;
        [SerializeField] private float m_GroundDampening;
        [SerializeField] private float m_StepOffset = 0.2f;
        [SerializeField] private float m_SlopeLimit = 45f;

        [HeaderLine("Physics")]
        [SerializeField] private LayerMask m_GroundLayer = 1 << 0;
        [SerializeField] private float m_SkinWidth = 0.08f;
        [field: SerializeField] public PhysicMaterial IdleFriction { get; set; }
        [field: SerializeField] public PhysicMaterial MovementFriction { get; set; }
        [field: SerializeField] public PhysicMaterial StepFriction { get; set; }
        [field: SerializeField] public PhysicMaterial AirFriction { get; set; }

        [HeaderLine("Footsteps")]
        [SerializeField] private AudioMixerGroup m_AudioMixerGroup;
        [SerializeField] private List<AudioClip> m_FootstepClips = new List<AudioClip>();

        [HeaderLine("Animator")]
        [SerializeField] private bool m_UseChildAnimator;
        [SerializeField] private float m_ForwardDampTime = 0.15f;
        [SerializeField] private float m_HorizontalDampTime = 0.15f;

        [field: SerializeField] public List<MotionState> Motions;

        private Dictionary<MotionState, bool> m_ToggleState;
        private Animator m_Animator;
        private Rigidbody m_Rigidbody;
        private CapsuleCollider m_CapsuleCollider;
        private Transform m_CameraTransform;
        private Transform m_Transform;

        private bool m_IsGrounded;
        private Vector3 m_RawInput;
        private bool m_IsAiming;
        private Vector3 m_Velocity;
        private float m_MouseInput;

        private Vector3 m_AirVelocity;
        private Vector3 m_PrevAirVelocity;
        private Vector3 m_RootMotionForce;
        private RaycastHit m_GroundHit;

        private Vector3 m_AimPosition;
        private float m_Slope;
        private AnimatorStateInfo[] m_LayerStateMap;
        private Dictionary<int, MotionState[]> m_MotionStateMap;
        private bool m_ControllerActive = true;
        private bool m_GUIClick;
        private IControllerEventHandler[] m_ControllerEvents;

        private delegate void EventFunction<in T>(T handler, object arg);

        private AudioSource m_AudioSource;

        protected static void Execute(IControllerGrounded handler, object grounded)
        {
            handler.OnControllerGrounded((bool)grounded);
        }

        protected static void Execute(IControllerAim handler, object aim)
        {
            handler.OnControllerAim((bool)aim);
        }

        private void ExecuteEvent<T>(EventFunction<T> func, object arg, bool includeDisabled = false)
            where T : IControllerEventHandler
        {
            foreach(IControllerEventHandler handler in m_ControllerEvents){
                if(ShouldSendEvent<T>(handler, includeDisabled)){
                    func.Invoke((T)handler, arg);
                }
            }
        }

        //Check if we should execute the event on that handler
        protected bool ShouldSendEvent<T>(IControllerEventHandler handler, bool includeDisabled)
        {
            bool valid = handler is T;
            if(!valid)
                return false;
            Behaviour behaviour = handler as Behaviour;
            if(behaviour != null && !includeDisabled)
                return behaviour.isActiveAndEnabled;

            return true;
        }

        public float SpeedMultiplier
        {
            get => m_SpeedMultiplier;
            set => m_SpeedMultiplier = value;
        }

        public bool IsGrounded
        {
            get => m_IsGrounded;
            set{
                if(m_IsGrounded != value){
                    m_IsGrounded = value;
                    ExecuteEvent<IControllerGrounded>(Execute, m_IsGrounded);
                }
            }
        }

        public bool IsStepping { get; private set; }

        public Vector3 RawInput
        {
            get => m_RawInput;
            set => m_RawInput = value;
        }

        public Vector3 RelativeInput
        {
            get{
                Vector3 input = Vector3.zero;

                if(IsAiming){
                    input.x = m_RawInput.x;
                    input.z = m_RawInput.z;
                }
                else{
                    float forward = Mathf.Max(Mathf.Abs(m_RawInput.x),
                                              Mathf.Max(Mathf.Abs(m_RawInput.z), 1f));
                    input.z = Mathf.Clamp(m_RawInput.magnitude, -forward, forward);
                }

                return input;
            }
        }

        public bool IsMoving { get; private set; }

        public bool IsAiming
        {
            get => m_IsAiming;
            set{
                if(m_IsAiming != value){
                    m_IsAiming = value;
                    m_Animator.SetFloat("Yaw Input", 0f);
                    ExecuteEvent<IControllerAim>(Execute, m_IsAiming);
                }
            }
        }

        public Vector3 Velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        public Quaternion LookRotation { get; private set; }

        public Vector3 RootMotionForce => m_RootMotionForce;

        private void Awake()
        {
            if(m_DontDestroyOnLoad){
                DontDestroyOnLoad(gameObject);
            }

            m_ControllerEvents = GetComponentsInChildren<IControllerEventHandler>(true);
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            Animator childAnimator = gameObject
                                     .GetComponentsInChildren<Animator>()
                                     .FirstOrDefault(x => x != m_Animator);

            if(childAnimator != null && m_UseChildAnimator){
                m_Animator.runtimeAnimatorController = childAnimator.runtimeAnimatorController;
                m_Animator.avatar = childAnimator.avatar;
                Destroy(childAnimator);
            }

            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_CameraTransform = Camera.main?.transform;
            GetComponent<CharacterIK>();
            m_ToggleState = new Dictionary<MotionState, bool>();

            for(int i = 0; i < Motions.Count; i++){
                Motions[i].Index = i;
                m_ToggleState.Add(Motions[i], false);
            }

            m_LayerStateMap = new AnimatorStateInfo[m_Animator.layerCount];
            m_MotionStateMap = new Dictionary<int, MotionState[]>();

            for(int j = 0; j < m_Animator.layerCount; j++){
                AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(j);
                List<MotionState> states = new List<MotionState>();

                foreach(MotionState state in
                        Motions.Where(state => m_Animator.HasState(j, Animator.StringToHash(state.State)))){
                    state.Layer = j;
                    states.Add(state);
                }

                m_MotionStateMap.Add(j, states.ToArray());
                m_LayerStateMap[j] = stateInfo;
            }
        }

        private void OnEnable()
        {
            SetControllerActive(true);
            m_MoveAction.action.Enable();
            m_FireAction.action.Enable();

            foreach(MotionState motion in Motions.Where(m => m.InputAction != null))
                motion.InputAction.action.Enable();
        }

        private void OnDisable()
        {
            SetControllerActive(false);
            m_MoveAction.action.Disable();
            m_FireAction.action.Disable();
            foreach(MotionState motion in Motions.Where(m => m.InputAction != null))
                motion.InputAction.action.Disable();
        }

        private void Update()
        {
            if(!m_ControllerActive)
                return;

            bool mouseClicked = Mouse.current.leftButton.wasPressedThisFrame ||
                                Mouse.current.rightButton.wasPressedThisFrame;

            if(mouseClicked &&
               EventSystem.current != null &&
               UnityTools.IsPointerOverUI())
                m_GUIClick = true;

            if(mouseClicked)
                m_GUIClick = false;

            Vector2 input = m_MoveAction.action.ReadValue<Vector2>();
            m_RawInput = new Vector3(input.x, 0, input.y);

            bool aimState = false;

            if(m_AimType.HasFlag<AimType>(AimType.Button) && !m_GUIClick){
                aimState = m_FireAction.action.triggered;
            }

            if(m_AimType.HasFlag<AimType>(AimType.Axis) && !aimState){
                float aim = m_FireAction.action.ReadValue<float>();

                if(Mathf.Abs(aim) > 0.01f){
                    aimState = true;
                    m_RawInput.x = aim;
                }
            }

            if(m_AimType.HasFlag<AimType>(AimType.Toggle) && m_FireAction.action.triggered && !aimState)
                aimState = !IsAiming;

            if(m_AimType.HasFlag<AimType>(AimType.Selectable) && !aimState){
                aimState = SelectableObject.current != null;
            }

            IsAiming = aimState;

            foreach(MotionState motion in Motions.Where(motion => motion.isActiveAndEnabled &&
                                                                  (!motion.ConsumeInputOverUI || !m_GUIClick))){
                if(motion.StartType != StartType.Down && motion.StopType != StopType.Toggle ||
                   !motion.InputAction.action.WasPressedThisFrame()){
                    if(motion.StopType == StopType.Up && motion.InputAction.action.WasReleasedThisFrame()){
                        TryStopMotion(motion);
                        m_ToggleState[motion] = motion.StopType == StopType.Up && motion.IsActive;
                    }
                }
                else if(!motion.IsActive && motion.StartType == StartType.Down){
                    TryStartMotion(motion);
                    m_ToggleState[motion] =
                        motion.StopType is StopType.Toggle or StopType.Up && motion.IsActive;
                }
                else if(motion.StopType == StopType.Toggle){
                    TryStopMotion(motion);
                    m_ToggleState[motion] = motion.StopType == StopType.Toggle && motion.IsActive;
                    break;
                }

                if(motion.StartType == StartType.Press && motion.InputAction.action.WasReleasedThisFrame()){
                    TryStartMotion(motion);
                }
            }
        }

        private void FixedUpdate()
        {
            if(!m_ControllerActive){
                return;
            }

            foreach(MotionState motion in Motions.Where(motion => motion.isActiveAndEnabled)){
                switch(motion.IsActive){
                    case false when (motion.StartType == StartType.Automatic || m_ToggleState[motion]):
                        TryStartMotion(motion);
                        break;
                    case true:{
                        if(motion.StopType == StopType.Automatic && motion.CanStop()){
                            TryStopMotion(motion);
                        }

                        break;
                    }
                }
            }

            LookRotation = Quaternion.Euler(m_Transform.eulerAngles.x, m_CameraTransform.eulerAngles.y,
                                            m_Transform.eulerAngles.z);

            m_Velocity = m_Rigidbody.velocity;

            if(IsGrounded){
                m_Velocity.x = m_RootMotionForce.x;
                m_Velocity.z = m_RootMotionForce.z;
                float force = m_Animator.GetFloat("Force");
                m_Velocity += m_Transform.TransformDirection(RelativeInput * force);
            }

            CheckGround();
            CheckStep();
            UpdateVelocity();
            UpdateFrictionMaterial();
            UpdateRotation();
            UpdateAnimator();
            m_Rigidbody.velocity = m_Velocity;
        }

        private void OnAnimatorMove()
        {
            if(!m_ControllerActive)
                return;

            m_RootMotionForce = m_Animator.deltaPosition / Time.deltaTime;
        }

        public void UpdateVelocity()
        {
            if(Motions.Any(motion => motion.IsActive && !motion.UpdateVelocity(ref m_Velocity)))
                return;

            if(IsGrounded){
                m_Velocity.x /= (1 + m_GroundDampening);
                m_Velocity.z /= (1 + m_GroundDampening);
            }
            else{
                m_AirVelocity.y = 0f;
                m_AirVelocity += m_Transform.TransformDirection(Vector3.Scale(RelativeInput, m_AirSpeed)) -
                                 m_PrevAirVelocity;
                m_Velocity += m_AirVelocity;
                m_PrevAirVelocity = m_AirVelocity;
                m_Velocity.x /= 1f + m_AirDampening;
                m_Velocity.z /= 1f + m_AirDampening;
            }
        }

        private void UpdateAnimator()
        {
            if(Motions.Any(motion => motion.IsActive && !motion.UpdateAnimator()))
                return;

            m_Animator.SetFloat("Forward Input", RelativeInput.z * m_SpeedMultiplier, m_ForwardDampTime,
                                Time.deltaTime);
            m_Animator.SetFloat("Horizontal Input", RelativeInput.x * m_SpeedMultiplier,
                                m_HorizontalDampTime, Time.deltaTime);
            IsMoving = RelativeInput.sqrMagnitude > 0.01f;
            m_Animator.SetBool("Moving", IsMoving);
            if(m_IsAiming)
                m_Animator.SetFloat("Yaw Input",
                                    Normalize(GetSignedAngle(m_Transform.rotation, LookRotation, Vector3.up) * m_AimRotation,
                                              -180, 180), 0.15f, Time.deltaTime);
        }

        float Normalize(float input, float min, float max)
        {
            float average = (min + max) / 2;
            float range = (max - min) / 2;
            float normalized_x = (input - average) / range;
            return normalized_x;
        }

        private float GetSignedAngle(Quaternion a, Quaternion b, Vector3 axis)
        {
            Quaternion q = (b * Quaternion.Inverse(a));
            q.ToAngleAxis(out float angle, out Vector3 angleAxis);

            if(Vector3.Angle(axis, angleAxis) > 90f){
                angle = -angle;
            }

            return Mathf.DeltaAngle(0f, angle);
        }

        private void UpdateRotation()
        {
            if(Motions.Any(motion => motion.IsActive && !motion.UpdateRotation()))
                return;

            Quaternion rotation = m_Transform.rotation;

            if(IsAiming)
                rotation = LookRotation;
            else if(m_RawInput.sqrMagnitude > 0.01f)
                rotation = Quaternion.LookRotation(LookRotation * m_RawInput);

            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, rotation,
                                                    (IsAiming ? m_AimRotation : m_RotationSpeed) *
                                                    Time.fixedDeltaTime);
        }

        public void UpdateFrictionMaterial()
        {
            if(IsGrounded){
                if(IsStepping){
                    m_CapsuleCollider.material = StepFriction;
                }
                else if(IsMoving){
                    m_CapsuleCollider.material = MovementFriction;
                }
                else{
                    m_CapsuleCollider.material = IdleFriction;
                }
            }
            else{
                m_CapsuleCollider.material = AirFriction;
            }
        }

        public void DeterminanteDefaultStates()
        {
            for(int j = 0; j < m_Animator.layerCount; j++){
                AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(j);

                if(!stateInfo.IsTag("Default"))
                    continue;

                if(m_MotionStateMap.TryGetValue(j, out MotionState[] states))
                    if(Array.Exists(states, motion => stateInfo.IsName(motion.GetDestinationState())))
                        continue;

                m_LayerStateMap[j] = stateInfo;
            }
        }

        public void CheckDefaultAnimatorStates()
        {
            for(int j = 0; j < m_LayerStateMap.Length; j++){
                if(m_MotionStateMap[j].Length > 0){
                    bool active = false;

                    for(int k = 0; k < m_MotionStateMap[j].Length; k++){
                        if(m_MotionStateMap[j][k].IsActive){
                            active = true;
                        }
                    }

                    if(!active && m_Animator.GetCurrentAnimatorStateInfo(j).shortNameHash !=
                       m_LayerStateMap[j].shortNameHash && !m_Animator.IsInTransition(j)){
                        //Debug.Log("Current: "+this.m_Animator.GetCurrentAnimatorClipInfo(j)[0].clip.name);
                        m_Animator.CrossFadeInFixedTime(m_LayerStateMap[j].shortNameHash, 0.3f);
                        //this.m_Animator.Update(0f);
                        //Debug.Log("Next: " + this.m_Animator.GetNextAnimatorClipInfo(j)[0].clip.name);
                    }
                }
            }
        }

        public void CheckGround()
        {
            if(Motions.Any(motion => motion.IsActive && !motion.CheckGround()))
                return;

            if(Physics.SphereCast(m_Transform.position + m_Transform.up * (m_CapsuleCollider.radius * 2f),
                                  m_CapsuleCollider.radius, -m_Transform.up, out m_GroundHit,
                                  m_CapsuleCollider.radius * 2f + m_SkinWidth, m_GroundLayer)){
                if(!IsStepping && Physics.Raycast(m_Transform.position + m_CapsuleCollider.center,
                                                  -m_Transform.up, out m_GroundHit,
                                                  m_CapsuleCollider.height * 0.5f + m_SkinWidth * 0.9f,
                                                  m_GroundLayer, QueryTriggerInteraction.Ignore)){
                    m_Velocity = Vector3.ProjectOnPlane(m_Velocity, m_GroundHit.normal);
                }
                else{
                    m_Velocity.y = m_Velocity.y - 6f * Time.fixedDeltaTime;
                }

                IsGrounded = true;
            }
            else{
                IsGrounded = false;
            }
        }

        public void CheckStep()
        {
            if(Motions.Any(motion => motion.IsActive && !motion.CheckStep())){
                m_Slope = -1f;
                return;
            }

            Vector3 velocity = m_Velocity;
            velocity.y = 0f;

            if(RelativeInput.sqrMagnitude > velocity.sqrMagnitude){
                velocity = m_Transform.TransformDirection(RelativeInput);
            }

            bool prevSlope = m_Slope is > -1f or < -1f;
            m_Slope = -1f;
            IsStepping = false;

            if(velocity.sqrMagnitude > 0.001f && Physics.Raycast(m_Transform.position + m_Transform.up * 0.1f,
                                                                 velocity.normalized, out RaycastHit hitInfo,
                                                                 m_CapsuleCollider.radius + 0.2f,
                                                                 m_GroundLayer, QueryTriggerInteraction.Ignore)){
                float slope = Mathf.Acos(Mathf.Clamp(hitInfo.normal.y, -1f, 1f)) * Mathf.Rad2Deg;

                if(slope > m_SlopeLimit){
                    Vector3 direction = hitInfo.point - m_Transform.position;
                    direction.y = 0f;
                    Physics.Raycast((hitInfo.point + (Vector3.up * m_StepOffset)) + (direction.normalized * 0.1f),
                                    Vector3.down, out hitInfo, m_StepOffset + 0.1f, m_GroundLayer,
                                    QueryTriggerInteraction.Ignore);

                    if(Mathf.Acos(Mathf.Clamp(hitInfo.normal.y, -1f, 1f)) * Mathf.Rad2Deg > m_SlopeLimit){
                        m_Velocity.x *= m_GroundDampening;
                        m_Velocity.z *= m_GroundDampening;
                    }
                    else{
                        Vector3 position = m_Transform.position;
                        float y = position.y;
                        position.y = Mathf.MoveTowards(y, position.y + m_StepOffset, Time.deltaTime);
                        m_Transform.position = position;
                        m_Velocity.y = 0f;
                        IsStepping = true;
                    }
                }
                else{
                    m_Slope = slope;
                    m_Velocity.y = 0f;
                }
            }

            if(prevSlope && Math.Abs(m_Slope - (-1f)) < .1f)
                m_Velocity.y = 0f;
        }

        private static void TryStopMotion(MotionState motion)
        {
            if(motion.IsActive)
                motion.StopMotion(false);
        }

        private void TryStartMotion(MotionState motion)
        {
            const string interruptableTag = "Interruptable";

            if(!motion.IsActive && motion.CanStart() &&
               m_Animator.GetCurrentAnimatorStateInfo(1).IsTag(interruptableTag)){
                if(!string.IsNullOrEmpty(motion.GetDestinationState())){
                    for(int j = 0; j < Motions.Count; j++){
                        if(Motions[j].IsActive && Motions[j].Layer == motion.Layer &&
                           !string.IsNullOrEmpty(Motions[j].GetDestinationState())){
                            if(j > motion.Index){
                                Motions[j].StopMotion(true);
                            }
                            else{
                                return;
                            }
                        }
                    }
                }

                if(!string.IsNullOrEmpty(motion.State))
                    DeterminanteDefaultStates();
                motion.StartMotion();
            }
        }

        public void SetControllerActive(bool active)
        {
            if(m_ControllerActive != active){
                EventHandler.Execute(gameObject, "OnSetControllerActive", active);
            }

            m_ControllerActive = active;

            if(!m_ControllerActive){
                m_RawInput = Vector3.zero;
                m_Animator.SetFloat("Forward Input", 0f);
                m_Animator.SetFloat("Horizontal Input", 0f);
                m_Rigidbody.velocity = Vector3.zero;
            }

            enabled = active;
        }

        public void SetMotionEnabled(object[] data)
        {
            string motionName = (string)data[0];
            bool state = (bool)data[1];
            SetMotionEnabled(motionName, state);
        }

        public void SetMotionEnabled(string motionName, bool state)
        {
            MotionState motion = GetMotion(motionName);

            if(motion != null){
                motion.enabled = state;

                if(!state){
                    motion.StopMotion(true);
                }
            }
        }

        public MotionState GetMotion(string motionName)
            => Motions.FirstOrDefault(motion => motion.FriendlyName == motionName);

        private void Footsteps(AnimationEvent evt)
        {
            if(!m_Animator.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
                return;

            if(m_IsGrounded && m_Rigidbody.velocity.sqrMagnitude > 0.5f && m_FootstepClips.Count > 0 &&
               evt.animatorClipInfo.weight > 0.5f){
                float volume = evt.animatorClipInfo.weight;
                AudioClip clip = m_FootstepClips[Random.Range(0, m_FootstepClips.Count)];
                PlaySound(clip, volume);
            }
        }

        private void PlaySound(AudioClip clip, float volume)
        {
            if(clip == null){
                return;
            }

            if(m_AudioSource == null){
                m_AudioSource = gameObject.AddComponent<AudioSource>();
                m_AudioSource.outputAudioMixerGroup = m_AudioMixerGroup;
                m_AudioSource.spatialBlend = 1f;
            }

            if(m_AudioSource != null){
                m_AudioSource.PlayOneShot(clip, volume);
            }
        }

        public void CopyProperties(ThirdPersonController other)
        {
            m_DontDestroyOnLoad = other.m_DontDestroyOnLoad;
            m_MoveAction = other.m_MoveAction;
            m_SpeedMultiplier = other.m_SpeedMultiplier;
            m_AimType = other.m_AimType;
            m_FireAction = other.m_FireAction;
            m_AimRotation = other.m_AimRotation;
            m_RotationSpeed = other.m_RotationSpeed;
            m_AirSpeed = other.m_AirSpeed;
            m_AirDampening = other.m_AirDampening;
            m_GroundDampening = other.m_GroundDampening;
            m_StepOffset = other.m_StepOffset;
            m_SlopeLimit = other.m_SlopeLimit;
            m_GroundLayer = other.m_GroundLayer;
            m_SkinWidth = other.m_SkinWidth;
            IdleFriction = other.IdleFriction;
            MovementFriction = other.MovementFriction;
            StepFriction = other.StepFriction;
            AirFriction = other.AirFriction;
            m_ForwardDampTime = other.m_ForwardDampTime;
            m_HorizontalDampTime = other.m_HorizontalDampTime;
            Motions = new List<MotionState>();

            foreach(MotionState state in other.Motions){
                MotionState motion = CopyComponent(state, gameObject);
                Motions.Add(motion);
            }
        }

        T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            FieldInfo[] fields = type.GetAllSerializedFields();

            foreach(FieldInfo field in fields){
                if(field.IsPrivate && !field.HasAttribute<SerializeField>()){
                    continue;
                }

                field.SetValue(copy, field.GetValue(original));
            }

            return copy as T;
        }
    }

    [Flags]
    public enum AimType
    {
        Button = 1,
        Axis = 2,
        Toggle = 4,
        Selectable = 8
    }
}