using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.UIWidgets
{
    public sealed class Chat : UIWidget
    {
        [Header("Behaviour")]
        [SerializeField] private string filterMask = "*";
        [TextArea(1, 5)]
        [SerializeField] private string filter = "fuck, ass, piss, cunt, shit";

        [Header("Reference")]
        [SerializeField] private Text text;
        [SerializeField] private InputField input;
        [SerializeField] private Button submit;

        private string[] filterWords;

        protected override void OnStart()
        {
            base.OnStart();
            input.onEndEdit.AddListener(Submit);

            if(submit != null){
                submit.onClick.AddListener(delegate { Submit(input.text); });
            }

            filterWords = filter.Replace(" ", "").Split(',');
        }

        private void Submit(string textSubmit)
        {
            if(!string.IsNullOrEmpty(textSubmit)){
                textSubmit = ApplyFilter(textSubmit);
                OnSubmit(textSubmit);
            }

            input.text = "";
        }

        private void OnSubmit(string textSubmit)
        {
            text.text += "\n" + textSubmit;
        }

        private string ApplyFilter(string textSubmit)
        {
            return filterWords.Aggregate(textSubmit,
                                         (current, _)
                                             => current.Replace(filter,
                                                                new System.Text.StringBuilder()
                                                                    .Insert(0, filterMask, filter.Length)
                                                                    .ToString()));
        }
    }
}