﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
    public class SingleInstance : MonoBehaviour
    {
        private static readonly Dictionary<string, GameObject> m_Instances = new();

        void Awake()
        {
            m_Instances.TryGetValue(name, out GameObject instance);
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                m_Instances[name] = gameObject;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public static List<GameObject> GetInstanceObjects() {
            return m_Instances.Values.Where(x=>x != null).ToList();
        }
    }
}