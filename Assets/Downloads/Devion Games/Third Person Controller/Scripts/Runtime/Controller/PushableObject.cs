using UnityEngine;

namespace DevionGames
{
	public class PushableObject : MonoBehaviour
	{
		
		public Vector3 leftHandOffset;
		public Vector3 rightHandOffset;
		private Rigidbody m_Rigidbody;
		private Transform m_Transform;

		private Vector3 m_MoveDirection;
		private bool m_CanMove = true;
		private bool m_Moving;
		private ThirdPersonController m_Controller;

		private void Start ()
		{
			m_Rigidbody = GetComponent<Rigidbody> ();	
			m_Transform = transform;
		}

		public void StartMove (ThirdPersonController controller)
		{
			m_CanMove = true;
			m_MoveDirection = controller.transform.forward;
			m_MoveDirection.y = 0f;
			m_Moving = true;
			m_Controller = controller;
		}

		public void StopMove ()
		{
			m_Moving = false;
		}

		public bool Move (Vector3 position)
		{
			m_Rigidbody.MovePosition (position);
			return m_CanMove;
		}

		private void OnCollisionStay (Collision collision)
		{
			if (m_Moving) {
				foreach (ContactPoint p in collision.contacts) {
					Vector3 direction = p.normal;
					if (p.otherCollider.transform != m_Controller.transform && p.point.y > m_Transform.position.y + 0.1f && Vector3.Dot (direction, m_MoveDirection) < -0.2) {
						
						m_CanMove = false;
						break;
					}
				}
			}
		}

	}
}