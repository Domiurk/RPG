using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/IsInTransition")]
    public class IsInTransition : Action, ICondition
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly int layer = 0;
        [SerializeField] private readonly bool invertResult=false;
        private Animator m_Animator;

        public override void OnStart()
        {
            m_Animator = m_Target == TargetType.Self ? gameObject.GetComponentInChildren<Animator>() : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            ActionStatus status = m_Animator.IsInTransition(layer) ? ActionStatus.Success : ActionStatus.Failure;

            if (invertResult) {
                return status == ActionStatus.Success ? ActionStatus.Failure : ActionStatus.Success;
            }
            return status;
        }
    }
}
