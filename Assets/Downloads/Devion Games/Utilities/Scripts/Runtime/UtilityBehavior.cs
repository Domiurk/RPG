using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DevionGames
{
    public class UtilityBehavior : MonoBehaviour
    {
        public void QuitApplication() {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void LoadScene(string scene) {
            SceneManager.LoadScene(scene);
        }

        public void Instantiate(GameObject gameObject) {
            Instantiate(gameObject, transform.position, Quaternion.identity);
        }
    }
}