using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("UI/Yes No Dialog")]
    [System.Serializable]
    public class YesNoDialog : Action
    {
        [SerializeField]
        private string m_WidgetName = "Dialog Box";
        [SerializeField]
        private string m_Title = "Are you sure?";
        [SerializeField]
        private string m_Text = string.Empty;
        [SerializeField]
        private Sprite m_Icon;

        private ActionStatus m_Status;
        private DialogBox m_DialogBox;

        public override void OnStart()
        {
            m_DialogBox = WidgetUtility.Find<DialogBox>(m_WidgetName);
            if (m_DialogBox == null)
            {
                Debug.LogWarning("Missing dialog box widget " + m_WidgetName + " in scene!");
                return;
            }
            m_DialogBox.RegisterListener("OnClose", OnClose);
            m_Status = ActionStatus.Running;
            m_DialogBox.Show(m_Title,m_Text,m_Icon,OnResponse,"Yes","No");
        }

        public override ActionStatus OnUpdate()
        {
            return m_Status;
        }

        private void OnClose(CallbackEventData ev) {
            m_Status = ActionStatus.Failure;
            m_DialogBox.RemoveListener("OnClose", OnClose);
        }

        private void OnResponse(int result) {
            if (result == 0){
                m_Status = ActionStatus.Success;
            }else {
                m_Status = ActionStatus.Failure;
            }
        }
    }
}