using UnityEngine;

namespace DevionGames
{
    public class MoveTo : MonoBehaviour
    {
        [SerializeField]
        private string m_Tag = "Player";
        [SerializeField]
        private float speed = 3f;
        private Transform player;
        [SerializeField]
        private Vector3 m_Offset = Vector3.up;

        void Start()
        {
            GameObject go = GameObject.FindGameObjectWithTag(m_Tag);
            if(go != null)
                player = go.transform;

            transform.rotation = Random.rotation;
        }

        void Update()
        {
            if(player == null)
                return;

            Vector3 dir = (player.position + m_Offset) - transform.position;
            if(dir != Vector3.zero)
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);

            if(Vector3.Distance(transform.position, player.position + m_Offset) > 0.5f){
                transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
            }
            else{
                Destroy(gameObject);
            }
        }
    }
}