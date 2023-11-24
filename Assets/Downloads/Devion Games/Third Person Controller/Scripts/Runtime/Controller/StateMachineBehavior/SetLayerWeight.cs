using UnityEngine;

namespace DevionGames
{
    public class SetLayerWeight : StateMachineBehaviour
    {
        [SerializeField]
        private AnimationEventType m_Type = AnimationEventType.OnStateExit;
        [SerializeField]
        private int m_Layer = 1;
        [SerializeField]
        private float m_Weight;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_Type == AnimationEventType.OnStateEnter)
                animator.SetLayerWeight(m_Layer, m_Weight);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_Type == AnimationEventType.OnStateUpdate)
                animator.SetLayerWeight(m_Layer, m_Weight);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_Type == AnimationEventType.OnStateExit)
                animator.SetLayerWeight(m_Layer, m_Weight);
        }

        public enum AnimationEventType
        {
            OnStateEnter,
            OnStateUpdate,
            OnStateExit
        }
    }
}