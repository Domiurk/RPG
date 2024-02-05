using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DevionGames.UIWidgets
{
	/// <summary>
	/// Tooltip trigger to display fixed tooltips
	/// </summary>
	public class TooltipTrigger : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
	{

		/// <summary>
		/// The name of the tooltip instance.
		/// </summary>
		[SerializeField]
		private string instanceName = "Tooltip";
		/// <summary>
		/// Show the background element
		/// </summary>
		[SerializeField]
		private bool showBackground=true;
		[SerializeField]
		private float width = 300;
		/// <summary>
		/// Color of the text.
		/// </summary>
		[SerializeField]
		private Color color = Color.white;
		///<summary>
        /// The title to display
		/// </summary>
        public string tooltipTitle;
        /// <summary>
        /// The text to display
        /// </summary>
        [TextArea]
		public string tooltip;
		/// <summary>
		/// Optionally show an icon
		/// </summary>
		public Sprite icon;

        public StringPair[] properties;

		private Tooltip instance;
        private Coroutine m_DelayTooltipCoroutine;
        private List<KeyValuePair<string, string>> m_PropertyPairs;

		/// <summary>
		/// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		/// </summary>
		private void Start ()
		{
			instance = WidgetUtility.Find<Tooltip> (instanceName);
			if (enabled && instance == null)
				enabled = false;
			m_PropertyPairs = new List<KeyValuePair<string, string>>();
            foreach(StringPair property in properties){
	            m_PropertyPairs.Add(new KeyValuePair<string, string>(property.key,property.value));
            }
		}

		/// <summary>
		/// Called when the mouse pointer starts hovering the ui element.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter (PointerEventData eventData)
		{
			ShowTooltip();
		}

		/// <summary>
		/// Called when the mouse pointer exits the element
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit (PointerEventData eventData)
		{
			CloseTooltip();
		}

        private IEnumerator DelayTooltip(float delay)
        {
            float time = 0.0f;
            yield return true;
            while (time < delay)
            {
                time += Time.deltaTime;
                yield return true;
            }

   
            instance.Show(WidgetUtility.ColorString(tooltipTitle, color), WidgetUtility.ColorString(tooltip, color), icon, m_PropertyPairs, width, showBackground);
        }

        private void ShowTooltip()
        {

            if (m_DelayTooltipCoroutine != null)
            {
                StopCoroutine(m_DelayTooltipCoroutine);
            }
            m_DelayTooltipCoroutine = StartCoroutine(DelayTooltip(0.3f));

        }

        private void CloseTooltip()
        {
            instance.Close();
            if (m_DelayTooltipCoroutine != null)
            {
                StopCoroutine(m_DelayTooltipCoroutine);
            }
        }

        [System.Serializable]
        public class StringPair {
            public string key;
            public string value;
        }
    }
}