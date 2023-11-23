using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevionGames
{
    public class SaveUIComponent : MonoBehaviour
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private UIBehaviour target;

        private void Awake()
        {
            if(target == null){
                return;
            }

            switch(target){
                case Slider slider:
                    slider.value = LoadFloat(slider.value);
                    slider.onValueChanged.AddListener(SaveFloat);
                    break;
                case InputField inputField:
                    inputField.text = LoadString(inputField.text);
                    inputField.onValueChanged.AddListener(SaveString);
                    break;
                case Dropdown dropdown:
                    dropdown.value = LoadInt(dropdown.value);
                    dropdown.onValueChanged.AddListener(SaveInt);
                    break;
                case Toggle toggle:
                    toggle.isOn = LoadBool(toggle.isOn);
                    toggle.onValueChanged.AddListener(SaveBool);
                    break;
            }
        }

        private void SaveFloat(float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        private float LoadFloat(float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        private void SaveInt(int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        private int LoadInt(int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        private void SaveString(string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        private string LoadString(string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        private void SaveBool(bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        private bool LoadBool(bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 0 ? false : true;
        }

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(key)){
                key = Guid.NewGuid().ToString();
            }

            if(target == null){
                target = GetComponent<UIBehaviour>();
            }
            else if(!(target is Slider or Dropdown or InputField or Toggle)){
                Debug.LogWarning("SaveUIComponent does not support target type (" + target.GetType().Name +
                                 "). Supported types are Slider, Dropdown, InputField, Toggle.");
            }
        }
    }
}