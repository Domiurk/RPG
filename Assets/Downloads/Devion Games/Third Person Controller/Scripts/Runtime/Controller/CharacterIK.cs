using UnityEngine;

namespace DevionGames
{
    public class CharacterIK : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_LookOffset = new(0f, 1.5f, 3f);
        [SerializeField]
        private float m_BodyWeight = 0.6f;
        [SerializeField]
        private float m_HeadWeight = 0.2f;
        [SerializeField]
        private float m_EyesWeight = 0.2f;
        [SerializeField]
        private float m_ClampWeight = 0.35f;

        private float m_Weight;
        private Transform m_CameraTransform;
        private Transform m_Transform;
        private Animator m_Animator;
        private Vector3 m_AimPosition;
        private ThirdPersonController m_Controller;
        private bool m_ControllerActive = true;

        private bool ik = true;

        private void Start()
        {
            m_CameraTransform = Camera.main.transform;
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            m_Controller = GetComponent<ThirdPersonController>();
            EventHandler.Register<bool>(gameObject,"OnSetControllerActive", OnSetControllerActive);
        }

        private void Update()
        {
            float relativeX = m_CameraTransform.InverseTransformPoint(m_Transform.position).x;
            m_AimPosition = m_Transform.position + m_CameraTransform.forward * m_LookOffset.z + Vector3.up * m_LookOffset.y + m_CameraTransform.right * (m_LookOffset.x - relativeX * 2f);
            Vector3 directionToTarget = m_Transform.position - m_CameraTransform.position;
            float angle = Vector3.Angle(m_Transform.forward, directionToTarget);
            if (Mathf.Abs(angle) < 90  && m_ControllerActive && m_Controller.isActiveAndEnabled && ik)
            {
                m_Weight = Mathf.Lerp(m_Weight, 1f, Time.deltaTime);
            }
            else
            {
                m_Weight = Mathf.Lerp(m_Weight, 0f, Time.deltaTime*2f);
            }
        }

        private void OnSetControllerActive(bool state) {
            m_ControllerActive = state;
        }

        private void SetIK(bool state) {
            ik = state;
        }

        private void OnAnimatorIK(int layer)
        {

            for (int i = 0; i < m_Controller.Motions.Count; i++)
            {
                MotionState motion = m_Controller.Motions[i];
                if (motion.IsActive && !motion.UpdateAnimatorIK(layer))
                {
                   return;
                }
            }

            if (layer == 0)
            {
                m_Animator.SetLookAtPosition(m_AimPosition);
                m_Animator.SetLookAtWeight(m_Weight, m_BodyWeight, m_HeadWeight, m_EyesWeight, m_ClampWeight);
            }
        }
    }
}