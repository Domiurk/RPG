using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate At Mouse")]
    public class InstantiateAtMouse : Action
    {
        [SerializeField] private readonly GameObject m_Original = null;
        [SerializeField] private readonly bool m_IgnorePlayerCollision = true;

        private Camera cameraMain;

        public override void OnStart()
        {
            cameraMain = Camera.main;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Original == null){
                Debug.LogWarning("The game object you want to instantiate is null.");
                return ActionStatus.Failure;
            }

            if(Physics.Raycast(cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit,
                               100)){
                GameObject go = Object.Instantiate(m_Original, hit.point, Quaternion.identity);

                if(m_IgnorePlayerCollision)
                    UnityTools.IgnoreCollision(playerInfo.gameObject, go);

                return ActionStatus.Success;
            }

            return ActionStatus.Failure;
        }
    }
}