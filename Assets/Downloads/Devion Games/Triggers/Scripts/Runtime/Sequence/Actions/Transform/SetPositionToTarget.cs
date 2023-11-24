using System.Linq;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Set Position To Target")]
    public class SetPositionToTarget : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_Tag = "Respawn";
        [SerializeField] private readonly Vector3 m_DefaultPosition = Vector3.zero;
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

            GameObject[] respawn = GameObject.FindGameObjectsWithTag(m_Tag);
            Transform closestRespawn = GetClosest(m_Transform, respawn.Select(x => x.transform).ToArray());
            Vector3 position = m_DefaultPosition;

            if(closestRespawn != null){
                position = closestRespawn.position;
            }

            m_Transform.position = position;
            if(m_SetCameraRelative)
                m_CameraTransform.position = position + dir;

            return ActionStatus.Success;
        }

        Transform GetClosest(Transform self, Transform[] transforms)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = self.position;

            foreach(Transform potentialTarget in transforms){
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if(dSqrToTarget < closestDistanceSqr){
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
    }
}