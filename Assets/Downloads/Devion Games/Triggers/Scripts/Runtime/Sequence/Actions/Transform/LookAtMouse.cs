using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look At Mouse")]
    public class LookAtMouse : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly float m_MaxDistance = 100f;
        [SerializeField] private readonly float m_Speed = 15f;

        private Quaternion m_LastRotation;
        private Quaternion m_DesiredRotation;
        private Transform m_Transform;

        public override void OnStart()
        {
            m_Transform = GetTarget(m_Target).transform;
            m_LastRotation = m_Transform.rotation;
            m_DesiredRotation = m_LastRotation;

            if(Physics.Raycast(Camera.main!.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, m_MaxDistance)){
                Vector3 targetPosition = hit.point;
                Vector3 position = m_Transform.position;
                targetPosition.y = position.y;

                Vector3 dir = targetPosition - position;

                if(dir.sqrMagnitude > 0f){
                    m_DesiredRotation = Quaternion.LookRotation(dir);
                }
            }
        }

        public override ActionStatus OnUpdate()
        {
            m_LastRotation = Quaternion.Slerp(m_LastRotation, m_DesiredRotation, m_Speed * Time.deltaTime);
            playerInfo.transform.rotation = m_LastRotation;
            return Quaternion.Angle(m_LastRotation, m_DesiredRotation) > 5f
                       ? ActionStatus.Running
                       : ActionStatus.Success;
        }
    }
}