﻿using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace DevionGames
{
    [Serializable]
    public class ModuleItem
    {

        public string name;
        public string id;
        public string assetPath = "";
        public string version;
        public string unityVersion = "19.1.1";
        public string assetStore;
        public string downloadPath;
        public string documentation;
        public string author;
        public string email;
        public string description;
        public string[] dependencies = new string[0];
        public Changelog[] changelogs = new Changelog[0];



        private bool m_IsDownloading;
        public bool IsDownloading {
            get => m_IsDownloading;
            set => m_IsDownloading = value;
        }

        private float m_DownloadProgress;
        public float DownloadProgress {
            get => m_DownloadProgress;
            set => m_DownloadProgress = value;
        }

        private Texture2D m_Icon;
        public Texture2D Icon
        {
            get {
                if (m_Icon == null) {
                    m_Icon = Resources.Load<Texture2D>("ModuleIcon");
                }
                return m_Icon;
            }
        }

        public bool IsInstalled => InstalledModule != null;

        [SerializeField]
        private ModuleItem m_InstalledModule;
        public ModuleItem InstalledModule => m_InstalledModule;
        [NonSerialized]
        private ModuleItem[] m_DependencyModules= new ModuleItem[0];
        public ModuleItem[] DependencyModules => m_DependencyModules;

        public bool CanInstall {
            get {
                return DependencyModules.Where(x => x == null).Count() == 0;
            }
        }

        public void Initialize() {
            if (m_InstalledModule == null)
            {
                string[] guids = AssetDatabase.FindAssets(id);
                if (guids.Length > 0)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));
                    m_InstalledModule = JsonUtility.FromJson<ModuleItem>(textAsset.text);
                }
            }

            m_DependencyModules = new ModuleItem[dependencies.Length];
            for (int i = 0; i < dependencies.Length; i++) {
                string[] guids = AssetDatabase.FindAssets(dependencies[i]);
                if (guids.Length > 0)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));
                    try
                    {
                        m_DependencyModules[i] = JsonUtility.FromJson<ModuleItem>(textAsset.text);
                    }
                    catch {
                        Debug.LogError(textAsset.text);
                    }
                }
            }
            Array.Reverse(changelogs);
        }

    }
}