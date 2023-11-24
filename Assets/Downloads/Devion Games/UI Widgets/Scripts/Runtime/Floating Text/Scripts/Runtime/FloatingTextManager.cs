using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.UIWidgets
{
    public class FloatingTextManager : MonoBehaviour
    {
        private static FloatingTextManager current;

        [SerializeField] private FloatingText m_Prefab;

        private static readonly Dictionary<GameObject, FloatingText> m_FloatingTexts = new();

        private void Awake()
        {
            if(current != null){
                Destroy(gameObject);
                return;
            }

            current = this;
        }

        public static void Add(GameObject target, string text, Color color, Vector3 offset)
        {
            if(!m_FloatingTexts.ContainsKey(target)){
                FloatingText floatingText = Instantiate(current.m_Prefab, current.transform);
                floatingText.SetText(target.transform, text, color, offset);
                floatingText.gameObject.SetActive(true);
                m_FloatingTexts.Add(target, floatingText);
            }
        }

        public static void Remove(GameObject target)
        {
            if(m_FloatingTexts.ContainsKey(target) && m_FloatingTexts[target] != null){
                Destroy(m_FloatingTexts[target].gameObject);
                m_FloatingTexts.Remove(target);
            }
        }
    }
}