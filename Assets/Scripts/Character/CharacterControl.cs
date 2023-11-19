using Character.Interfaces;
using Test.CharacterMover;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Character
{
    public class CharacterControl : MonoBehaviour, ICharacterControl
    {
        [field: SerializeField] public CharacterControlData CharacterControlData { get; private set; }
        private InputActionReference moveAction;
        private InputActionReference jumpAction;
        private InputActionReference lookAction;
        private StateMachine stateMachine;
        private CharacterController controller;
        private float gravityVelocity;

        private void Awake()
        {
            stateMachine = new StateMachine(this)
                           .With(machine => machine.AddState(new PlayerIdleState("idle", stateMachine)))
                           .With(machine => machine.AddState(new PlayerMovementState("move", stateMachine,
                                                              moveAction)));
        }

        private void Update()
        {
            stateMachine.StateUpdate();
        }

        public void Moving(Vector3 direction)
        {
            controller.Move(direction);
        }

        public void Jump() { }

        private bool CheckGround()
        {
            return true;
        }

        private float GetGravity()
        {
            gravityVelocity = CharacterControlData.Gravity * Time.fixedDeltaTime;
            return 0; //gravityVelocity * ;
        }
    }
}