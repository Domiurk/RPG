using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Compare Tag")]
    public class CompareTag : Action, ICondition
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Self;
        [SerializeField] private readonly string m_Tag = "Player";

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(m_Target);
            return target.CompareTag(m_Tag) ? ActionStatus.Success : ActionStatus.Failure;
        }
    }
}