using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace DevionGames
{
    public class AudioEventListener : MonoBehaviour
    {
        [SerializeField] private List<AudioGroup> m_AudioGroups = new();

        private void Awake()
        {
            foreach(AudioGroup group in m_AudioGroups){
                group.audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void PlayAudio(AnimationEvent evt) {
            AudioGroup group = m_AudioGroups.First(x => x.name == evt.stringParameter);
            group.PlayOneShot(evt.objectReferenceParameter as AudioClip, evt.floatParameter);
        }



        [Serializable]
        public class AudioGroup {
            public string name = "SFX";
            [SerializeField]
            private AudioMixerGroup m_AudioMixerGroup;

            private AudioSource m_AudioSource;

            public AudioSource audioSource {
                get => m_AudioSource;
                set { m_AudioSource = value;
                    m_AudioSource.outputAudioMixerGroup = m_AudioMixerGroup;
                    m_AudioSource.spatialBlend = 1f;
                }
            }

            public void PlayOneShot(AudioClip clip, float volumeScale) {
                audioSource.PlayOneShot(clip,volumeScale);
            }
        }
    }
}