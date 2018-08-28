using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xeltica.Kurutto
{
	public class ForceBlock : Block
	{
		public override void OnTrigger(Collision2D collision)
		{
			var rb = collision.gameObject.GetComponent<Rigidbody2D>();

			if (rb == null) return;

			rb.AddForce(transform.up * 500);
			GameMaster.Instance.PlayForce();
		}
	}
}