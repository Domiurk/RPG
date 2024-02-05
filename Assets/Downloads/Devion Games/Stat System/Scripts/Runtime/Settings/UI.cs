using DevionGames.UIWidgets;
using UnityEngine;
using UnityEngine.Assertions;

namespace DevionGames.StatSystem.Configuration
{
    [System.Serializable]
    public class UI : Settings
    {
        public override string Name => "UI";

        [InspectorLabel("Notification", "Name of Notification widget.")]
        public string notificationName = "Notification";
        [InspectorLabel("Dialog Box", "Name of the dialog box widget.")]
        public string dialogBoxName = "Dialog Box";


        private Notification m_Notification;
        public Notification notification
        {
            get
            {
                if (m_Notification == null)
                {
                    m_Notification = WidgetUtility.Find<Notification>(notificationName);
                    Debug.Log(m_Notification);
                }
                Assert.IsNotNull(m_Notification, "Notification widget with name " + notificationName + " is not present in scene.");
                return m_Notification;
            }
        }

        private DialogBox m_DialogBox;
        public DialogBox dialogBox
        {
            get
            {
                if (m_DialogBox == null)
                {
                    m_DialogBox = WidgetUtility.Find<DialogBox>(dialogBoxName);
                }
                Assert.IsNotNull(m_DialogBox, "DialogBox widget with name " + dialogBoxName + " is not present in scene.");
                return m_DialogBox;
            }
        }
    }
}