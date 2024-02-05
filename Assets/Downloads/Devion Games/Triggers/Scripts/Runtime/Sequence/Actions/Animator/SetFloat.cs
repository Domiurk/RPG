using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Float")]
    public class SetFloat : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_ParameterName = "Forward Input";
        [SerializeField] private readonly float m_Value = 1f;
        [SerializeField] private readonly float m_DampTime = 0f;

        private Animator m_Animator;

        public override void OnSequenceStart()
        {
            m_Animator = m_Target == TargetType.Self
                             ? gameObject.GetComponentInChildren<Animator>()
                             : playerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_DampTime > 0f)
                return ActionStatus.Success;

            if(m_Animator == null){
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }

            m_Animator.SetFloat(m_ParameterName, m_Value);

            return ActionStatus.Success;
        }

        public override void Update()
        {
            if(m_DampTime <= 0f)
                return;

            if(m_Animator == null){
                Debug.LogWarning("Missing Component of type Animator!");
                return;
            }

            m_Animator.SetFloat(m_ParameterName, m_Value, m_DampTime, Time.deltaTime);
        }
    }
}