﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames
{
    [System.Serializable]
    public class CameraSettings
    {
        [SerializeField] private string m_Name = "Camera Preset";
        [SerializeField] private InputActionReference m_InputAction;
        [SerializeField] private ActivationType m_Activation = ActivationType.Automatic;
        [SerializeField] private Sprite m_Crosshair;
        [SerializeField] private Vector2 m_Offset = new Vector2(0.25f, 1.5f);
        [SerializeField] private float m_Distance = 2.5f;

        [HeaderLine("Input")]
        [SerializeField] private InputActionReference m_TurnButton;
        [SerializeField] private float m_TurnSpeed = 1.5f;
        [SerializeField] private float m_TurnSmoothing = 0.05f;
        [MinMaxSlider(-180, 180)]
        [SerializeField] private Vector2 m_YawLimit = new Vector2(-180, 180);
        [MinMaxSlider(-90f, 90f)]
        [SerializeField] private Vector2 m_PitchLimit = new Vector2(-60, 60);
        [SerializeField] private float m_VisibilityDelta = 0.3f;

        [SerializeField] private float m_ZoomSpeed = 5f;
        [MinMaxSlider(0f, 25f)]
        [SerializeField] private Vector2 m_ZoomLimit = new Vector2(0f, 10f);
        [SerializeField] private float m_ZoomSmoothing = 0.1f;
        [SerializeField] private float m_MoveSmoothing = 0.07f;
        [SerializeField] private CursorLockMode m_CursorMode;
        [SerializeField] private bool m_ConsumeInputOverUI = true;

        [HeaderLine("Collision")]
        [SerializeField] private LayerMask m_CollisionLayer = 1 << 0;
        [SerializeField] private float m_CollisionRadius = 0.4f;

        private bool m_IsActive;
        private float m_Zoom;

        public string Name
        {
            get => this.m_Name;
            set => this.m_Name = value;
        }

        public InputActionReference InputAction
        {
            get => this.m_InputAction;
            set => this.m_InputAction = value;
        }

        public ActivationType Activation
        {
            get => this.m_Activation;
            set => this.m_Activation = value;
        }

        public Vector2 Offset
        {
            get => this.m_Offset;
            set => this.m_Offset = value;
        }

        public float Distance
        {
            get => this.m_Distance;
            set => this.m_Distance = value;
        }

        public Sprite Crosshair
        {
            get => this.m_Crosshair;
            set => this.m_Crosshair = value;
        }

        public InputActionReference TurnButton
        {
            get => this.m_TurnButton;
            set => this.m_TurnButton = value;
        }

        public float TurnSpeed
        {
            get => this.m_TurnSpeed;
            set => this.m_TurnSpeed = value;
        }

        public float TurnSmoothing
        {
            get => this.m_TurnSmoothing;
            set => this.m_TurnSmoothing = value;
        }

        public Vector2 YawLimit
        {
            get => this.m_YawLimit;
            set => this.m_YawLimit = value;
        }

        public Vector2 PitchLimit
        {
            get => this.m_PitchLimit;
            set => this.m_PitchLimit = value;
        }

        public float VisibilityDelta
        {
            get => this.m_VisibilityDelta;
            set => this.m_VisibilityDelta = value;
        }

        public float ZoomSpeed
        {
            get => this.m_ZoomSpeed;
            set => this.m_ZoomSpeed = value;
        }

        public Vector2 ZoomLimit
        {
            get => this.m_ZoomLimit;
            set => this.m_ZoomLimit = value;
        }

        public float ZoomSmoothing
        {
            get => this.m_ZoomSmoothing;
            set => this.m_ZoomSmoothing = value;
        }

        public float MoveSmoothing
        {
            get => this.m_MoveSmoothing;
            set => this.m_MoveSmoothing = value;
        }

        public CursorLockMode CursorMode
        {
            get => this.m_CursorMode;
            set => this.m_CursorMode = value;
        }

        public bool ConsumeInputOverUI
        {
            get => this.m_ConsumeInputOverUI;
            set => this.m_ConsumeInputOverUI = value;
        }

        public LayerMask CollisionLayer
        {
            get => this.m_CollisionLayer;
            set => this.m_CollisionLayer = value;
        }

        public float CollisionRadius
        {
            get => this.m_CollisionRadius;
            set => this.m_CollisionRadius = value;
        }

        public bool IsActive
        {
            get => this.m_IsActive;
            set => this.m_IsActive = value;
        }

        public float Zoom
        {
            get => this.m_Zoom;
            set => this.m_Zoom = value;
        }

        public enum ActivationType
        {
            Automatic,
            Manual,
            Button,
            Toggle
        }
    }
}