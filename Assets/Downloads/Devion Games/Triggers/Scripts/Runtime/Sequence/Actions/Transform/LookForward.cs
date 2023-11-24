using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look Forward")]
    public class LookForward : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        private Transform m_Transform;
        private Transform m_CameraTransform;

        public override void OnStart()
        {
            m_CameraTransform = Camera.main!.transform;
            m_Transform = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            Quaternion lookRotation = Quaternion.Euler(m_Transform.eulerAngles.x, m_CameraTransform.eulerAngles.y,
                                                       m_Transform.eulerAngles.z);

            if(SelectableObject.current != null){
                Vector3 direction = SelectableObject.current.transform.position - m_Transform.position;
                direction.y = 0f;
                lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            m_Transform.rotation = lookRotation;

            return ActionStatus.Success;
        }
    }
}