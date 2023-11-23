using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames.UIWidgets
{
    public class WidgetInputHandler : MonoBehaviour
    {
        private static Dictionary<Key, List<UIWidget>> m_WidgetKeyBindings;
        private Keyboard keyboard;

        private void Start()
        {
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            if(m_WidgetKeyBindings == null){
                return;
            }

            foreach(KeyValuePair<Key, List<UIWidget>> keyBinding in m_WidgetKeyBindings){
                if(keyboard[keyBinding.Key].wasPressedThisFrame){
                    foreach(UIWidget widget in keyBinding.Value){
                        widget.Toggle();
                    }
                }
            }
        }

        public static void RegisterInput(Key key, UIWidget widget)
        {
            if(m_WidgetKeyBindings == null){
                WidgetInputHandler handler = GameObject.FindObjectOfType<WidgetInputHandler>();

                if(handler == null){
                    GameObject handlerObject = new GameObject("WidgetInputHandler");
                    handlerObject.AddComponent<WidgetInputHandler>();
                    handlerObject.AddComponent<SingleInstance>();
                }

                m_WidgetKeyBindings = new Dictionary<Key, List<UIWidget>>();
            }

            if(key == Key.None)
                return;

            if(!m_WidgetKeyBindings.TryGetValue(key, out List<UIWidget> widgets)){
                m_WidgetKeyBindings.Add(key, new List<UIWidget>(){ widget });
            }
            else{
                widgets.RemoveAll(x => x == null);
                widgets.Add(widget);
                m_WidgetKeyBindings[key] = widgets;
            }
        }

        public static void UnregisterInput(Key key, UIWidget widget)
        {
            if(m_WidgetKeyBindings == null)
                return;

            if(m_WidgetKeyBindings.TryGetValue(key, out List<UIWidget> widgets))
                widgets.Remove(widget);
        }
    }
}