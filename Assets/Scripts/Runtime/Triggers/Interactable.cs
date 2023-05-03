using UnityEngine;
using Utilities.Runtime;

namespace Runtime.Triggers
{
    public class Interactable : MonoBehaviour
    {
        [Enum(typeof(KeyCode))] public KeyCode InteractKey = KeyCode.E;

        [SerializeField] private float _radius = 0.5f;

        private bool isRange;

        protected virtual void Init()
        { }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = _radius;
            sphereCollider.isTrigger = true;
        }

        private void Update()
        {
            if(!isRange)
                return;
            if(Input.GetKeyDown(InteractKey))
                Interact();
        }

        protected virtual void Interact()
        {
            Debug.Log("Interact");
        }

        protected virtual void Enter(Collider other)
        { }

        protected virtual void Exit(Collider other)
        { }

        private void OnTriggerEnter(Collider other)
        {
            isRange = true;
            Enter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            isRange = false;
            Exit(other);
        }
    }
}