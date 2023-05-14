using TMPro;
using UnityEngine;

namespace Runtime.TestUI
{
    public class UIInterface : MonoBehaviour
    {
        public static UIInterface Current;
        [SerializeField] private TMP_Text _text1;
        [SerializeField] private TMP_Text _text2;

        private void Start()
        {
            if(Current != null && Current != this)
                Destroy(this);
            else
                Current = this;
        }

        public void SetText(string text1 = "", string text2 = "")
        {
            if(_text1 != null && !string.IsNullOrEmpty(text1))
                _text1.text = text1;
            if(_text2 != null && !string.IsNullOrEmpty(text2))
                _text2.text = text2;
        }
    }
}