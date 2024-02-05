using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
    public class BehaviorTrigger : BaseTrigger
    {
        public ActionTemplate actionTemplate;

        [SerializeReference] public List<Action> actions = new();
        [SerializeField] protected bool m_Interruptable;

        private Sequence m_ActionBehavior;
        private PlayerInfo m_PlayerInfo;
        protected AnimatorStateInfo[] m_LayerStateMap;

        public override PlayerInfo PlayerInfo => m_PlayerInfo ??= new PlayerInfo("Player");

        protected override void Start()
        {
            base.Start();
            List<ITriggerEventHandler> list = new List<ITriggerEventHandler>(m_TriggerEvents);
            list.AddRange(actions.Where(x => x is ITriggerEventHandler).Cast<ITriggerEventHandler>());
            m_TriggerEvents = list.ToArray();
            if(actionTemplate != null)
                actionTemplate = Instantiate(actionTemplate);
            m_ActionBehavior = new Sequence(gameObject, PlayerInfo, GetComponent<Blackboard>(),
                                            actionTemplate != null
                                                ? actionTemplate.actions.Cast<IAction>().ToArray()
                                                : actions.Cast<IAction>().ToArray());
        }

        protected override void Update()
        {
            if(!InRange)
                return;

            if(key != null && key.action.triggered && triggerType.HasFlag<TriggerInputType>(TriggerInputType.Key) &&
               InRange &&
               IsBestTrigger())
                Use();

            //TODO : check if this is the best way to do this
            if(m_Interruptable && InUse && (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f ||
                                            Mathf.Abs(Input.GetAxis("Vertical")) > 0.5f)){
                NotifyInterrupted();
                m_ActionBehavior.Interrupt();
                return;
            }

            InUse = m_ActionBehavior.Tick();
        }

        protected override void OnDisable()
        {
            if(Time.frameCount > 0){
                if(m_Interruptable && InUse){
                    NotifyInterrupted();
                    m_ActionBehavior.Interrupt();
                }

                InRange = false;
            }
        }

        protected override void OnDestroy()
        {
            if(Time.frameCount > 0){
                if(m_Interruptable && InUse){
                    NotifyInterrupted();
                    m_ActionBehavior.Interrupt();
                }

                InRange = false;
            }
        }

        protected void NotifyInterrupted()
        {
            InUse = false;
            OnTriggerInterrupted();
        }

        protected virtual void OnTriggerInterrupted() { }

        protected override void OnTriggerUsed()
        {
            CacheAnimatorStates();
        }

        protected override void OnTriggerUnUsed()
        {
            m_ActionBehavior.Stop();
            LoadCachedAnimatorStates();
        }

        public override bool Use()
        {
            if(!CanUse()){
                return false;
            }

            InUse = true;
            m_ActionBehavior.Start();
            return true;
        }

        protected void CacheAnimatorStates()
        {
            if(PlayerInfo == null)
                return;

            Animator animator = PlayerInfo.animator;

            if(animator != null){
                m_LayerStateMap = new AnimatorStateInfo[animator.layerCount];

                for(int j = 0; j < animator.layerCount; j++){
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(j);
                    m_LayerStateMap[j] = stateInfo;
                }
            }
        }

        protected void LoadCachedAnimatorStates()
        {
            if(PlayerInfo == null)
                return;

            Animator animator = PlayerInfo.animator;

            if(animator != null){
                for(int j = 0; j < m_LayerStateMap.Length; j++){
                    if(animator.GetCurrentAnimatorStateInfo(j).shortNameHash != m_LayerStateMap[j].shortNameHash &&
                       !animator.IsInTransition(j)){
                        animator.CrossFadeInFixedTime(m_LayerStateMap[j].shortNameHash, 0.15f);
                    }
                }
            }
        }
    }
}