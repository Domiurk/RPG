using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/ApplyRootMotion")]
    public class ApplyRootMotion : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly bool m_Apply = false;

        private Animator m_Animator;

        public override void OnStart()
        {
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

            m_Animator.applyRootMotion = m_Apply;

            return ActionStatus.Success;
        }
    }
}