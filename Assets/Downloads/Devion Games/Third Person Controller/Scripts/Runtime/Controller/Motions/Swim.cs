using UnityEngine;

namespace DevionGames
{
	/// <summary>
	/// Note: Swimming will only work if the MotionTrigger/SwimTrigger size is set to 1, collider size can be any.
	/// </summary>
	public class Swim : MotionState
	{
		[SerializeField]
		private float m_HeightOffset=-1.57f;
		[SerializeField]
		private float m_OffsetSmoothing = 0.1f;
		[InspectorLabel("Trigger")]
		[SerializeField]
		private string m_TriggerName = "Ladder";

		private MotionTrigger m_Trigger;
		private float m_SmoothOffset;
		private float m_SmoothVelocity;

        private readonly float m_HeightAdjustment = 0f;
   
		public override void OnStart ()
		{
			m_Rigidbody.useGravity = false;
			m_Rigidbody.velocity = Vector3.zero;
			m_Controller.Velocity = Vector3.zero;
			Vector3 position = transform.position;
			position.y = m_Trigger.transform.position.y + m_HeightOffset;
			transform.position = position;
			m_Controller.IsGrounded = true;
			m_CapsuleCollider.height += m_HeightAdjustment;
			m_CapsuleCollider.center = new Vector3 (m_CapsuleCollider.center.x, m_CapsuleCollider.center.y + m_HeightAdjustment * 0.5f, m_CapsuleCollider.center.z);
		}

		public override void OnStop ()
		{
			m_Rigidbody.useGravity = true;
			m_CapsuleCollider.height -= m_HeightAdjustment;
			m_CapsuleCollider.center = new Vector3 (m_CapsuleCollider.center.x, m_CapsuleCollider.center.y - m_HeightAdjustment * 0.5f, m_CapsuleCollider.center.z);
	
		}

		public override bool CanStart ()
		{
			if (m_Trigger != null && transform.position.y < m_Trigger.transform.position.y + m_HeightOffset-0.1f) {
				return true;
			}
			return false;
		}

		public override bool CanStop ()
		{
			if (m_Trigger == null || transform.position.y > m_Trigger.transform.position.y + m_HeightOffset + 0.1f) {
				return true;
			}
			return false;
		}

		public override bool UpdateVelocity (ref Vector3 velocity)
		{

			if (!m_Controller.IsStepping) {
				Vector3 position = transform.position;
				m_SmoothOffset = Mathf.SmoothDamp (position.y, m_Trigger.transform.position.y + m_HeightOffset, ref m_SmoothVelocity, m_OffsetSmoothing);
				position.y = m_SmoothOffset;
				transform.position = position;
			}
			return true;
		}


		public override bool CheckGround ()
		{
			m_Controller.CheckStep ();
			return false;
		}

		public override bool UpdateAnimatorIK (int layer)
		{
			return false;
		}

		private void OnTriggerEnter(Collider other)
		{
			MotionTrigger trigger = other.GetComponent<MotionTrigger>();
			if (StartType == StartType.Automatic && trigger != null && (trigger.triggerName == m_TriggerName || trigger is SwimTrigger))
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