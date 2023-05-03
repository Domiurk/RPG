using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Move : MonoBehaviour
    {
        private const string HORIZONTAL = "Horizontal";
        private const string VERTICAL = "Vertical";

        [SerializeField] private float _speed;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if(Input.GetAxis(VERTICAL) != 0 || Input.GetAxis(HORIZONTAL) != 0){
                float x = Input.GetAxis(HORIZONTAL) * _speed;
                float z = Input.GetAxis(VERTICAL) * _speed;
                _rigidbody.velocity = new Vector3(x, 0, z);
            }
        }
    }
}
