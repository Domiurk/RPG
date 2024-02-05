using System.Collections;
using UnityEngine;
using System.Linq;

namespace DevionGames.InventorySystem
{
    public class AreaOfEffect : MonoBehaviour
    {
        [SerializeField]
        private float m_StartDelay = 0.3f;
        [SerializeField]
        private float m_Radius = 3f;
        [SerializeField]
        private int m_Repeat = 1;
        [SerializeField]
        private float m_RepeatDelay = 1f;
        [SerializeField]
        private Object m_Data;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(m_StartDelay);
            for (int r = 0; r < m_Repeat; r++)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, m_Radius);
                colliders = colliders.Where(x => x.GetComponent($"DevionGames.StatSystem.StatsHandler") != null).ToArray();
                for (int i = 0; i < colliders.Length; i++)
                {
                    EventHandler.Execute(InventoryManager.current.PlayerInfo.gameObject, "SendDamage", colliders[i].gameObject, m_Data);
                }
                yield return new WaitForSeconds(m_RepeatDelay);
            }
            Destroy(gameObject, 0.1f);
        }


    }
}