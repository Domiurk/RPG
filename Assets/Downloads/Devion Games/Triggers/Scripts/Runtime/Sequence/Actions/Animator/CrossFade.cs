using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/CrossFade")]
    public class CrossFade : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_AnimatorState = "Pickup";
        [SerializeField] private readonly float m_TransitionDuration = 0.2f;

        private Animator m_Animator;
        private int m_ShortNameHash;

        public override void OnStart()
        {
            m_ShortNameHash = Animator.StringToHash(m_AnimatorState);

            m_Animator = m_Target == TargetType.Self
                             ? gameObject.GetComponentInChildren<Animator>()
                             : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Animator == null){
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }

            m_Animator.CrossFadeInFixedTime(m_ShortNameHash, m_TransitionDuration);
            return ActionStatus.Success;
        }
    }
}