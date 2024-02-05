using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DevionGames
{
    public abstract class BaseTrigger : CallbackHandler
    {
        private const int TOLERANCE = 0;

        public abstract PlayerInfo PlayerInfo { get; }

        public override string[] Callbacks => new[]{
            "OnTriggerUsed",
            "OnTriggerUnUsed",
            "OnCameInRange",
            "OnWentOutOfRange",
        };

        public float useDistance = 1.2f;
        [EnumFlags]
        public TriggerInputType triggerType = TriggerInputType.LeftClick | TriggerInputType.Key;
        public InputActionReference key;

        protected ITriggerEventHandler[] m_TriggerEvents;
        public static BaseTrigger currentUsedTrigger;

        private static readonly List<BaseTrigger> m_TriggerInRange = new();
        protected Dictionary<Type, string> m_CallbackHandlers;

        protected delegate void EventFunction<T>(T handler, GameObject player);

        protected delegate void PointerEventFunction<T>(T handler, PointerEventData eventData);

        protected bool m_CheckBlocking = true;
        protected bool m_Started;

        private bool m_InRange;
        public bool InRange
        {
            get => m_InRange;
            protected set{
                if(m_InRange != value){
                    m_InRange = value;

                    if(m_InRange){
                        NotifyCameInRange();
                    }
                    else{
                        NotifyWentOutOfRange();
                    }
                }
            }
        }

        private bool m_InUse;
        public bool InUse
        {
            get => m_InUse;
            set{
                if(m_InUse != value){
                    m_InUse = value;

                    if(!m_InUse){
                        NotifyUnUsed();
                    }
                    else{
                        NotifyUsed();
                    }
                }
            }
        }

        protected virtual void Start()
        {
            RegisterCallbacks();
            m_TriggerEvents = GetComponentsInChildren<ITriggerEventHandler>();

            if(PlayerInfo.gameObject == null && useDistance != -1){
                useDistance = -1;
                Debug.LogWarning("There is no Player in scene! Please set Use Distance to -1 to ignore range check in " +
                                 gameObject + ".");
            }

            if(PlayerInfo.gameObject == null && triggerType.HasFlag<TriggerInputType>(TriggerInputType.OnTriggerEnter)){
                Debug.LogWarning("OnTriggerEnter is only valid with a Player in scene. Please remove OnTriggerEnter in " +
                                 gameObject + ".");
                triggerType = TriggerInputType.LeftClick;
            }

            EventHandler.Register<int>(gameObject, "OnPointerClickTrigger", OnPointerTriggerClick);

            if(gameObject == PlayerInfo.gameObject || useDistance == -1){
                InRange = true;
            }
            else{
                CreateTriggerCollider();
            }

            m_Started = true;
        }

        protected virtual void OnDisable()
        {
            key?.action.Disable();

            if(Time.frameCount > 0){
                InRange = false;
            }
        }

        protected virtual void OnEnable()
        {
            key?.action.Enable();
            if(Time.frameCount > 0 && m_Started && PlayerInfo.transform != null)
                InRange = Vector3.Distance(transform.position, PlayerInfo.transform.position) <= useDistance;
        }

        protected virtual void Update()
        {
            if(!InRange){
                return;
            }

            if(key != null && key.action.WasPressedThisFrame() &&
               triggerType.HasFlag<TriggerInputType>(TriggerInputType.Key) && InRange && IsBestTrigger()){
                Use();
            }
        }

        protected virtual void OnDestroy()
        {
            if(Time.frameCount > 0){
                InRange = false;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if(isActiveAndEnabled && PlayerInfo.gameObject != null && other.tag == PlayerInfo.gameObject.tag){
                InRange = true;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if(isActiveAndEnabled && PlayerInfo.gameObject != null && other.tag == PlayerInfo.gameObject.tag){
                InRange = false;
            }
        }

        private void OnPointerTriggerClick(int button)
        {
            if(!UnityTools.IsPointerOverUI() &&
               triggerType.HasFlag<TriggerInputType>(TriggerInputType.LeftClick) && button == 0 ||
               triggerType.HasFlag<TriggerInputType>(TriggerInputType.RightClick) && button == 1 ||
               triggerType.HasFlag<TriggerInputType>(TriggerInputType.MiddleClick) && button == 2){
                Use();
            }
        }

        public virtual bool Use()
        {
            if(!CanUse()){
                return false;
            }

            InUse = true;
            return true;
        }

        public virtual bool CanUse()
        {
            if(InUse || (currentUsedTrigger != null && currentUsedTrigger.InUse)){
                DisplayInUse();
                return false;
            }

            if(Math.Abs(useDistance - (-1)) < TOLERANCE){
                return true;
            }

            if(!InRange){
                DisplayOutOfRange();
                return false;
            }

            Animator animator = PlayerInfo.animator;

            if(PlayerInfo != null && animator != null){
                for(int j = 0; j < animator.layerCount; j++){
                    if(animator.IsInTransition(j))
                        return false;
                }
            }

            return true;
        }

        protected virtual void OnWentOutOfRange() { }

        protected void NotifyWentOutOfRange()
        {
            ExecuteEvent<ITriggerWentOutOfRange>(Execute, true);
            m_TriggerInRange.Remove(this);
            InUse = false;
            OnWentOutOfRange();
        }

        protected virtual void OnCameInRange() { }

        protected void NotifyCameInRange()
        {
            ExecuteEvent<ITriggerCameInRange>(Execute, true);
            m_TriggerInRange.Add(this);

            if(triggerType.HasFlag<TriggerInputType>(TriggerInputType.OnTriggerEnter) && IsBestTrigger()){
                m_CheckBlocking = false;
                Use();
                m_CheckBlocking = true;
            }

            OnCameInRange();
        }

        protected virtual void OnTriggerUsed() { }

        private void NotifyUsed()
        {
            currentUsedTrigger = this;
            ExecuteEvent<ITriggerUsedHandler>(Execute);
            OnTriggerUsed();
        }

        protected virtual void OnTriggerUnUsed() { }

        protected void NotifyUnUsed()
        {
            ExecuteEvent<ITriggerUnUsedHandler>(Execute, true);
            currentUsedTrigger = null;
            OnTriggerUnUsed();
        }

        protected virtual void DisplayInUse() { }

        protected virtual void DisplayOutOfRange() { }

        protected virtual void CreateTriggerCollider()
        {
            Vector3 position = Vector3.zero;
            GameObject handlerGameObject = new GameObject("TriggerRangeHandler");
            handlerGameObject.transform.SetParent(transform, false);
            handlerGameObject.layer = 2;

            Collider triggerCollider = GetComponent<Collider>();

            if(triggerCollider != null){
                position = triggerCollider.bounds.center;
                position.y = (triggerCollider.bounds.center - triggerCollider.bounds.extents).y;
                position = transform.InverseTransformPoint(position);
            }

            SphereCollider sphereCollider = handlerGameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = position;
            Vector3 scale = transform.lossyScale;
            sphereCollider.radius = useDistance / Mathf.Max(scale.x, scale.y, scale.z);

            Rigidbody rigidbodyComponent = GetComponent<Rigidbody>();

            if(rigidbodyComponent == null){
                rigidbodyComponent = gameObject.AddComponent<Rigidbody>();
                rigidbodyComponent.isKinematic = true;
            }
        }

        public virtual bool IsBestTrigger()
        {
            if(gameObject == PlayerInfo.gameObject){
                return true;
            }

            BaseTrigger tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = PlayerInfo.transform.position;

            foreach(BaseTrigger t in m_TriggerInRange){
                if(t.key != key)
                    continue;
                Vector3 dir = t.transform.position - currentPos;
                float angle = 0f;
                if(dir != Vector3.zero)
                    angle = Quaternion.Angle(PlayerInfo.transform.rotation, Quaternion.LookRotation(dir));

                float dist = Vector3.Distance(t.transform.position, currentPos) * angle;

                if(dist < minDist){
                    tMin = t;
                    minDist = dist;
                }
            }

            return tMin == this;
        }

        protected static void Execute(ITriggerUsedHandler handler, GameObject player)
        {
            handler.OnTriggerUsed(player);
        }

        protected static void Execute(ITriggerUnUsedHandler handler, GameObject player)
        {
            handler.OnTriggerUnUsed(player);
        }

        protected static void Execute(ITriggerCameInRange handler, GameObject player)
        {
            handler.OnCameInRange(player);
        }

        protected static void Execute(ITriggerWentOutOfRange handler, GameObject player)
        {
            handler.OnWentOutOfRange(player);
        }

        protected void ExecuteEvent<T>(EventFunction<T> func, bool includeDisabled = false)
            where T : ITriggerEventHandler
        {
            foreach(ITriggerEventHandler handler in m_TriggerEvents){
                if(ShouldSendEvent<T>(handler, includeDisabled)){
                    func.Invoke((T)handler, PlayerInfo.gameObject);
                }
            }

            if(m_CallbackHandlers.TryGetValue(typeof(T), out string eventID)){
                CallbackEventData triggerEventData = new CallbackEventData();
                triggerEventData.AddData("Trigger", this);
                triggerEventData.AddData("Player", PlayerInfo.gameObject);
                triggerEventData.AddData("EventData", new PointerEventData(EventSystem.current));
                base.Execute(eventID, triggerEventData);
            }
        }

        protected void ExecuteEvent<T>(PointerEventFunction<T> func,
                                       PointerEventData eventData,
                                       bool includeDisabled = false) where T : ITriggerEventHandler
        {
            foreach(ITriggerEventHandler handler in m_TriggerEvents){
                if(ShouldSendEvent<T>(handler, includeDisabled)){
                    func.Invoke((T)handler, eventData);
                }
            }

            if(m_CallbackHandlers.TryGetValue(typeof(T), out string eventID)){
                CallbackEventData triggerEventData = new CallbackEventData();
                triggerEventData.AddData("Trigger", this);
                triggerEventData.AddData("Player", PlayerInfo.gameObject);
                triggerEventData.AddData("EventData", new PointerEventData(EventSystem.current));
                base.Execute(eventID, triggerEventData);
            }
        }

        protected bool ShouldSendEvent<T>(ITriggerEventHandler handler, bool includeDisabled)
        {
            bool valid = handler is T;
            if(!valid)
                return false;
            var behaviour = handler as Behaviour;
            if(behaviour != null && !includeDisabled)
                return behaviour.isActiveAndEnabled;

            return true;
        }

        protected virtual void RegisterCallbacks()
        {
            m_CallbackHandlers = new Dictionary<Type, string>{
                { typeof(ITriggerUsedHandler), "OnTriggerUsed" },
                { typeof(ITriggerUnUsedHandler), "OnTriggerUnUsed" },
                { typeof(ITriggerCameInRange), "OnCameInRange" },
                { typeof(ITriggerWentOutOfRange), "OnWentOutOfRange" }
            };
        }

        [Flags]
        public enum TriggerInputType
        {
            LeftClick = 1,
            RightClick = 2,
            MiddleClick = 4,
            Key = 8,
            OnTriggerEnter = 16,
        }
    }
}