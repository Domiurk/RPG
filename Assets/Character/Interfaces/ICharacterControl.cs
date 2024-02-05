using UnityEngine;

namespace Character.Interfaces
{
    public interface ICharacterControl
    {
        CharacterControlData CharacterControlData { get; }
        void Moving(Vector3 direction);
        void Jump();
    }
}