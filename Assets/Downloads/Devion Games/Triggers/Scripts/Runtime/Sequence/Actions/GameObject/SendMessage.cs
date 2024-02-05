using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/SendMessage")]
    public class SendMessage : Action
    {
        [SerializeField] private readonly TargetType m_Target = TargetType.Player;
        [SerializeField] private readonly string methodName = string.Empty;
        [SerializeField] private readonly ArgumentVariable m_Argument = null;
        [SerializeField] private readonly SendMessageOptions m_Options = SendMessageOptions.DontRequireReceiver;

        private GameObject m_TargetObject;

        public override void OnStart()
        {
            m_TargetObject = GetTarget(m_Target);
        }

        public override ActionStatus OnUpdate()
        {
            if(m_Argument.ArgumentType != ArgumentType.None)
                m_TargetObject.SendMessage(methodName, m_Argument.GetValue(), m_Options);
            else
                m_TargetObject.SendMessage(methodName, m_Options);

            return ActionStatus.Success;
        }
    }
}