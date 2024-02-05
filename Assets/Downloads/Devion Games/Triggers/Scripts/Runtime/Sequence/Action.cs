using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public abstract class Action : IAction
    {
        [HideInInspector]
        [SerializeField] private string m_Type;
        [HideInInspector]
        [SerializeField] private bool m_Enabled = true;
        public bool enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public bool isActiveAndEnabled => enabled && gameObject.activeSelf;

        protected PlayerInfo playerInfo;
        protected GameObject gameObject;
        protected Blackboard blackboard;

        public Action()
        {
            m_Type = GetType().FullName;
        }

        public void Initialize(GameObject newGameObject, PlayerInfo newPlayerInfo, Blackboard newBlackboard)
        {
            this.gameObject = newGameObject;
            this.playerInfo = newPlayerInfo;
            this.blackboard = newBlackboard;
        }

        public abstract ActionStatus OnUpdate();

        public virtual void Update() { }

        public virtual void OnStart() { }

        public virtual void OnEnd() { }

        public virtual void OnSequenceStart() { }

        public virtual void OnSequenceEnd() { }

        public virtual void OnInterrupt() { }

        protected GameObject GetTarget(TargetType type)
        {
            return type switch{
                TargetType.Player => playerInfo.gameObject,
                TargetType.Camera => Camera.main.gameObject,
                _ => gameObject
            };
        }
    }

    public enum TargetType
    {
        Self,
        Player,
        Camera
    }
}