using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public class FloatingText : MonoBehaviour
    {
        private Transform m_Target;
        private Vector3 m_Offset;
        private Text m_Text;
        private Camera cameraMain;

        private void Awake()
        {
            cameraMain = Camera.main;
            m_Text = GetComponent<Text>();
        }

        private void LateUpdate()
        {
            Vector3 pos = UnityTools.GetBounds(m_Target.gameObject).center + m_Offset;
            Vector3 screenPos = cameraMain.WorldToScreenPoint(pos);
            m_Text.enabled = screenPos.x > 0 && screenPos.x < cameraMain.pixelWidth && screenPos.y > 0 &&
                             screenPos.y < cameraMain.pixelHeight && screenPos.z > 0;
            transform.position =
                cameraMain.WorldToScreenPoint(UnityTools.GetBounds(m_Target.gameObject).center + m_Offset);
        }

        public void SetText(Transform target, string text, Color color, Vector3 offset)
        {
            m_Target = target;
            m_Offset = offset;
            Text component = GetComponent<Text>();
            component.text = text;
            component.color = color;
        }
    }
}