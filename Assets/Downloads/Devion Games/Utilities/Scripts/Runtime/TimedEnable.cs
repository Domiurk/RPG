using System.Collections;
using UnityEngine;

namespace DevionGames
{
    public class TimedEnable : MonoBehaviour
    {
        [SerializeField]
        private float m_Delay = 1f;
        [SerializeField]
        private Behaviour m_Component;
        [SerializeField]
        private bool m_Enable = true;

        private void OnEnable()
        {
            StartCoroutine(WaitAndSetEnabled());
        }

        private IEnumerator WaitAndSetEnabled() {
            yield return new WaitForSeconds(m_Delay);
            if (m_Component != null)
                m_Component.enabled = m_Enable;

            enabled = false;
        }
    }
}