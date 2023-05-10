using System;
using UnityEngine;

namespace Runtime.Player
{
    public class InputManager : MonoBehaviour
    {
        public static Input InputController;
        public static InputManager Current;

        private void Awake()
        {
            if(Current != null && Current != this)
                Destroy(Current);
            else
                Current = this;
            InputController ??= new Input();
        }
    }
}