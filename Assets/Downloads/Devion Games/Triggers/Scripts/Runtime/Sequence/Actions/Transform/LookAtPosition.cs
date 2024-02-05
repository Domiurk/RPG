using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look At Position")]
    public class LookAtPosition : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly Vector3 m_Position = Vector3.zero;
        [SerializeField] private readonly float m_Speed = 15f;

        private Quaternion m_LastRotation;
        private Quaternion m_DesiredRotation;
        private Transform m_Transform;

        public override void OnStart()
        {
            m_Transform = GetTarget(m_Target).transform;
            m_LastRotation = m_Transform.rotation;
            m_DesiredRotation = m_LastRotation;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 targetPosition = m_Position;
            Vector3 gameObjectPosition = m_Transform.position;
            targetPosition.y = gameObjectPosition.y;

            Vector3 dir = targetPosition - gameObjectPosition;

            if(dir.sqrMagnitude > 0f){
                m_DesiredRotation = Quaternion.LookRotation(dir);
            }

            m_LastRotation = Quaternion.Slerp(m_LastRotation, m_DesiredRotation, m_Speed * Time.deltaTime);
            m_Transform.rotation = m_LastRotation;
            return Quaternion.Angle(m_LastRotation, m_DesiredRotation) > 5
                       ? ActionStatus.Running
                       : ActionStatus.Success;
        }
    }
}