using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Is Name")]
    public class IsName : Action, ICondition
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly int layer = 0;
        [SerializeField] private readonly string name = string.Empty;

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

            AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(layer);

            return stateInfo.IsName(name) ? ActionStatus.Success : ActionStatus.Failure;
        }
    }
}