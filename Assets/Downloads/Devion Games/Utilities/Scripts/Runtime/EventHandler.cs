﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames
{
    public class EventHandler : MonoBehaviour
    {
        private static readonly Dictionary<string, Delegate> m_GlobalEvents;
        private static readonly Dictionary<object, Dictionary<string, Delegate>> m_Events;

        static EventHandler()
        {
            m_GlobalEvents = new Dictionary<string, Delegate>();
            m_Events = new Dictionary<object, Dictionary<string, Delegate>>();
        }

        public static void Execute(string eventName)
        {
            if(GetDelegate(eventName) is Action mDelegate){
                mDelegate();
            }
        }

        public static void Execute(object obj, string eventName)
        {
            if(GetDelegate(obj, eventName) is Action mDelegate){
                mDelegate();
            }
        }

        public static void Execute<T1>(string eventName, T1 arg1)
        {
            if(GetDelegate(eventName) is Action<T1> mDelegate){
                mDelegate(arg1);
            }
        }

        public static void Execute<T1>(object obj, string eventName, T1 arg1)
        {
            if(GetDelegate(obj, eventName) is Action<T1> mDelegate){
                mDelegate(arg1);
            }
        }

        public static void Execute<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            if(GetDelegate(eventName) is Action<T1, T2> mDelegate){
                mDelegate(arg1, arg2);
            }
        }

        public static void Execute<T1, T2>(object obj, string eventName, T1 arg1, T2 arg2)
        {
            if(GetDelegate(obj, eventName) is Action<T1, T2> mDelegate){
                mDelegate(arg1, arg2);
            }
        }

        public static void Execute<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            if(GetDelegate(eventName) is Action<T1, T2, T3> mDelegate){
                mDelegate(arg1, arg2, arg3);
            }
        }

        public static void Execute<T1, T2, T3>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            if(GetDelegate(obj, eventName) is Action<T1, T2, T3> mDelegate){
                mDelegate(arg1, arg2, arg3);
            }
        }

        public static void Register(string eventName, Action handler)
        {
            Register(eventName, (Delegate)handler);
        }

        public static void Register(object obj, string eventName, Action handler)
        {
            Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1>(string eventName, Action<T1> handler)
        {
            Register(eventName, (Delegate)handler);
        }

        public static void Register<T1>(object obj, string eventName, Action<T1> handler)
        {
            Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            Register(eventName, (Delegate)handler);
        }

        public static void Register<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
        {
            Register(obj, eventName, (Delegate)handler);
        }

        public static void Register<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            Register(eventName, (Delegate)handler);
        }

        public static void Register<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
        {
            Register(obj, eventName, (Delegate)handler);
        }

        public static void Unregister(string eventName, Action handler)
        {
            Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister(object obj, string eventName, Action handler)
        {
            Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1>(string eventName, Action<T1> handler)
        {
            Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1>(object obj, string eventName, Action<T1> handler)
        {
            Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2>(object obj, string eventName, Action<T1, T2> handler)
        {
            Unregister(obj, eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler)
        {
            Unregister(eventName, (Delegate)handler);
        }

        public static void Unregister<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> handler)
        {
            Unregister(obj, eventName, (Delegate)handler);
        }

        private static void Register(string eventName, Delegate handler)
        {
            if(!m_GlobalEvents.TryGetValue(eventName, out Delegate mDelegate)){
                m_GlobalEvents.Add(eventName, handler);
            }
            else{
                m_GlobalEvents[eventName] = Delegate.Combine(mDelegate, handler);
            }
        }

        private static void Register(object obj, string eventName, Delegate handler)
        {
            if(obj == null)
                return;

            if(!m_Events.TryGetValue(obj, out Dictionary<string, Delegate> mEvents)){
                mEvents = new Dictionary<string, Delegate>();
                m_Events.Add(obj, mEvents);
            }

            if(!mEvents.TryGetValue(eventName, out Delegate mDelegate)){
                mEvents.Add(eventName, handler);
            }
            else{
                mEvents[eventName] = Delegate.Combine(mDelegate, handler);
            }
        }

        private static void Unregister(string eventName, Delegate handler)
        {
            if(m_GlobalEvents.TryGetValue(eventName, out Delegate mDelegate)){
                m_GlobalEvents[eventName] = Delegate.Remove(mDelegate, handler);
            }
        }

        private static void Unregister(object obj, string eventName, Delegate handler)
        {
            if(obj == null)
                return;

            if(m_Events.TryGetValue(obj, out Dictionary<string, Delegate> mEvents) &&
               mEvents.TryGetValue(eventName, out Delegate mDelegate)){
                mEvents[eventName] = Delegate.Remove(mDelegate, handler);
            }
        }

        private static Delegate GetDelegate(string eventName)
        {
            if(m_GlobalEvents.TryGetValue(eventName, out Delegate mDelegate)){
                return mDelegate;
            }

            return null;
        }

        private static Delegate GetDelegate(object obj, string eventName)
        {
            if(m_Events.TryGetValue(obj, out Dictionary<string, Delegate> mEvents) &&
               mEvents.TryGetValue(eventName, out Delegate mDelegate)){
                return mDelegate;
            }

            return null;
        }
    }
}