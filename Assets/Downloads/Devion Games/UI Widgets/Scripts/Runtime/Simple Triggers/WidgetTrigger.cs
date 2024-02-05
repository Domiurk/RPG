using UnityEngine;

namespace DevionGames.UIWidgets
{
    public class WidgetTrigger : MonoBehaviour
    {
        public void Show(string nameWidget) {
           UIWidget widget = WidgetUtility.Find<UIWidget>(nameWidget);
            if (widget != null)
                widget.Show();
        }

        public void Close(string nameWidget)
        {
            UIWidget widget = WidgetUtility.Find<UIWidget>(nameWidget);
            if (widget != null)
                widget.Close();
        }
    }
}