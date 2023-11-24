using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Add Modifier")]
    [System.Serializable]
    public class RemoveModifier : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [InspectorLabel("Stat")]
        [SerializeField]
        protected string m_StatName="Critical Strike";

        private StatsHandler m_Handler;

        public override void OnStart()
        {
            m_Handler = m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {
            Stat stat = m_Handler.GetStat(m_StatName);
            if (stat == null) return ActionStatus.Failure;
            stat.RemoveModifiersFromSource(m_Handler.gameObject);
            return ActionStatus.Success;
        }


    }
}
