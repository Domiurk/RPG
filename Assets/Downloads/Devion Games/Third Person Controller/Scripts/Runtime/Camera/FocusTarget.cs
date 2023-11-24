using UnityEngine;

namespace DevionGames
{
    public class FocusTarget : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_OffsetPosition = new(0f, 1f, 0f);
        [SerializeField]
        private float m_Pitch = 22f;
        [SerializeField]
        private float m_Distance = 2f;
        [SerializeField]
        private float m_Speed = 5f;

        [SerializeField]
        private bool m_SpinTarget = true;

        private bool m_Focus;
        private ThirdPersonCamera m_ThirdPersonCamera;
        private bool m_TargetRotationFinished;
        private bool m_GUIClick;

        private void Start()
        {
            m_ThirdPersonCamera = GetComponent<ThirdPersonCamera>();
        }

        private void Update()
        {
            if (m_Focus) {
                Transform target = m_ThirdPersonCamera.Target;
                Vector3 targetPosition = target.position + m_OffsetPosition.x * target.right + m_OffsetPosition.y* target.up;
                Vector3 direction = -m_Distance * transform.forward;
                Vector3 desiredPosition = targetPosition + direction;

                transform.position = Vector3.Lerp(transform.position, desiredPosition, m_Speed*Time.deltaTime );
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(m_Pitch,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z), m_Speed * Time.deltaTime);

                if (!m_TargetRotationFinished)
                {
                    Vector3 cameraDirection = transform.position - m_ThirdPersonCamera.Target.position;
                    cameraDirection.y = 0f;
                    Quaternion targetRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
                    m_ThirdPersonCamera.Target.rotation = Quaternion.Lerp(target.rotation, targetRotation, Time.deltaTime * m_Speed);
                    if (Quaternion.Angle(target.rotation, targetRotation) < 0.1f)
                    {
                        m_TargetRotationFinished = true;
                    }
                }else if (m_SpinTarget) {

                    if (Input.GetMouseButtonDown(0) && UnityTools.IsPointerOverUI()) {
                        m_GUIClick = true;
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        m_GUIClick = false;
                    }

                    if (Input.GetMouseButton(0) && !m_GUIClick)
                    {
                        float input = Input.GetAxis("Mouse X") * -m_Speed; 
                        target.Rotate(0, input, 0, Space.World);
                    }
                }
   

            }
        }

        private void Focus(bool focus)
        {
            m_Focus = focus;
            m_TargetRotationFinished = false;
            m_GUIClick = false;
            if (m_Focus) {
                m_ThirdPersonCamera.Target.SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}