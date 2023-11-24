using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames.InventorySystem.Configuration
{
    [Serializable]
    public class Input : Settings
    {
        public override string Name => "Input";

        [Header("Unstacking:")]
        [InspectorLabel("Event")]
        [EnumFlags]
        public UnstackInput unstackEvent = UnstackInput.OnClick | UnstackInput.OnDrag;
        [InspectorLabel("Key Code")]
        public Key unstackKeyCode = Key.LeftShift;

        [Flags]
        public enum UnstackInput {
            OnClick = 1,
            OnDrag = 2
        }
    }
}