using UnityEngine;

namespace DevionGames.StatSystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Stat System/Remove Effect")]
    [System.Serializable]
    public class RemoveEffect : Action
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        protected StatEffect m_Effect;
      

        private StatsHandler m_Handler;

        public override void OnStart()
        {
            m_Handler = m_Target == TargetType.Self ? gameObject.GetComponent<StatsHandler>() : playerInfo.gameObject.GetComponent<StatsHandler>();
        }

        public override ActionStatus OnUpdate()
        {

            m_Handler.RemoveEffect(m_Effect);
            return ActionStatus.Success;
        }


    }
}
