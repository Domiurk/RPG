using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DevionGames.UIWidgets
{
    public sealed class ContextMenu : UIWidget
    {
        [Header("Reference")]
        [SerializeField]
        private MenuItem m_MenuItemPrefab;
        private readonly List<MenuItem> itemCache = new();

        public override void Show()
        {
            m_RectTransform.position = Input.mousePosition;
            base.Show();
        }

        protected override void Update()
        {
            base.Update();

            if(m_CanvasGroup.alpha > 0f && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ||
                                            Input.GetMouseButtonDown(2))){
                var pointer = new PointerEventData(EventSystem.current){
                    position = Input.mousePosition
                };
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                foreach(RaycastResult result in raycastResults){
                    MenuItem item = result.gameObject.GetComponent<MenuItem>();

                    if(item != null){
                        item.OnPointerClick(pointer);
                        return;
                    }
                }

                Close();
            }
        }

        public void Clear()
        {
            foreach(MenuItem menu in itemCache)
                menu.gameObject.SetActive(false);
        }

        public MenuItem AddMenuItem(string text, UnityAction used)
        {
            MenuItem item = itemCache.Find(x => !x.gameObject.activeSelf);

            if(item == null){
                Debug.Log(text);
                item = Instantiate(m_MenuItemPrefab);
                itemCache.Add(item);
            }

            Text itemText = item.GetComponentInChildren<Text>();

            if(itemText != null){
                itemText.text = text;
            }

            item.onTrigger.RemoveAllListeners();
            item.gameObject.SetActive(true);
            item.transform.SetParent(m_RectTransform, false);
            item.onTrigger.AddListener(delegate
                                           {
                                               Close();

                                               used?.Invoke();
                                           });
            return item;
        }
    }
}