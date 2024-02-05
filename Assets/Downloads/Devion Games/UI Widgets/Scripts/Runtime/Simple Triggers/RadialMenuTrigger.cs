using UnityEngine;
using DevionGames.UIWidgets;
using UnityEngine.EventSystems;

public class RadialMenuTrigger : MonoBehaviour, IPointerDownHandler
{
    public Sprite[] menuIcons;

    private RadialMenu m_RadialMenu;

    private void Start()
    {
        m_RadialMenu = WidgetUtility.Find<RadialMenu>("RadialMenu");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_RadialMenu.Show(gameObject, menuIcons, index => { Debug.Log($"Used index{index}"); });
    }
}