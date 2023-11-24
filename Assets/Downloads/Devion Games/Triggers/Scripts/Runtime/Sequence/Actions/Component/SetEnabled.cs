using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Component")]
    [ComponentMenu("Component/Set Enabled")]
    public class SetEnabled : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string m_ComponentName = string.Empty;
        [SerializeField] private readonly bool m_Enable = false;

        private Behaviour m_Component;
        private bool m_IsEnabled;

        public override void OnStart()
        {
            GameObject target = GetTarget(m_Target);
            m_Component = target.GetComponent(m_ComponentName) as Behaviour;
            if(m_Component != null)
                m_IsEnabled = m_Component.enabled;
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Component == null){
                Debug.LogWarning("Missing Component of type " + m_ComponentName + "!");
                return ActionStatus.Failure;
            }

            m_Component.enabled = m_Enable;
            return ActionStatus.Success;
        }

        public override void OnInterrupt()
        {
            if(m_Component != null)
                m_Component.enabled = m_IsEnabled;
        }
    }
}