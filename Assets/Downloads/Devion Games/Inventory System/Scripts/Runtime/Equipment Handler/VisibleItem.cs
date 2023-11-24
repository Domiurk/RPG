using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class VisibleItem : CallbackHandler
    {

        public override string[] Callbacks => new string[] { "OnEquip", "OnUnEquip" };


        [ItemPicker(true)]
        public Item item;

        [SerializeField]
        public Attachment[] attachments;

        protected Animator m_CharacterAnimator;
        protected Transform m_CameraTransform;
        protected Camera m_Camera;
        protected Collider[] m_CharacterColliders;
        protected EquipmentHandler m_Handler;
        protected Item m_CurrentEquipedItem;

        protected virtual void Start() {
          
        }

        protected virtual void Awake(){
            m_Handler = GetComponent<EquipmentHandler>();
            m_CharacterAnimator = GetComponentInParent<Animator>();
            m_CharacterColliders = GetComponentsInChildren<Collider>(true);
            m_Camera = Camera.main;
            m_CameraTransform = m_Camera.transform;
        }

        protected virtual void Update() {}

        public virtual void OnItemEquip(Item item) {
            enabled = true;
            m_CurrentEquipedItem = item;
            foreach (Attachment att in attachments)
            {
                if (att.gameObject != null)
                {
                    att.gameObject.SetActive(true);
                }else {
                    att.Instantiate(m_Handler);
                }
            }
            CallbackEventData data = new CallbackEventData();
            data.AddData("Item", item);
            data.AddData("Attachments", attachments);
            Execute("OnEquip", data);
        }

        public virtual void OnItemUnEquip(Item item) {
            m_CurrentEquipedItem = null;
            enabled = false;
            foreach (Attachment att in attachments)
            {
                if (att.gameObject != null)
                {
                    att.gameObject.SetActive(false);
                }
            }
            CallbackEventData data = new CallbackEventData();
            data.AddData("Item", item);
            data.AddData("Attachments", attachments);
            Execute("OnUnEquip", data);
        }

        protected void IgnoreCollision(GameObject gameObject) {
            Collider collider = gameObject.GetComponent<Collider>();
            for (int i = 0; i < m_CharacterColliders.Length; i++) {
                Physics.IgnoreCollision(m_CharacterColliders[i],collider);
            }
            collider.enabled = true;
        }

        [System.Serializable]
        public class Attachment {
            [EquipmentPicker(true)]
            public EquipmentRegion region;
            public GameObject prefab;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale = Vector3.one;

            [HideInInspector]
            public GameObject gameObject;

            public GameObject Instantiate(EquipmentHandler handler) {
                gameObject = Object.Instantiate(prefab, handler.GetBone(region));
                gameObject.SetActive(true);
                Trigger trigger = gameObject.GetComponent<Trigger>();
                if (trigger != null) {
                    Destroy(trigger);
                }
                IGenerator[] generators = gameObject.GetComponents<IGenerator>();
                for (int i = 0; i < generators.Length; i++) {
                    Destroy((generators[i] as Component));
                }
                ItemCollection collection = gameObject.GetComponent<ItemCollection>();
                if (collection != null){
                    Destroy(collection);
                }

                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody != null) {
                    Destroy(rigidbody);
                }
                Collider[] colliders = gameObject.GetComponents<Collider>();
                for (int i = 0; i < colliders.Length; i++) {
                    Destroy(colliders[i]);
                }
                gameObject.transform.localPosition = position;
                gameObject.transform.localEulerAngles = rotation;
                gameObject.transform.localScale = scale;
                return gameObject;
            }
        }
    }
}