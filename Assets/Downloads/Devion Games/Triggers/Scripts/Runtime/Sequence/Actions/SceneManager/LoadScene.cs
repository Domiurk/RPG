using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("SceneManager/Load Scene")]
    public class LoadScene : Action
    {
        [SerializeField] private readonly string m_Scene = string.Empty;

        public override ActionStatus OnUpdate()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if(currentScene.name != m_Scene){
                SceneManager.LoadScene(m_Scene);
            }

            return ActionStatus.Success;
        }
    }
}