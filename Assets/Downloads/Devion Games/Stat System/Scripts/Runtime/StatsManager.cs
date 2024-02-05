using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using DevionGames.StatSystem.Configuration;

namespace DevionGames.StatSystem
{
    public class StatsManager : MonoBehaviour
    {
        private static StatsManager m_Current;

        /// <summary>
        /// The StatManager singleton object. This object is set inside Awake()
        /// </summary>
        public static StatsManager current
        {
            get
            {
                Assert.IsNotNull(m_Current, "Requires a Stats Manager.Create one from Tools > Devion Games > Stat System > Create Stats Manager!");
                return m_Current;
            }
        }

        [SerializeField]
        private StatDatabase m_Database;

        /// <summary>
        /// Gets the item database. Configurate it inside the editor.
        /// </summary>
        /// <value>The database.</value>
        public static StatDatabase Database
        {
            get
            {
                if (current != null)
                {
                    Assert.IsNotNull(current.m_Database, "Please assign StatDatabase to the Stats Manager!");
                    return current.m_Database;
                }
                return null;
            }
        }

        private static Default m_DefaultSettings;
        public static Default DefaultSettings
        {
            get
            {
                if (m_DefaultSettings == null)
                {
                    m_DefaultSettings = GetSetting<Default>();
                }
                return m_DefaultSettings;
            }
        }

        private static UI m_UI;
        public static UI UI
        {
            get
            {
                if (m_UI == null)
                {
                    m_UI = GetSetting<UI>();
                }
                return m_UI;
            }
        }

        private static Notifications m_Notifications;
        public static Notifications Notifications
        {
            get
            {
                if (m_Notifications == null)
                {
                    m_Notifications = GetSetting<Notifications>();
                }
                return m_Notifications;
            }
        }

        private static SavingLoading m_SavingLoading;
        public static SavingLoading SavingLoading
        {
            get
            {
                if (m_SavingLoading == null)
                {
                    m_SavingLoading = GetSetting<SavingLoading>();
                }
                return m_SavingLoading;
            }
        }

        private static T GetSetting<T>() where T : Settings
        {
            if (Database != null)
            {
                return (T)Database.settings.Where(x => x.GetType() == typeof(T)).FirstOrDefault();
            }
            return default(T);
        }

        /// Don't destroy this object instance when loading new scenes.
        /// </summary>
        public bool dontDestroyOnLoad = true;

        private List<StatsHandler> m_StatsHandler;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (m_Current != null)
            {
                Destroy(gameObject);
                return;
            }

            m_Current = this;
            if (dontDestroyOnLoad)
            {
                if (transform.parent != null)
                {
                    if (DefaultSettings.debugMessages)
                        Debug.Log("Stats Manager with DontDestroyOnLoad can't be a child transform. Unparent!");
                    transform.parent = null;
                }
                DontDestroyOnLoad(gameObject);
            }

            m_StatsHandler = new List<StatsHandler>();
            if (SavingLoading.autoSave)
            {
                StartCoroutine(RepeatSaving(SavingLoading.savingRate));
            }
            if (DefaultSettings.debugMessages)
                Debug.Log("Stats Manager initialized.");
        }

        private void Start()
        {
            if (SavingLoading.autoSave)
            {
                StartCoroutine(DelayedLoading(1f));
            }
        }


        public static void Save()
        {
            string key = PlayerPrefs.GetString(SavingLoading.savingKey, SavingLoading.savingKey);
            Save(key);
        }

        public static void Save(string key)
        {
            StatsHandler[] results = FindObjectsOfType<StatsHandler>().Where(x => x.saveable).ToArray();
            if (results.Length > 0)
            {
                string data = JsonSerializer.Serialize(results);

                foreach (StatsHandler handler in results)
                {
                    foreach (Stat stat in handler.m_Stats)
                    {
                        PlayerPrefs.SetFloat(key + ".Stats." + handler.HandlerName + "." + stat.Name + ".Value", stat.Value);
                        if(stat is Attribute attribute)
                            PlayerPrefs.SetFloat(key + ".Stats." + handler.HandlerName + "." + stat.Name + ".CurrentValue", attribute.CurrentValue);
                    }
                }

                PlayerPrefs.SetString(key+".Stats", data);

                List<string> keys = PlayerPrefs.GetString("StatSystemSavedKeys").Split(';').ToList();
                keys.RemoveAll(x => string.IsNullOrEmpty(x));
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                }
                PlayerPrefs.SetString("StatSystemSavedKeys", string.Join(";", keys));

    
                if (DefaultSettings.debugMessages)
                    Debug.Log("[Stat System] Stats saved: " + data);
            }
        }

        public static void Load()
        {
            string key = PlayerPrefs.GetString(SavingLoading.savingKey, SavingLoading.savingKey);
            Load(key);
        }

        public static void Load(string key)
        {
            string data = PlayerPrefs.GetString(key+".Stats");
            if (string.IsNullOrEmpty(data)) { return; }

            List<StatsHandler> results = FindObjectsOfType<StatsHandler>().Where(x => x.saveable).ToList();
            List<object> list = MiniJSON.Deserialize(data) as List<object>;

            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, object> handlerData = list[i] as Dictionary<string, object>;
                string handlerName = (string)handlerData["Name"];
                StatsHandler handler = results.Find(x => x.HandlerName == handlerName);
                if (handler != null)
                {
                    handler.SetObjectData(handlerData);
                }
            }

            if (DefaultSettings.debugMessages)
                Debug.Log("[Stat System] Stats loaded: "+ data);
        }



        private IEnumerator DelayedLoading(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Load();
        }

        private IEnumerator RepeatSaving(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                Save();
            }
        }

        public static void RegisterStatsHandler(StatsHandler handler)
        {
            if (!current.m_StatsHandler.Contains(handler))
            {
                current.m_StatsHandler.Add(handler);
            }
        }

        public static StatsHandler GetStatsHandler(string name)
        {
            return current.m_StatsHandler.Find(x => x.HandlerName == name);
        }

    }
}