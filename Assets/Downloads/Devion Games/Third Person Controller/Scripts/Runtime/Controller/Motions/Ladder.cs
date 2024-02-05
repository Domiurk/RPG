using UnityEngine;

namespace DevionGames
{
    public class Ladder : MotionState
    {
		[InspectorLabel("Trigger")]
		[SerializeField]
		private string m_TriggerName = "Ladder";
		private MotionTrigger m_Trigger;
		private bool m_StartMove;
        public override void OnStart()
        {
			GetComponent<CharacterIK>().enabled = false;
			m_Rigidbody.useGravity = false;
			m_Rigidbody.velocity = Vector3.zero;
			m_Controller.Velocity = Vector3.zero;
			m_Controller.IsGrounded = false;
			m_StartMove = false;
			Vector3 startPosition = m_Trigger.transform.position;
			startPosition.y = 0.186f;
			m_Animator.SetFloat("Forward Input", 0f);
			MoveToTarget(m_Transform, startPosition, m_Trigger.transform.rotation, 0.3f, delegate {m_StartMove = true; });
	
		}

        public override void OnStop()
        {
			m_Rigidbody.useGravity = true;
			GetComponent<CharacterIK>().enabled = true;
		}

        public override bool CanStart()
		{
			return m_Trigger != null && m_Controller.RawInput.z > 0f;
		}

        public override bool CanStop()
        {
            return (m_Trigger== null || m_Controller.IsGrounded && m_Controller.RawInput.z < 0f);
        }

		public override bool UpdateVelocity(ref Vector3 velocity)
		{
			if(m_StartMove)
				velocity = m_Controller.RootMotionForce;
			return false;
		}

        public override bool UpdateAnimator()
        {
			if (m_StartMove)
			{
				m_Animator.SetFloat("Forward Input", m_Controller.RawInput.z);
			}
            return false;
        }

        public override bool CheckGround()
        {
            return m_Controller.RawInput.z < 0f;
        }

        public override bool UpdateRotation()
        {
            return false;
        }


		private void OnTriggerEnter(Collider other)
		{
			MotionTrigger trigger = other.GetComponent<MotionTrigger>();
			if (StartType == StartType.Automatic && trigger != null && trigger.triggerName == m_TriggerName )
			{
				m_Trigger = trigger;

			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (m_Trigger == other.GetComponent<MotionTrigger>())
				m_Trigger = null;
		}
	}
}