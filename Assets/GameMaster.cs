using UnityEngine;

namespace Xeltica.Kurutto
{
	[RequireComponent(typeof(AudioSource))]
	public class GameMaster : Singleton<GameMaster>
	{
		private bool isPlaying;
		public bool IsPlaying
		{
			get { return isPlaying; }
			set { OnIsPlayingChanged(isPlaying, value); isPlaying = value; }
		}

		public void TogglePlay() => IsPlaying = !IsPlaying;

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

		[SerializeField]
		GameObject ball;
		public GameObject Ball => ball;

		AudioSource aud;

		[SerializeField]
		AudioClip bgmEditor;

		[SerializeField]
		AudioClip bgmPlaying;

		[SerializeField]
		AudioClip sfxStart;

		[SerializeField]
		MapBase[] maps;

		MapBase currentMap;

		GameObject currentBall;

		private void Start()
		{
			LoadMapById(0);
			aud = GetComponent<AudioSource>();
			aud.clip = bgmEditor;
			aud.loop = true;
			aud.Play();
		}

		private void Update()
		{
		}

		public void LoadMapById(int id)
		{
			if (id >= maps.Length)
				return;

			if (currentMap != null)
				Destroy(currentMap.gameObject);
			if (currentBall != null)
				Destroy(currentBall);

			currentMap = Instantiate(maps[id]);

			currentMap.PopulateMapObject();
			currentBall = Instantiate(ball, currentMap.BallLocation, Quaternion.Euler(0, 0, 0));
		}

		void OnIsPlayingChanged(bool oldValue, bool newValue)
		{
			var pos = aud.timeSamples;
			if (newValue)
			{
				currentBall.AddComponent<Rigidbody2D>();
				aud.clip = bgmPlaying;
				aud.PlayOneShot(sfxStart);
			}
			else
			{
				Destroy(currentBall.GetComponent<Rigidbody2D>());
				aud.clip = bgmEditor;
				currentBall.transform.position = currentMap.BallLocation;
			}
			aud.timeSamples = pos;
			aud.Play();
		}
	}

}