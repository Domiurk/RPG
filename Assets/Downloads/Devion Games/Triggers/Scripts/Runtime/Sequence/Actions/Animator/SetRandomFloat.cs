using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Random Float")]
    public class SetRandomFloat : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_ParameterName = "Attack";
        [SerializeField] private readonly float m_DampTime = 0f;
        [SerializeField] private readonly float m_Min = 0;
        [SerializeField] private readonly float m_Max = 1;
        [SerializeField] private readonly bool m_RoundToInt = false;

        private Animator m_Animator;

        public override void OnStart()
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

            float random = Random.Range(m_Min, m_Max);

            if(m_RoundToInt){
                random = Mathf.Round(random);
            }

            m_Animator.SetFloat(m_ParameterName, random);

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

            float random = Random.Range(m_Min, m_Max);

            if(m_RoundToInt){
                random = Mathf.Round(random);
            }

            m_Animator.SetFloat(m_ParameterName, random, m_DampTime, Time.deltaTime);
        }
    }
}