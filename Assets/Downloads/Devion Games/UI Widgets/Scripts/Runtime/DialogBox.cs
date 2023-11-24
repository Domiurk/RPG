using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public sealed class DialogBox : UIWidget
    {
        /// <summary>
        /// Closes the window when a button is clicked.
        /// </summary>
        public bool autoClose = true;
        /// <summary>
        /// The title component reference
        /// </summary>
        [Header("Reference")]
        public Text Title;
        /// <summary>
        /// The text component reference
        /// </summary>
        public Text Text;
        /// <summary>
        /// The icon sprite reference
        /// </summary>
        public Image Icon;
        /// <summary>
        /// The button prefab reference
        /// </summary>
        public Button Button;

        private readonly List<Button> buttonCache = new();
        private GameObject m_IconParent;

        protected override void OnAwake()
        {
            base.OnAwake();
            if(Icon != null)
                m_IconParent = Icon.GetComponentInParent<LayoutElement>().gameObject;
        }

        public void Show(NotificationOptions settings, UnityAction<int> result, params string[] buttons)
        {
            Show(settings.title, WidgetUtility.ColorString(settings.text, settings.color), settings.icon, result,
                 buttons);
        }

        /// <summary>
        /// Show the MessageBox
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="buttons">Buttons.</param>
        public void Show(string title, string text, params string[] buttons)
        {
            Show(title, text, null, null, buttons);
        }

        /// <summary>
        /// Show the MessageBox
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="result">Result.</param>
        /// <param name="buttons">Buttons.</param>
        public void Show(string title, string text, UnityAction<int> result, params string[] buttons)
        {
            Show(title, text, null, result, buttons);
        }

        /// <summary>
        /// Show the MessageBox
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="text">Text.</param>
        /// <param name="icon">Icon.</param>
        /// <param name="result">Result.</param>
        /// <param name="buttons">Buttons.</param>
        public void Show(string title,
                         string text,
                         Sprite icon,
                         UnityAction<int> result,
                         params string[] buttons)
        {
            foreach(Button button in buttonCache){
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }

            if(Title != null){
                if(!string.IsNullOrEmpty(title)){
                    Title.text = title;
                    Title.gameObject.SetActive(true);
                }
                else{
                    Title.gameObject.SetActive(false);
                }
            }

            if(Text != null){
                Text.text = text;
            }

            if(Icon != null){
                if(icon != null){
                    Icon.overrideSprite = icon;
                    m_IconParent.SetActive(true);
                }
                else{
                    m_IconParent.SetActive(false);
                }
            }

            base.Show();
            Button.gameObject.SetActive(false);

            for(int i = 0; i < buttons.Length; i++){
                string caption = buttons[i];
                int index = i;
                AddButton(caption)
                    .onClick.AddListener(delegate
                                             {
                                                 if(autoClose)
                                                     Close();
                                                 result?.Invoke(index);
                                             });
            }
        }

        private Button AddButton(string text)
        {
            Button mButton = buttonCache.Find(x => !x.isActiveAndEnabled);

            if(mButton == null){
                mButton = Instantiate(Button);
                buttonCache.Add(mButton);
            }

            mButton.gameObject.SetActive(true);
            mButton.onClick.RemoveAllListeners();
            mButton.transform.SetParent(Button.transform.parent, false);
            Text[] buttonTexts = mButton.GetComponentsInChildren<Text>(true);

            if(buttonTexts.Length > 0){
                buttonTexts[0].text = text;
            }

            return mButton;
        }
    }
}