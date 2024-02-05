using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public class HorizontalCompass : MonoBehaviour
    {
        public RawImage image;
        private Camera cameraMain;

        private void Start()
        {
            cameraMain = Camera.main;
        }

        private void Update()
        {
            image.uvRect = new Rect(cameraMain.transform.localEulerAngles.y / 360f, 0f, 1f, 1f);
        }
    }
}