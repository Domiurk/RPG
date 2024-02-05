using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames
{
    public class SetCursorLockState : CallbackHandler
    {
        public Key key = Key.LeftCtrl;

        public override string[] Callbacks
        {
            get{ return new[]{ "OnCursorLocked", "OnCursorUnlocked" }; }
        }

        private void Update()
        {
            CursorLockMode currentMode = Cursor.lockState;

            if(Keyboard.current[key].isPressed){
                if(currentMode != CursorLockMode.None){
                    Cursor.lockState = CursorLockMode.None;
                    Execute("OnCursorUnlocked", new CallbackEventData());
                }
            }
            else{
                if(currentMode != CursorLockMode.Locked){
                    Cursor.lockState = CursorLockMode.Locked;
                    Execute("OnCursorLocked", new CallbackEventData());
                }
            }
        }
    }
}