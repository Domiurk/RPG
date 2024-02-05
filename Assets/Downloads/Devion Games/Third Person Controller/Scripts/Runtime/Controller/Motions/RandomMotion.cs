using UnityEngine;

namespace DevionGames
{
	public class RandomMotion : SimpleMotion
	{
		[SerializeField]
		private string[] m_DestinationStates;

		public override string GetDestinationState ()
		{
			return m_DestinationStates [Random.Range (0, m_DestinationStates.Length)];
		}
	}
}