using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public class AnimateRawImage : MonoBehaviour
    {
        public Vector2 animRate = new(1f, 0f);
        private RawImage image;

        private void Start()
        {
            image = GetComponent<RawImage>();
        }

        private void Update()
        {
            Rect rect = image.uvRect;
            rect.x += animRate.x * Time.deltaTime;
            rect.y += animRate.y * Time.deltaTime;
            image.uvRect = rect;
        }
    }
}