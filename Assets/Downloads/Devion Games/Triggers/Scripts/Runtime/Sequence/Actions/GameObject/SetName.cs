using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Set Name")]
    public class SetName : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Self;
        [InspectorLabel("Name")]
        [SerializeField] private readonly string m_Value = string.Empty;

        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(m_Target);
            target.name = m_Value;
            return ActionStatus.Success;
        }
    }
}