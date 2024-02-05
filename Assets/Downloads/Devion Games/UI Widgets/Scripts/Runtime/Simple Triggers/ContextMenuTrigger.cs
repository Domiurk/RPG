using UnityEngine;
using DevionGames.UIWidgets;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;
using UnityEngine.EventSystems;

public class ContextMenuTrigger : MonoBehaviour, IPointerDownHandler
{
    private ContextMenu m_ContextMenu;

    public string[] menu;

    private void Start()
    {
        m_ContextMenu = WidgetUtility.Find<ContextMenu>("ContextMenu");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            m_ContextMenu.Clear();
            for (int i = 0; i < menu.Length; i++)
            {
                string menuItem = menu[i];
                m_ContextMenu.AddMenuItem(menuItem, delegate { Debug.Log("Used - " + menuItem); });
            }
            m_ContextMenu.Show();
        }
    }
}
