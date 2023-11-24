using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.StatSystem
{
	public class UIStat : MonoBehaviour
	{
		[Header("Stat Definition")]
		[SerializeField]
		protected string m_StatsHandler = "Player Stats";
		[StatPicker]
		[SerializeField]
		protected Stat m_Stat;
		[StatPicker]
		[SerializeField]
		protected Stat m_FreePoints;
		[Header("UI References")]
		[SerializeField]
		protected Text m_StatName;
		[SerializeField]
		protected Image m_StatBar;
		[SerializeField]
		protected Image m_StatBarFade;
		[SerializeField]
		protected Text m_CurrentValue;
		[SerializeField]
		protected Text m_Value;
		[SerializeField]
		protected Button m_IncrementButton;

		private Stat stat;
		private Stat freePoints;

		protected virtual void Start() {
			if (m_IncrementButton != null)
			{
				m_IncrementButton.onClick.AddListener(delegate () {
					stat.Add(1f);
					freePoints.Subtract(1f);
				});
			}
		}

		protected virtual void Update() {
			StatsHandler handler = GetStatsHandler();
				if (handler == null)
					return;
				stat = handler.GetStat(m_Stat);
				if(m_FreePoints != null)
					freePoints = handler.GetStat(m_FreePoints);

				if (m_StatName != null)
					m_StatName.text = stat.Name;
				Repaint();
		}

		protected virtual StatsHandler GetStatsHandler() { 
			return StatsManager.GetStatsHandler(m_StatsHandler); 
		}

		protected virtual void Repaint() {
			if (stat is Attribute attribute)
			{
				float normalized = attribute.CurrentValue / attribute.Value;

				if (m_StatBar != null)
				{
					m_StatBar.fillAmount = normalized;
				}

				if (m_StatBarFade != null)
				{
					m_StatBarFade.fillAmount = Mathf.MoveTowards(m_StatBarFade.fillAmount, normalized, Time.deltaTime * 0.5f);
				}

				if (m_CurrentValue != null)
				{
					m_CurrentValue.text = attribute.CurrentValue.ToString();
				}
			}

			if (m_Value != null)
			{

				m_Value.text = stat.Value.ToString();
			}
			
			if (m_IncrementButton != null && freePoints != null)
			{
				m_IncrementButton.gameObject.SetActive(freePoints.Value > 0?true:false);
			}
		}

		private void OnCharacterLoaded(CallbackEventData data)
		{
			if (GetComponentInParent(data.GetData("Slot").GetType()) != (Component)data.GetData("Slot"))
			{
				return;
			}
			
			string key = data.GetData("CharacterName") + ".Stats." + m_StatsHandler + "." + m_Stat.Name;
			if (PlayerPrefs.HasKey(key + ".Value"))
			{
				float value = PlayerPrefs.GetFloat(key + ".Value");
				if (m_Value != null)
					m_Value.text = value.ToString();
			}
			if (PlayerPrefs.HasKey(key + ".CurrentValue"))
			{
				float currentValue = PlayerPrefs.GetFloat(key + ".CurrentValue");
				if (m_CurrentValue != null)
					m_CurrentValue.text = currentValue.ToString();
			}
		}
	}
}