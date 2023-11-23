using UnityEngine;

namespace DevionGames
{
    public sealed class SelectableObject : CallbackHandler, ISelectable
    {
        public static SelectableObject current;

        private Transform m_Transform;

        public Vector3 position => m_Transform != null ? m_Transform.position : Vector3.zero;

        public override string[] Callbacks => new[]{ "OnSelect", "OnDeselect" };

        private void Awake()
        {
            m_Transform = transform;
        }

        public void OnSelect()
        {
            current = this;
            Execute("OnSelect", new CallbackEventData());
        }

        public void OnDeselect()
        {
            Execute("OnDeselect", new CallbackEventData());
            current = null;
        }

        private void OnDestroy()
        {
            if(current == this)
                OnDeselect();
        }
    }
}