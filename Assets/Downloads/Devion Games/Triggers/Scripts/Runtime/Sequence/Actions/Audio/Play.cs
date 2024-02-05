using UnityEngine;
using UnityEngine.Audio;

namespace DevionGames
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [ComponentMenu("Audio/Play")]
    public class Play : Action
    {
        [SerializeField] private readonly AudioClip m_Clip = null;
        [SerializeField] private readonly AudioMixerGroup m_AudioMixerGroup = null;
        [SerializeField] private readonly float m_Volume = 0.4f;

        public override ActionStatus OnUpdate()
        {
            UnityTools.PlaySound(m_Clip, m_Volume, m_AudioMixerGroup);
            return ActionStatus.Success;
        }
    }
}