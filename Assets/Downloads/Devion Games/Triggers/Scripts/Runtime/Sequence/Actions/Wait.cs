using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Time/Wait")]
    [Icon("Time")]
    public class Wait : Action
    {
        [SerializeField] private readonly float duration = 1f;

        private float m_Time;

        public override void OnStart()
        {
            m_Time = 0f;
        }

        public override ActionStatus OnUpdate()
        {
            m_Time += Time.deltaTime;
            return m_Time > duration ? ActionStatus.Success : ActionStatus.Running;
        }
    }
}