﻿using UnityEngine;

namespace DevionGames
{
    public class AnimationEventSender : StateMachineBehaviour
    {
        [SerializeField] private AnimationEventType m_Type = AnimationEventType.OnStateExit;
        [SerializeField] private string m_EventName = "OnEndUse";
        [SerializeField] private ArgumentVariable m_Argument;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if(m_Type == AnimationEventType.OnStateMachineEnter)
                SendEvent(animator);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if(m_Type == AnimationEventType.OnStateMachineExit)
                SendEvent(animator);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(m_Type == AnimationEventType.OnStateEnter)
                SendEvent(animator);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(m_Type == AnimationEventType.OnStateUpdate)
                SendEvent(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(m_Type == AnimationEventType.OnStateExit)
                SendEvent(animator);
        }

        private void SendEvent(Animator animator)
        {
            if(m_Argument.ArgumentType != ArgumentType.None){
                animator.SendMessage(m_EventName, m_Argument.GetValue(), SendMessageOptions.DontRequireReceiver);
            }
            else{
                animator.SendMessage(m_EventName, SendMessageOptions.DontRequireReceiver);
            }
        }

        public enum AnimationEventType
        {
            OnStateEnter,
            OnStateUpdate,
            OnStateExit,
            OnStateMachineEnter,
            OnStateMachineExit
        }
    }
}