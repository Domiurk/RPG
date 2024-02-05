using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "Character Control Data", menuName = "Game/new Character Control Data", order = 0)]
    public class CharacterControlData : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; } = 1f;
        [field: SerializeField] public float JumpPower { get; private set; } = 1f;
        [field: SerializeField] public float Gravity { get; private set; } = -9.8f;
        [field: SerializeField] public LayerMask GroundLayers { get; private set; } = LayerMask.GetMask("Ground");
    }
}