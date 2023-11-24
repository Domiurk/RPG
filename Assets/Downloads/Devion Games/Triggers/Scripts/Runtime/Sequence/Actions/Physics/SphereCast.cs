using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/SphereCast")]
    public class SphereCast : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Camera;
        [SerializeField] private readonly Direction m_Direction = Direction.Forward;
        [SerializeField] private readonly float m_Radius = 1f;
        [SerializeField] private readonly float m_MaxDistance = 5f;
        [SerializeField] private readonly LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private readonly LayerMask m_HitSuccessLayer = Physics.DefaultRaycastLayers;
        [SerializeField] private readonly QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Collide;

        private Transform m_TargetTransform;

        public override void OnStart()
        {
            m_TargetTransform = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 startPosition = m_TargetTransform.position;
            Vector3 direction = PhysicsUtility.GetDirection(m_TargetTransform, m_Direction);

            if(Physics.SphereCast(startPosition + Vector3.up * 0.2f, m_Radius, direction, out RaycastHit hit,
                                  m_MaxDistance, m_LayerMask, m_QueryTriggerInteraction) &&
               m_HitSuccessLayer.Contains(hit.collider.gameObject.layer)){
                Debug.Log(hit.collider.name);
                return ActionStatus.Success;
            }

            return ActionStatus.Failure;
        }
    }
}