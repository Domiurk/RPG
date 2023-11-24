using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Rigidbody))]
    [ComponentMenu("Rigidbody/Set Constraints")]
    public class SetConstraints : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly RigidbodyConstraints m_Constraints = RigidbodyConstraints.FreezePosition;

        private Rigidbody m_Rigidbody;
        private RigidbodyConstraints m_CurrentConstraints;

        public override void OnStart()
        {
            m_Rigidbody = m_Target == TargetType.Self
                              ? gameObject.GetComponent<Rigidbody>()
                              : playerInfo.gameObject.GetComponent<Rigidbody>();
            if(m_Rigidbody != null)
                m_CurrentConstraints = m_Rigidbody.constraints;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Rigidbody == null){
                Debug.LogWarning("Missing Component of type Rigidbody!");
                return ActionStatus.Failure;
            }

            m_Rigidbody.constraints = m_Constraints;

            return ActionStatus.Success;
        }

        public override void OnInterrupt()
        {
            if(m_Rigidbody != null)
                m_Rigidbody.constraints = m_CurrentConstraints;
        }
    }
}