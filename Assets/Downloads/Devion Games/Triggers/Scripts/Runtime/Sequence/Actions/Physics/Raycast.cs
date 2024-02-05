using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Raycast")]
    public class Raycast : Action
    {
        [SerializeField] protected readonly Direction m_Direction = Direction.Forward;
        [SerializeField] protected readonly float m_MaxDistance = 15f;
        [SerializeField] protected readonly TargetType m_Target = TargetType.Camera;
        [SerializeField] protected Vector3 m_Offset = Vector3.zero;
        [SerializeField] protected LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] protected LayerMask m_HitLayer = Physics.DefaultRaycastLayers;
        [SerializeField] protected readonly QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Collide;

        protected Transform m_TargetTransform;

        public override void OnStart()
        {
            m_TargetTransform = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            return DoRaycast() ? ActionStatus.Success : ActionStatus.Failure;
        }

        protected virtual bool DoRaycast()
        {
            Vector3 startPosition = m_TargetTransform.position + m_TargetTransform.InverseTransformDirection(m_Offset);
            Vector3 direction = PhysicsUtility.GetDirection(m_TargetTransform, m_Direction);

            if(Physics.Raycast(startPosition, direction, out RaycastHit hit, m_MaxDistance, m_LayerMask,
                               m_QueryTriggerInteraction) && m_HitLayer.Contains(hit.collider.gameObject.layer)){
                return true;
            }

            return false;
        }
    }
}