using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public sealed class Progressbar : UIWidget
    {
        [Header("Reference")]
        [SerializeField] private Image progressbar;
        [SerializeField] private Text m_ProgressbarTitle;
        [SerializeField] private Text progressLabel;
        [SerializeField] private string format = "F0";

        protected override void OnStart()
        {
            progressbar.type = Image.Type.Filled;
        }

        public void SetProgress(float progress)
        {
            progressbar.fillAmount = progress;

            if(progressLabel != null){
                progressLabel.text = (progress * 100f).ToString(format) + "%";
            }
        }

        public override void Show()
        {
            Show("");
        }

        public void Show(string title)
        {
            if(m_ProgressbarTitle != null){
                m_ProgressbarTitle.text = title;
            }

            progressbar.fillAmount = 0f;

            if(progressLabel != null){
                progressLabel.text = "0%";
            }

            base.Show();
        }
    }
}