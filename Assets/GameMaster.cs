using UnityEngine;

namespace Xeltica.Kurutto
{
	public class GameMaster : Singleton<GameMaster>
	{
		public bool IsPlaying { get; set; }

		public void Play() => IsPlaying = true;
		public void Stop() => IsPlaying = false;

		[SerializeField]
		Block staticBlock;
		public Block StaticBlock => staticBlock;

		[SerializeField]
		Block forceBlock;
		public Block ForceBlock => forceBlock;

		[SerializeField]
		Block warpBlock;
		public Block WarpBlock => warpBlock;

		[SerializeField]
		Block accelerationBlock;
		public Block AccelerationBlock => accelerationBlock;

		[SerializeField]
		Camera camera;
		public Camera Camera => camera;

		private void Update()
		{

		}
	}

}