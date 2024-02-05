using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DevionGames.InventorySystem
{
    public class PlaceItem : MonoBehaviour
    {

		public LayerMask mask;
		public float maxHeightDiffrence = 1.0f;
		public float maxDistance = 10;
		public Key rotate = Key.E;

		public UnityEvent onPlace;
		private BoxCollider m_BoxCollider;
		private Transform m_Player;
		private bool m_CanPlaceChanged;
		private int m_Layer;
		private Keyboard keyboard;
		private Camera cameraMain;

		private void Start()
		{
			m_BoxCollider = GetComponent<BoxCollider>();

			keyboard = Keyboard.current;
			cameraMain = Camera.main;
			m_Player = InventoryManager.current.PlayerInfo.transform;
			m_Layer = gameObject.layer;
			gameObject.layer = 2;
			SetColor(Color.red);
		}

		private void Update()
		{
			if (keyboard[rotate].isPressed)
			{
				transform.Rotate(Vector3.up);
			}
			RaycastHit hit;
			Vector3 pos = transform.position;
			if (Physics.Raycast(cameraMain.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask))
			{
				pos = hit.point;
			}
			Vector3 diff = pos - m_Player.transform.position;
			float distance = diff.magnitude;
			if (distance > maxDistance)
			{
				pos = m_Player.transform.position + (diff / distance) * maxDistance;
			}

			float maxHeight = GetMaxCornerHeight();
			if (maxHeight > pos.y)
			{
				pos.y = GetMaxCornerHeight();
			}

			transform.position = pos;

			bool canPlace = CanPlace();
			if (canPlace != m_CanPlaceChanged)
			{
				m_CanPlaceChanged = canPlace;
				SetColor(canPlace ? Color.green : Color.red);
			}

			if (canPlace && Input.GetMouseButtonDown(0))
			{
				Build();
			}
		}

		protected virtual void Build()
		{
			SetColor(Color.white);
			gameObject.layer = m_Layer;
			onPlace?.Invoke();
			enabled = false;
		}

		private void SetColor(Color color)
		{
			Renderer[] renderers = GetComponentsInChildren<Renderer>(false);
			foreach (Renderer renderer in renderers)
			{
				renderer.material.color = color;
			}
		}

		private bool CanPlace()
		{
			Vector3 boxColliderCenter = m_BoxCollider.center;
			Vector3 boxColliderExtents = m_BoxCollider.size;
			bool res = true;
			for (int i = 0; i < 4; i++)
			{
				Vector3 ext = boxColliderExtents;
				ext.Scale(new Vector3((i & 1) == 0 ? 1 : -1, -1, (i & 2) == 0 ? 1 : -1) * 0.5f);
				Vector3 vertPositionLocal = boxColliderCenter + ext;
				Debug.DrawRay(transform.TransformPoint(vertPositionLocal) + Vector3.up * 0.2f, Vector3.down * maxHeightDiffrence);
				if (!Physics.Raycast(transform.TransformPoint(vertPositionLocal) + Vector3.up * 0.2f, Vector3.down, maxHeightDiffrence))
				{
					res = false;
				}
			}
			return res;
		}

		private float GetMaxCornerHeight()
		{
			Vector3 boxColliderCenter = m_BoxCollider.center;
			Vector3 boxColliderExtents = m_BoxCollider.size;
			float maxHeight = 0;
			for (int i = 0; i < 4; i++)
			{
				Vector3 ext = boxColliderExtents;
				ext.Scale(new Vector3((i & 1) == 0 ? 1 : -1, -1, (i & 2) == 0 ? 1 : -1) * 0.5f);
				Vector3 vertPositionLocal = boxColliderCenter + ext;
				RaycastHit hit;
				if (Physics.Raycast(transform.TransformPoint(vertPositionLocal) + Vector3.up, Vector3.down, out hit, Mathf.Infinity, mask))
				{
					if (hit.point.y > maxHeight)
					{
						maxHeight = hit.point.y;
					}
				}
			}
			return maxHeight;
		}
	}
}