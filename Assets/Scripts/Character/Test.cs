using Character.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Test.CharacterMover
{
    public class StateMachine
    {
        public ICharacterControl CharacterControl { get; }
        public State CurrentState { get; private set; }
        private readonly Dictionary<string, State> states;

        public StateMachine(ICharacterControl control)
        {
            states = new Dictionary<string, State>();
            CharacterControl = control;
        }

        public void AddState(State newState)
        {
            if(states.TryGetValue(newState.Name.ToLower(), out _))
                return;

            states.Add(newState.Name.ToLower(), newState);
        }

        public bool TryChangeState(State state) => TryChangeState(state.Name);

        public bool TryChangeState(string stateName)
        {
            if(!states.TryGetValue(stateName.ToLower(), out State newState))
                return false;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            return true;
        }

        public void StateUpdate()
        {
            CurrentState?.Update();
        }
    }

    public abstract class State
    {
        public string Name { get; }
        protected StateMachine StateMachine { get; }

        protected State(string name, StateMachine stateMachine)
        {
            StateMachine = stateMachine;
            Name = name;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }

        public virtual bool HandleInput() => false;
        public virtual void FixedUpdate() { }
    }

    internal class PlayerIdleState : State
    {
        public PlayerIdleState(string name, StateMachine stateMachine) : base(name, stateMachine) { }
    }

    internal class PlayerMovementState : State
    {
        private InputActionReference MoveAction { get; }
        private Vector2 ReadMoving { get; set; }
        private float speed;

        public PlayerMovementState(string name,
                                   StateMachine stateMachine,
                                   InputActionReference moveAction)
            : base(name, stateMachine)
        {
            MoveAction = moveAction;
        }

        public override void Enter()
        {
            MoveAction.action.Enable();
            speed = StateMachine.CharacterControl.CharacterControlData.Speed;
        }

        public override void Update()
        {
            ReadMoving = MoveAction.action.ReadValue<Vector2>();
        }

        public override void FixedUpdate()
        {
            Vector3 moving = new Vector3(ReadMoving.x * speed, 0, ReadMoving.y * speed);
            StateMachine.CharacterControl.Moving(moving);
        }

        public override void Exit()
        {
            MoveAction.action.Disable();
        }
    }
}