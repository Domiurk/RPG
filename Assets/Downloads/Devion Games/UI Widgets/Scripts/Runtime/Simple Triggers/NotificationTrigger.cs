using UnityEngine;
using UnityEngine.UI;
using DevionGames.UIWidgets;

/// <summary>
/// Message container example.
/// </summary>
public class NotificationTrigger : MonoBehaviour {
	private Notification m_Notification;
	public NotificationOptions[] options;

	private void Start(){
		m_Notification = WidgetUtility.Find<Notification> ("Notification");
	}

	/// <summary>
	/// Called from a button OnClick event in the example
	/// </summary>
	public void AddRandomNotification(){
		NotificationOptions option=options[Random.Range(0,options.Length)];
		m_Notification.AddItem(option);
	}

	/// <summary>
	/// Called from a button OnClick event in the example
	/// </summary>
	public void AddNotification(InputField input){
		m_Notification.AddItem (input.text);
	}

	/// <summary>
	/// Called from a Slider OnValueChanged event in the example
	/// </summary>
	public void AddNotification(float index){
		NotificationOptions option = options [Mathf.RoundToInt (index)];
		m_Notification.AddItem (option);
	}
}
