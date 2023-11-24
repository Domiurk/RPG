using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Destroy")]
    public class Destroy : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Self;
        [SerializeField] private readonly float m_Delay = 0f;

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(m_Target);
            Object.Destroy(target, m_Delay);
            return ActionStatus.Success;
        }
    }
}