using UnityEngine;
using UnityEngine.AI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Move Towards")]
    public class MoveTowards : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly Vector3 m_Position = Vector3.zero;
        [SerializeField] private readonly float m_Speed = 1f;
        [SerializeField] private readonly bool m_LookAtPosition = true;
        [SerializeField] private readonly bool m_UsePath = false;

        private NavMeshPath m_Path;
        private int m_CurrentPathIndex;
        private Transform m_Transform;

        public override void OnStart()
        {
            m_Transform = GetTarget(m_Target).transform;

            if(m_UsePath){
                m_CurrentPathIndex = 0;
                m_Path = new NavMeshPath();
                NavMesh.CalculatePath(m_Transform.position, m_Position, NavMesh.AllAreas, m_Path);
            }
        }

        public override ActionStatus OnUpdate()
        {
            Transform transform = m_Transform;

            float step = m_Speed * Time.deltaTime;

            if(m_Path != null && m_Path.corners.Length > m_CurrentPathIndex){
                Vector3 nextPosition = m_Path.corners[m_CurrentPathIndex];
                LookAtPosition(nextPosition);
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);

                if(Vector3.Distance(nextPosition, transform.position) < 0.1f &&
                   m_Path.corners.Length > m_CurrentPathIndex){
                    m_CurrentPathIndex++;
                }

                for(int i = 0; i < m_Path.corners.Length - 1; i++){
                    Debug.DrawLine(m_Path.corners[i], m_Path.corners[i + 1], Color.red);
                }

                return ActionStatus.Running;
            }

            LookAtPosition(m_Position);
            transform.position = Vector3.MoveTowards(transform.position, m_Position, step);

            return Vector3.Distance(m_Position, transform.position) < 0.1f
                       ? ActionStatus.Success
                       : ActionStatus.Running;
        }

        private void LookAtPosition(Vector3 targetPosition)
        {
            if(!m_LookAtPosition){
                return;
            }

            targetPosition.y = m_Transform.position.y;
            Vector3 dir = targetPosition - m_Transform.position;

            if(dir.sqrMagnitude > 0f){
                Quaternion desiredRotation = Quaternion.LookRotation(dir);
                Quaternion lastRotation = m_Transform.rotation;

                lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, 5f * Time.deltaTime);
                m_Transform.rotation = lastRotation;
            }
        }
    }
}