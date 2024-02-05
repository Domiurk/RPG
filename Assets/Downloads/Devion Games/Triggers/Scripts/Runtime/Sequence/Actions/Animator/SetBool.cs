using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Bool")]
    public class SetBool : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_ParameterName = string.Empty;
        [SerializeField] private readonly bool m_Value = true;

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

            m_Animator.SetBool(m_ParameterName, m_Value);

            return ActionStatus.Success;
        }
    }
}