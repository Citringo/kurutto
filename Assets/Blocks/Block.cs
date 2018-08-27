using System.Linq;
using UnityEngine;

namespace Xeltica.Kurutto
{
	public abstract class Block : MonoBehaviour
	{
		public abstract void OnTrigger(Collision2D collision);

		public void OnCollisionStay2D(Collision2D collision)
		{
			OnTrigger(collision);
		}

		/// <summary>
		/// 叩くと回転するロジック
		/// </summary>
		public virtual void Update()
		{
			if (!GameMaster.Instance.IsPlaying && (IsClicked() || IsTapped()))
			{
				transform.Rotate(Vector3.forward * 90);
				GameMaster.Instance.PlayClick();
			}
		}

		bool IsClicked()
		{
			if (!Input.GetMouseButtonDown(0))
				return false;

			return  Physics2D.Raycast(GameMaster.Instance.Camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).collider?.gameObject == gameObject;
	//		return Physics2D.OverlapPointAll(GameMaster.Instance.Camera.ScreenToWorldPoint(Input.mousePosition)).Any(c => c.gameObject == this);
		}

		bool IsTapped()
		{
			return Input.touches
				.Where(t => t.phase == TouchPhase.Began)
				.Select(t => Physics2D.Raycast(GameMaster.Instance.Camera.ScreenToWorldPoint(t.position), Vector2.zero))
				.Any(info => info.collider.gameObject == this);
		}

	}
}