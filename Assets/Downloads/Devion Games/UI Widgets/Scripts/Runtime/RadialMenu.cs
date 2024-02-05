using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;

namespace DevionGames.UIWidgets
{
    public sealed class RadialMenu : UIWidget
    {
        [SerializeField] private float m_Radius = 100f;
        [SerializeField] private float m_Angle = 360f;
        
        [Header("Reference")]
        [SerializeField] private MenuItem m_Item;

        private readonly List<MenuItem> itemCache = new();
        private GameObject m_Target;

        protected override void Update()
        {
            base.Update();

            if(m_CanvasGroup.alpha > 0f && (Mouse.current.rightButton.isPressed ||
                                            Mouse.current.leftButton.isPressed ||
                                            Mouse.current.middleButton.isPressed)){
                var pointer = new PointerEventData(EventSystem.current){
                    position = Mouse.current.position.ReadValue()
                };
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                List<GameObject> results = raycastResults.Select(x => x.gameObject).ToList();

                if(results.Count > 0 && results.Contains(m_Target)){
                    results[0].SendMessage("Press", SendMessageOptions.DontRequireReceiver);
                }
                else
                    Close();
            }
        }

        public void Show(GameObject target, Sprite[] icons, UnityAction<int> result)
        {
            if(m_Target == target){
                Close();
                return;
            }

            m_Target = target;

            foreach(MenuItem menuItem in itemCache){
                menuItem.gameObject.SetActive(false);
            }

            Show();

            for(int i = 0; i < icons.Length; i++){
                int index = i;
                MenuItem item = AddMenuItem(icons[index]);
                float theta = Mathf.Deg2Rad * (m_Angle / icons.Length) * index;
                Vector3 position = new Vector3(Mathf.Sin(theta), Mathf.Cos(theta), 0);
                item.transform.localPosition = position * m_Radius;

                item.onTrigger.AddListener(delegate
                                               {
                                                   Close();

                                                   result?.Invoke(index);
                                               });
            }
        }

        public override void Close()
        {
            base.Close();
            m_Target = null;
        }

        public override void Show()
        {
            m_RectTransform.position = Input.mousePosition;
            base.Show();
        }

        private MenuItem AddMenuItem(Sprite icon)
        {
            MenuItem item = itemCache.Find(x => !x.isActiveAndEnabled);

            if(item == null){
                item = Instantiate(m_Item);
                itemCache.Add(item);
            }

            if(item.targetGraphic != null && item.targetGraphic is Image image){
                image.overrideSprite = icon;
            }

            item.onTrigger.RemoveAllListeners();
            item.gameObject.SetActive(true);
            item.transform.SetParent(m_RectTransform, false);
            return item;
        }
    }
}