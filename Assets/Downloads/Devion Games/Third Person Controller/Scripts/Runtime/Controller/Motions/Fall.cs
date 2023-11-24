using UnityEngine;

namespace DevionGames
{
	public class Fall : MotionState, IControllerGrounded
	{
		[SerializeField]
		private float m_GravityMultiplier = 2f;
		[SerializeField]
		private float m_FallMinHeight = 0.3f;

		public override void OnStart ()
		{
			m_Animator.SetInteger ("Int Value", 0);
            if (m_Controller.RawInput.z < 0f)
            {
                m_Animator.SetBool("Bool Value", true);
            }
            else
            {
                m_Animator.SetBool("Bool Value", false);
            }
        }

		public override bool UpdateVelocity (ref Vector3 velocity)
		{
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			velocity += extraGravityForce * Time.deltaTime;
			return true;
		}

		public override bool UpdateAnimator ()
		{
			m_Animator.SetFloat ("Float Value", m_Rigidbody.velocity.y, 0.15f, Time.deltaTime);
			return true;
		}

		public override bool CanStart ()
		{
			if (m_FallMinHeight != 0f && Physics.Raycast (m_Transform.position + m_Transform.up, -m_Transform.up, out RaycastHit hitInfo, m_Transform.up.y + m_FallMinHeight) && hitInfo.distance < m_Transform.up.y + m_FallMinHeight) {
				return false;
			}

			return m_Rigidbody.velocity.y < 0f && !m_Controller.IsGrounded;
		}

		private void OnControllerLanded ()
		{
			StopMotion (true);
		}

        public void OnControllerGrounded(bool grounded)
        {
			if (IsActive)
            {
                m_Animator.SetInteger("Int Value", 1);
            }
        }
    }
}