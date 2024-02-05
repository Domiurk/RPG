using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GraphicRaycaster))]
    [ComponentMenu("Physics/Overlap Sphere")]
    public class OverlapSphere : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Camera;
        [SerializeField] private readonly float m_Radius = 1f;
        [SerializeField] private readonly LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private readonly LayerMask m_HitSuccessLayer = Physics.DefaultRaycastLayers;
        [SerializeField]
        private readonly QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Collide;

        private Transform m_TargetTransform;

        public override void OnStart()
        {
            m_TargetTransform = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 startPosition = m_TargetTransform.position;

            Collider[] colliders =
                Physics.OverlapSphere(startPosition, m_Radius, m_LayerMask, m_QueryTriggerInteraction);

            return colliders.Any(t => m_HitSuccessLayer.Contains(t.gameObject.layer))
                       ? ActionStatus.Success
                       : ActionStatus.Failure;
        }
    }
}