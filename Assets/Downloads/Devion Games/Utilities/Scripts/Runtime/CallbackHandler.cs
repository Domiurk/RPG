using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DevionGames
{
    /// <summary>
    /// Callback handler for custom events.
    /// </summary>
    public abstract class CallbackHandler : MonoBehaviour
    {
        [HideInInspector]
        public List<Entry> delegates;
        public abstract string[] Callbacks { get; }

        protected void Execute(string eventID, CallbackEventData eventData)
        {
            if(delegates != null){
                int num = 0;
                int count = delegates.Count;

                while(num < count){
                    Entry item = delegates[num];

                    if(item.eventID == eventID && item.callback != null){
                        item.callback.Invoke(eventData);
                    }

                    num++;
                }
            }
        }

        public void RegisterListener(string eventID, UnityAction<CallbackEventData> call)
        {
            delegates ??= new List<Entry>();

            Entry entry = delegates.FirstOrDefault(mEntry => mEntry.eventID == eventID);

            if(entry == null){
                entry = new Entry{
                    eventID = eventID,
                    callback = new CallbackEvent()
                };
                delegates.Add(entry);
            }

            entry.callback.AddListener(call);
        }

        public void RemoveListener(string eventID, UnityAction<CallbackEventData> call)
        {
            if(delegates == null){
                return;
            }

            foreach(Entry entry in delegates.Where(entry => entry.eventID == eventID)){
                entry.callback.RemoveListener(call);
            }
        }

        [Serializable]
        public class Entry
        {
            public string eventID;

            public CallbackEvent callback;
        }

        [Serializable]
        public class CallbackEvent : UnityEvent<CallbackEventData> { }
    }
}