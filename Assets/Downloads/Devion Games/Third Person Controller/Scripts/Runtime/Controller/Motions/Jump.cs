using UnityEngine;

namespace DevionGames
{
	public class Jump : MotionState, IControllerGrounded
	{
		[SerializeField]
		private float m_Force = 5f;
		[SerializeField]
		private float m_RecurrenceDelay = 0.2f;


		private float jumpTime;
		private float lastJumpTime;

		public override void OnStart ()
		{
			StartJump ();
		}


        public override bool UpdateAnimator ()
		{
            m_Animator.SetFloat ("Float Value", m_Rigidbody.velocity.y, 0.15f, Time.deltaTime);
			return true;
		}

		private void StartJump ()
		{
			if (IsActive) {
				jumpTime = Time.time;
				m_Controller.IsGrounded = false;
				Vector3 velocity = m_Rigidbody.velocity;
				velocity.y = m_Force;
				m_Rigidbody.velocity = velocity;
				m_Animator.SetFloat ("Float Value", m_Rigidbody.velocity.y);
                float cycle = 0f;
                if (m_Controller.IsMoving)
                {
                    float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
                    cycle = Mathf.Sin(360f * normalizedTime);
                }
                m_Animator.SetFloat("Leg", cycle);
                if (m_Controller.RawInput.z < 0f)
                {
                    m_Animator.SetBool("Bool Value", true);
                }else {
                    m_Animator.SetBool("Bool Value", false);
                }
            }
		}

		public override bool CheckGround ()
		{
			if (Time.time > jumpTime + 0.2f) {
				return true;
			}
			return false;
		}

        public override bool CheckStep()
        {
			return false;
        }

        public void OnControllerGrounded (bool grounded)
		{
			if (grounded) {
				lastJumpTime = Time.time;
				StopMotion (true);
			}
		}

		public override bool CanStart ()
		{
			return m_Controller.IsGrounded && (Time.time > lastJumpTime + m_RecurrenceDelay);
		}

		public override bool CanStop ()
		{
		
			return !m_Controller.IsGrounded && m_Rigidbody.velocity.y < 0.01f && Time.time > jumpTime + 0.2f; 
		}
	}
}