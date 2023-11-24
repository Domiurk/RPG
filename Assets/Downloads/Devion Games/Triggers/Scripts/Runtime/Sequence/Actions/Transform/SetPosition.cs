using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Set Position")]
    public class SetPosition : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly Vector3 m_Position = Vector3.zero;
        [SerializeField] private readonly bool m_SetCameraRelative = true;

        private Transform m_Transform;
        private Transform m_CameraTransform;

        public override void OnStart()
        {
            m_Transform = GetTarget(m_Target).transform;
            m_CameraTransform = Camera.main!.transform;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 dir = m_CameraTransform.position - m_Transform.position;

            m_Transform.position = m_Position;
            if(m_SetCameraRelative)
                m_CameraTransform.position = m_Position + dir;

            return ActionStatus.Success;
        }
    }
}