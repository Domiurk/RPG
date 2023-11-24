using UnityEngine;

namespace DevionGames
{
	public class Push : MotionState
	{
		[SerializeField]
		private float m_Distance = 0.5f;
	
		private PushableObject m_PushableObject;
		private RaycastHit hitInfo;

		public override bool CanStart ()
		{
			return CheckPush (m_Distance);
		}

		public override bool CanStop ()
		{
			return !CheckPush (m_Distance + 0.3f) && m_InPosition;
		}

		public override void OnStart ()
		{
			Vector3 targetPosition = hitInfo.point + hitInfo.normal * m_Distance;
			targetPosition.y = m_Transform.position.y;
			MoveToTarget (m_Transform, targetPosition, Quaternion.LookRotation (-hitInfo.normal), 0.5f, delegate() {
				m_PushableObject.StartMove (m_Controller);
			});
		}

		public override void OnStop ()
		{
			m_PushableObject.StopMove ();
		}

		public override bool UpdateVelocity (ref Vector3 velocity)
		{
			if (!m_InPosition) {
				velocity = Vector3.zero;
				return false;
			}

			Vector3 movePosition = velocity * Time.deltaTime / 1.15f;
			if (!m_PushableObject.Move (m_PushableObject.transform.position + movePosition)) {
				velocity = Vector3.zero;
				return false;
			}
			return true;
		}

		public override bool UpdateAnimatorIK (int layer)
		{
			if (m_PushableObject != null) {
				Vector3 leftHandPosition = m_Animator.GetIKPosition (AvatarIKGoal.LeftHand) + m_PushableObject.leftHandOffset;
				m_Animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandPosition);
				m_Animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);

				Vector3 rightHandPosition = m_Animator.GetIKPosition (AvatarIKGoal.RightHand) + m_PushableObject.rightHandOffset;
				m_Animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandPosition);
				m_Animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
			}
			return false;
		}

		public override bool UpdateRotation ()
		{
			return false;
		}

		private bool CheckPush (float distance)
		{
			Vector3 direction = m_Controller.LookRotation * (m_Controller.IsAiming ? m_Controller.RelativeInput : m_Controller.RawInput);
			Ray ray = new Ray (transform.position + new Vector3 (0f, m_CapsuleCollider.height * 0.5f, 0f), direction.normalized);
			if (Physics.Raycast (ray, out hitInfo, distance)) {

				m_PushableObject = hitInfo.transform.GetComponent<PushableObject> ();
				if (m_PushableObject != null) {
					return true;
				}
			}
			return false;
		}
	}
}