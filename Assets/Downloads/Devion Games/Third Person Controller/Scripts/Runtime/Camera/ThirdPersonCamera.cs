using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DevionGames
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private bool m_DontDestroyOnLoad = true;
        [SerializeField] private Transform m_Target;
        public Transform Target
        {
            get{
                GameObject target = GameObject.FindGameObjectWithTag("Player");

                if(target != null)
                    m_Target = target.transform;

                return m_Target;
            }
        }

        [SerializeField] private CameraSettings[] m_Presets;

        public CameraSettings[] Presets => m_Presets;

        private Transform m_Transform;
        private CameraSettings m_ActivePreset;

        private float m_MouseX;
        private float m_MouseY;
        private float m_SmoothX;
        private float m_SmoothY;
        private float m_SmoothZoom;
        private float m_SmoothXVelocity;
        private float m_SmoothYVelocity;
        private float m_ZoomVelocity;
        private Vector3 m_SmoothMoveVelocity;
        private bool m_GUIClick;
        private bool m_ConsumeTurn;
        private bool m_ConsumeZoom;

        private Mouse m_Mouse;
        private Canvas m_CrosshairCanvas;
        private Image m_CrosshairImage;
        private bool m_CrosshairActive;
        private bool m_RotatedLastFrame;
        private bool m_CharacterControllerActive = true;

        private void Start()
        {
            m_Transform = transform;
            m_Mouse = Mouse.current;

            if(m_Transform.parent != null)
                m_Transform.parent = null;

            if(m_DontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            foreach(CameraSettings preset in m_Presets){
                if(preset.Activation == CameraSettings.ActivationType.Automatic){
                    m_ActivePreset = preset;
                    m_ActivePreset.IsActive = true;
                    break;
                }
            }

            ApplyCrosshair(m_ActivePreset.Crosshair);
            m_MouseY = Target.eulerAngles.x;
            m_MouseX = Target.eulerAngles.y;
            Vector3 targetPosition = Target.position + m_ActivePreset.Offset.x * m_Transform.right +
                                     m_ActivePreset.Offset.y * Target.up;
            m_SmoothZoom = m_ActivePreset.Distance + m_ActivePreset.Zoom;
            Vector3 direction = -m_SmoothZoom * m_Transform.forward;
            Vector3 desiredPosition = targetPosition + direction;
            m_Transform.position = desiredPosition;
            Cursor.lockState = m_ActivePreset.CursorMode;
            Cursor.visible = m_ActivePreset.CursorMode != CursorLockMode.Locked;
            EventHandler.Register<bool>(Target.gameObject, "OnSetControllerActive", OnSetControllerActive);
            EventHandler.Register(Target.gameObject, "OnEndUse", OnEndUse);
            EventHandler.Register(Target.gameObject, "TriggerAnimationEvent", TriggerAnimationEvent);
        }

        private void OnEnable()
        {
            foreach(CameraSettings preset in m_Presets.Where(p => p.InputAction != null))
                preset.InputAction.action.Enable();

            if(m_CrosshairImage != null)
                m_CrosshairImage.gameObject.SetActive(m_CrosshairActive);
        }

        private void OnDisable()
        {
            foreach(CameraSettings preset in m_Presets.Where(p => p.InputAction != null))
                preset.InputAction.action.Enable();

            if(m_CrosshairImage != null){
                m_CrosshairActive = m_CrosshairImage.gameObject.activeSelf;
                m_CrosshairImage.gameObject.SetActive(false);
            }
        }

        private void OnSetControllerActive(bool active)
        {
            m_CharacterControllerActive = active;
        }

        public void TriggerAnimationEvent()
        {
            print("muchas gracias!!!");
        }
        
        public void OnEndUse()
        {
            print("Buy buy");
        }  
        
        private void LateUpdate()
        {
            UpdateInput();
            if(!m_CharacterControllerActive)
                UpdateTransform();
        }

        public void FixedUpdate()
        {
            if(m_CharacterControllerActive)
                UpdateTransform();
        }

        private void UpdateTransform()
        {
            m_SmoothX = Mathf.SmoothDamp(m_SmoothX, m_MouseX, ref m_SmoothXVelocity,
                                         m_ActivePreset.TurnSmoothing);
            m_SmoothY = Mathf.SmoothDamp(m_SmoothY, m_MouseY, ref m_SmoothYVelocity,
                                         m_ActivePreset.TurnSmoothing);
            m_Transform.rotation = Quaternion.Euler(m_SmoothY, m_SmoothX, 0f);

            m_SmoothZoom = Mathf.SmoothDamp(m_SmoothZoom,
                                            m_ActivePreset.Distance + m_ActivePreset.Zoom,
                                            ref m_ZoomVelocity, m_ActivePreset.ZoomSmoothing);

            Vector3 targetPosition = Target.position + m_ActivePreset.Offset.x * m_Transform.right +
                                     m_ActivePreset.Offset.y * Target.up;
            Vector3 direction = -m_SmoothZoom * m_Transform.forward;
            Vector3 desiredPosition = targetPosition + direction;

            if(Physics.SphereCast(targetPosition - direction.normalized * m_ActivePreset.CollisionRadius,
                                  m_ActivePreset.CollisionRadius, direction.normalized, out RaycastHit hit,
                                  direction.magnitude, m_ActivePreset.CollisionLayer,
                                  QueryTriggerInteraction.Ignore)){
                desiredPosition = hit.point + hit.normal * 0.1f;
            }

            m_Transform.position = Vector3.SmoothDamp(m_Transform.position, desiredPosition,
                                                      ref m_SmoothMoveVelocity,
                                                      m_ActivePreset.MoveSmoothing);
        }

        private void UpdateInput()
        {
            bool mouseClicked = m_Mouse.rightButton.isPressed || m_Mouse.leftButton.isPressed;
            m_ConsumeTurn = false;
            m_ConsumeZoom = m_ActivePreset.ConsumeInputOverUI && UnityTools.IsPointerOverUI();

            if(mouseClicked)
                m_GUIClick = EventSystem.current != null && UnityTools.IsPointerOverUI();

            foreach(CameraSettings preset in m_Presets){
                if(!m_GUIClick){
                    switch(preset.Activation){
                        case CameraSettings.ActivationType.Button:
                            if(preset.InputAction.action.WasPressedThisFrame())
                                preset.IsActive = true;
                            if(preset.InputAction.action.WasReleasedThisFrame())
                                preset.IsActive = false;
                            break;
                        case CameraSettings.ActivationType.Toggle:
                            if(preset.InputAction.action.WasPressedThisFrame())
                                preset.IsActive = !preset.IsActive;
                            break;
                    }
                }

                if(preset.IsActive){
                    if(m_ActivePreset != preset){
                        m_ActivePreset = preset;
                        ApplyCrosshair(m_ActivePreset.Crosshair);
                    }
                    break;
                }
            }

            if(m_ActivePreset.ConsumeInputOverUI && m_GUIClick){
                m_ConsumeTurn = true;
                m_ConsumeZoom = true;
            }

            m_ConsumeTurn = m_ActivePreset.TurnButton == null || m_ConsumeTurn;

            if(!m_ConsumeTurn && 
               (m_ActivePreset.TurnButton != null || m_ActivePreset.TurnButton.action.WasReleasedThisFrame())){
                Vector2 mouse = m_Mouse.delta.ReadValue() * m_ActivePreset.TurnSpeed;

                if(m_ActivePreset.VisibilityDelta == 0f || Mathf.Abs(mouse.x) > m_ActivePreset.VisibilityDelta ||
                   Mathf.Abs(mouse.y) > m_ActivePreset.VisibilityDelta){
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    m_RotatedLastFrame = true;
                }

                m_MouseX += mouse.x;
                m_MouseY -= mouse.y;

                if(Mathf.Abs(m_ActivePreset.YawLimit.x) + Mathf.Abs(m_ActivePreset.YawLimit.y) < 360)
                    m_MouseX = ClampAngle(m_MouseX, m_ActivePreset.YawLimit.x,
                                          m_ActivePreset.YawLimit.y);

                m_MouseY = ClampAngle(m_MouseY, m_ActivePreset.PitchLimit.x,
                                      m_ActivePreset.PitchLimit.y);
            }
            else if(m_RotatedLastFrame){
                Cursor.lockState = m_ActivePreset.CursorMode;
                Cursor.visible = m_ActivePreset.CursorMode != CursorLockMode.Locked;
                m_RotatedLastFrame = false;
            }

            if(!m_ConsumeZoom){
                m_ActivePreset.Zoom -= m_Mouse.scroll.y.ReadValue() * m_ActivePreset.ZoomSpeed;
                m_ActivePreset.Zoom = Mathf.Clamp(m_ActivePreset.Zoom,
                                                  m_ActivePreset.ZoomLimit.x - m_ActivePreset.Distance,
                                                  m_ActivePreset.ZoomLimit.y - m_ActivePreset.Distance);
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            do{
                if(angle < -360f)
                    angle += 360f;
                if(angle > 360f)
                    angle -= 360f;
            }
            while(angle is < -360f or > 360f);

            return Mathf.Clamp(angle, min, max);
        }

        private void ApplyCrosshair(Sprite crosshair)
        {
            if(m_CrosshairImage == null){
                CreateCrosshairUI();
            }

            if(crosshair != null){
                m_CrosshairImage.sprite = crosshair;
                m_CrosshairImage.SetNativeSize();
                m_CrosshairImage.gameObject.SetActive(true);
            }
            else{
                m_CrosshairImage.gameObject.SetActive(false);
            }
        }

        private void CreateCrosshairUI()
        {
            GameObject canvasGameObject = new GameObject("Crosshair Canvas");
            m_CrosshairCanvas = canvasGameObject.AddComponent<Canvas>();
            m_CrosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_CrosshairCanvas.pixelPerfect = true;
            m_CrosshairCanvas.overrideSorting = true;
            m_CrosshairCanvas.sortingOrder = 100;
            GameObject crosshairGameObject = new GameObject("Crosshair");
            m_CrosshairImage = crosshairGameObject.AddComponent<Image>();
            crosshairGameObject.transform.SetParent(canvasGameObject.transform, false);
            crosshairGameObject.SetActive(false);
            canvasGameObject.AddComponent<DontDestroyOnLoad>();
        }
    }
}