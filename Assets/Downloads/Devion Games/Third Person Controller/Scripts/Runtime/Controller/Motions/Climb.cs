using UnityEngine;

namespace DevionGames
{
    public class Climb : MotionState
	{
		[SerializeField]
		private LayerMask m_ClimbMask = Physics.DefaultRaycastLayers;
		[SerializeField]
		private float m_MinForwardInput;
		[SerializeField]
		private float m_MinHeight = 0.8f;
		[SerializeField]
		private float m_MaxHeight = 1.2f;
		[SerializeField]
		private float m_MaxDistance = 0.5f;
		[SerializeField]
		private Vector3 m_ExtraForce= Vector3.zero;
		[SerializeField]
		private Vector3 m_IKLeftHandOffset=new(0.266f,0.01f,0.05f);
		[SerializeField]
		private Vector3 m_IKRightHandOffset = new(0.266f, 0.01f, 0.05f);
		[SerializeField]
		private float m_IKWeightSpeed = 8f;
		[SerializeField]
		private bool m_UpdateLookAt;


		private float m_FinalHeight;
		private float m_IKWeight;
		private float m_IKCurrentWeight;

		private Vector3 m_IKLeftHand;
		private Quaternion m_IKLeftHandRotation;

		private Vector3 m_IKRightHand;
		private Quaternion m_IKRightHandRotation;

		public override void OnStart()
        {
			m_Controller.IsGrounded = false;
			
			m_Controller.Velocity = Vector3.zero;
			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.useGravity = false;
		
			m_CapsuleCollider.isTrigger = true;
			Ray ray = new Ray(transform.position + transform.forward * m_MaxDistance + Vector3.up * m_MaxHeight, Vector3.down);

			if (Physics.Raycast(ray, out RaycastHit hit, m_MaxHeight - m_MinHeight, m_ClimbMask))
			{
				Vector3 position = transform.position - transform.forward * 0.01f; ;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);

				if (Physics.Raycast(edgeRay, out RaycastHit edgeHit, m_MaxDistance, m_ClimbMask))
				{
					Ray rightHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight + transform.right.normalized * m_IKLeftHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);
					Ray leftHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight - transform.right.normalized * m_IKRightHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);

					if (Physics.Raycast(rightHeightRay, out RaycastHit rightHeightHit, m_MaxHeight - m_MinHeight, m_ClimbMask) && Physics.Raycast(leftHeightRay, out RaycastHit leftHeightHit, m_MaxHeight - m_MinHeight, m_ClimbMask))
					{
						m_IKLeftHand = leftHeightHit.point+transform.up.normalized*m_IKLeftHandOffset.y;
						m_IKRightHand = rightHeightHit.point+transform.up.normalized * m_IKRightHandOffset.y;


						m_IKLeftHandRotation = Quaternion.LookRotation(Vector3.forward, leftHeightHit.normal)*transform.rotation;
						m_IKRightHandRotation = Quaternion.LookRotation(Vector3.forward, rightHeightHit.normal)*transform.rotation;

						m_FinalHeight = leftHeightHit.point.y;
					}

				}
			}
		}

        public override void OnStop()
        {
			m_CapsuleCollider.isTrigger = false;
			m_Rigidbody.useGravity = true;
		}

        public override bool CanStart()
		{
			if (m_Controller.RelativeInput.z < m_MinForwardInput) return false;

			Ray ray = new Ray(transform.position+transform.forward * m_MaxDistance + Vector3.up * m_MaxHeight, Vector3.down);

			if (Physics.Raycast(ray, out RaycastHit hit, m_MaxHeight - m_MinHeight, m_ClimbMask))
			{
				Vector3 position = transform.position-transform.forward*0.01f;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);

				if (Physics.Raycast(edgeRay, out RaycastHit edgeHit, m_MaxDistance, m_ClimbMask))
				{
					Ray rightHeightRay = new Ray(transform.position+transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight + transform.right.normalized * m_IKLeftHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);
					Ray leftHeightRay = new Ray(transform.position+transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight - transform.right.normalized * m_IKRightHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);

					if (Physics.Raycast(rightHeightRay, out RaycastHit rightHeightHit, m_MaxHeight - m_MinHeight, m_ClimbMask) && Physics.Raycast(leftHeightRay, out RaycastHit leftHeightHit, m_MaxHeight - m_MinHeight, m_ClimbMask))
					{
						return true;
					}

				}
			}
			return false;
		}

        public override bool UpdateAnimatorIK(int layer)
        {

			m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_IKCurrentWeight);
			m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_IKLeftHand);

			m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_IKCurrentWeight);
			m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_IKLeftHandRotation);

			m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_IKCurrentWeight);
			m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_IKRightHand);

			m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_IKCurrentWeight);
			m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_IKRightHandRotation);
			return m_UpdateLookAt;
        }

        private void OnDrawGizmosSelected()
        {
			if (!enabled) return;
			Ray ray = new Ray(transform.position+transform.forward * m_MaxDistance + Vector3.up * m_MaxHeight, Vector3.down);

			if (DebugRay(ray, out RaycastHit hit, m_MaxHeight - m_MinHeight))
			{
				
				Vector3 position = transform.position;
				position.y = hit.point.y;
				Ray edgeRay = new Ray(position, transform.forward);

				if (Physics.Raycast(edgeRay, out RaycastHit edgeHit, m_MaxDistance))
				{
					Debug.DrawLine(edgeRay.origin, edgeHit.point, Color.green);


					Ray rightHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight + transform.right.normalized * m_IKRightHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);

					if (Physics.Raycast(rightHeightRay, out RaycastHit rightHeightHit, m_MaxHeight - m_MinHeight))
					{
						Debug.DrawLine(rightHeightRay.origin, rightHeightHit.point, Color.green);
					}
					else
					{
						Debug.DrawLine(rightHeightRay.origin, rightHeightRay.origin + Vector3.down * (m_MaxHeight - m_MinHeight), Color.red);
					}


					Ray leftHeightRay = new Ray(transform.position + transform.forward * edgeHit.distance + Vector3.up * m_MaxHeight - transform.right.normalized * m_IKLeftHandOffset.x + transform.forward * m_IKLeftHandOffset.z, Vector3.down);

					if (Physics.Raycast(leftHeightRay, out RaycastHit leftHeightHit, m_MaxHeight - m_MinHeight))
					{
						Debug.DrawLine(leftHeightRay.origin, leftHeightHit.point, Color.green);
					}
					else
					{
						Debug.DrawLine(leftHeightRay.origin, leftHeightRay.origin + Vector3.down * (m_MaxHeight - m_MinHeight), Color.red);
					}

				}
				else {
					Debug.DrawLine(ray.origin, ray.origin + Vector3.down * (m_MaxHeight - m_MinHeight), Color.red);
				}
			}

			
		}

		private bool DebugRay(Ray ray, out RaycastHit hit, float distance) {
			bool result = Physics.Raycast(ray, out hit, distance);
			Debug.DrawLine(ray.origin, ray.origin+ ray.direction * distance, result ? Color.green : Color.red);
			return result;
		}

        public override bool UpdateVelocity(ref Vector3 velocity)
        {
			if (!IsPlaying() ) {
				return false;
			}
			Vector3 rootMotion = m_Controller.RootMotionForce;
			rootMotion += transform.TransformDirection(m_ExtraForce);
			float force = m_Animator.GetFloat("Force");
			rootMotion += transform.forward * force;

			if (Mathf.Abs(transform.position.y - m_FinalHeight) < 0.05f)
			{
				rootMotion.y = 0f;
				Vector3 position = transform.position;
				position.y = Mathf.Lerp(position.y, m_FinalHeight, Time.fixedDeltaTime * 15f);

				transform.position = position;
			}

			m_IKCurrentWeight = Mathf.Lerp(m_IKCurrentWeight, m_IKWeight, Time.fixedDeltaTime * m_IKWeightSpeed);
			velocity = rootMotion;
			return false;
        }

        private void SetIKWeight(float weight)
		{
			m_IKWeight = weight;
		}

        public override bool UpdateRotation()
        {
            return false;
        }

        public override bool CheckGround()
		{
			return false;
		}

        public override bool CheckStep()
        {
            return false;
        }
    }
}