using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate")]
    public class Instantiate : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Self;
        [SerializeField] private readonly GameObject m_Original = null;
        [SerializeField] private readonly string m_BoneName = string.Empty;
        [SerializeField] private readonly Vector3 m_Offset = Vector3.zero;
        [SerializeField] private readonly bool m_IgnorePlayerCollision = true;

        private Transform m_Bone;

        public override void OnStart()
        {
            m_Bone = FindBone(GetTarget(m_Target).transform, m_BoneName);
            if(m_Bone == null)
                m_Bone = GetTarget(m_Target).transform;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Original == null){
                Debug.LogWarning("The game object you want to instantiate is null.");
                return ActionStatus.Failure;
            }

            GameObject go = Object.Instantiate(m_Original, m_Bone.position + m_Offset, m_Bone.rotation, m_Bone);

            if(m_IgnorePlayerCollision){
                UnityTools.IgnoreCollision(playerInfo.gameObject, go);
            }

            return ActionStatus.Success;
        }

        private Transform FindBone(Transform current, string name)
        {
            if(current.name == name)
                return current;

            for(int i = 0; i < current.childCount; ++i){
                Transform found = FindBone(current.GetChild(i), name);
                if(found != null)
                    return found;
            }

            return null;
        }
    }
}