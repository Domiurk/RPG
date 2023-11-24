using UnityEngine;

namespace DevionGames
{
	public class ChangeHeight : MotionState
	{
		[SerializeField]
		private float m_HeightAdjustment = -0.8f;

		public override void OnStart ()
		{
			m_CapsuleCollider.height += m_HeightAdjustment;
			m_CapsuleCollider.center = new Vector3 (m_CapsuleCollider.center.x, m_CapsuleCollider.center.y + m_HeightAdjustment * 0.5f, m_CapsuleCollider.center.z);
		}

		public override void OnStop ()
		{
			m_CapsuleCollider.height -= m_HeightAdjustment;
			m_CapsuleCollider.center = new Vector3 (m_CapsuleCollider.center.x, m_CapsuleCollider.center.y - m_HeightAdjustment * 0.5f, m_CapsuleCollider.center.z);
		}
	}
}