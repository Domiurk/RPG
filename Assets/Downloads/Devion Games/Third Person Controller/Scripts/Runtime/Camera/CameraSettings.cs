using UnityEngine;
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
        [SerializeField] private Vector2 m_Offset = new(0.25f, 1.5f);
        [SerializeField] private float m_Distance = 2.5f;

        [HeaderLine("Input")]
        [SerializeField] private InputActionReference m_TurnButton;
        [SerializeField] private float m_TurnSpeed = 1.5f;
        [SerializeField] private float m_TurnSmoothing = 0.05f;
        [MinMaxSlider(-180, 180)]
        [SerializeField] private Vector2 m_YawLimit = new(-180, 180);
        [MinMaxSlider(-90f, 90f)]
        [SerializeField] private Vector2 m_PitchLimit = new(-60, 60);
        [SerializeField] private float m_VisibilityDelta = 0.3f;

        [SerializeField] private float m_ZoomSpeed = 5f;
        [MinMaxSlider(0f, 25f)]
        [SerializeField] private Vector2 m_ZoomLimit = new(0f, 10f);
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
            get => m_Name;
            set => m_Name = value;
        }

        public InputActionReference InputAction
        {
            get => m_InputAction;
            set => m_InputAction = value;
        }

        public ActivationType Activation
        {
            get => m_Activation;
            set => m_Activation = value;
        }

        public Vector2 Offset
        {
            get => m_Offset;
            set => m_Offset = value;
        }

        public float Distance
        {
            get => m_Distance;
            set => m_Distance = value;
        }

        public Sprite Crosshair
        {
            get => m_Crosshair;
            set => m_Crosshair = value;
        }

        public InputActionReference TurnButton
        {
            get => m_TurnButton;
            set => m_TurnButton = value;
        }

        public float TurnSpeed
        {
            get => m_TurnSpeed;
            set => m_TurnSpeed = value;
        }

        public float TurnSmoothing
        {
            get => m_TurnSmoothing;
            set => m_TurnSmoothing = value;
        }

        public Vector2 YawLimit
        {
            get => m_YawLimit;
            set => m_YawLimit = value;
        }

        public Vector2 PitchLimit
        {
            get => m_PitchLimit;
            set => m_PitchLimit = value;
        }

        public float VisibilityDelta
        {
            get => m_VisibilityDelta;
            set => m_VisibilityDelta = value;
        }

        public float ZoomSpeed
        {
            get => m_ZoomSpeed;
            set => m_ZoomSpeed = value;
        }

        public Vector2 ZoomLimit
        {
            get => m_ZoomLimit;
            set => m_ZoomLimit = value;
        }

        public float ZoomSmoothing
        {
            get => m_ZoomSmoothing;
            set => m_ZoomSmoothing = value;
        }

        public float MoveSmoothing
        {
            get => m_MoveSmoothing;
            set => m_MoveSmoothing = value;
        }

        public CursorLockMode CursorMode
        {
            get => m_CursorMode;
            set => m_CursorMode = value;
        }

        public bool ConsumeInputOverUI
        {
            get => m_ConsumeInputOverUI;
            set => m_ConsumeInputOverUI = value;
        }

        public LayerMask CollisionLayer
        {
            get => m_CollisionLayer;
            set => m_CollisionLayer = value;
        }

        public float CollisionRadius
        {
            get => m_CollisionRadius;
            set => m_CollisionRadius = value;
        }

        public bool IsActive
        {
            get => m_IsActive;
            set => m_IsActive = value;
        }

        public float Zoom
        {
            get => m_Zoom;
            set => m_Zoom = value;
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