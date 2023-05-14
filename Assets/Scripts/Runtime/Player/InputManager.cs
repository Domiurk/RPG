using UnityEngine;

namespace Runtime.Player
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Current;
        public Input InputController { get; set; }
        public Vector2 Movement => InputController.Mover.Movement.ReadValue<Vector2>();

        private void Awake()
        {
            if(Current != null && Current != this)
                Destroy(Current);
            else
                Current = this;

            InputController = new Input();
            InputController.Mover.Enable();
        }
    }
}