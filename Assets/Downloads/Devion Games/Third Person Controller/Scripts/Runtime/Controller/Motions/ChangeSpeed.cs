using UnityEngine;

namespace DevionGames
{
	public class ChangeSpeed : MotionState
	{
		[SerializeField]
		private float m_SpeedMultiplier = 2f;

		private float m_PrevSpeedMutiplier;

		public override void OnStart ()
		{
			m_PrevSpeedMutiplier = m_Controller.SpeedMultiplier;
			m_Controller.SpeedMultiplier = m_SpeedMultiplier;
		}

		public override void OnStop ()
		{
			m_Controller.SpeedMultiplier = m_PrevSpeedMutiplier;


		}

		public override bool CanStop ()
		{
			return StopType != StopType.Automatic;
		}
	}
}