using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

		[Header("Block")]

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

		AudioSource aud;

		[Header("BGM")]

		[SerializeField]
		AudioClip bgmEditor;

		[SerializeField]
		AudioClip bgmPlaying;

		[Header("SFX")]

		[SerializeField]
		AudioClip sfxStart;

		[SerializeField]
		AudioClip sfxCancel;

		[SerializeField]
		AudioClip sfxClear;

		[SerializeField]
		AudioClip sfxFailed;

		[SerializeField]
		AudioClip sfxClick;

		[Header("GUI")]

		[SerializeField]
		GameObject clearGui;
		[SerializeField]
		GameObject failedGui;

		[Header("Maps")]

		[SerializeField]
		MapBase[] maps;


		[Header("Misc")]

		[SerializeField]
		Camera camera;
		public Camera Camera => camera;

		[SerializeField]
		GameObject ball;
		public GameObject Ball => ball;


		MapBase currentMap;

		GameObject currentBall;

		float knockOutTimer;

		Vector3 prevBallPos;

		bool clearGuiIsShowing, failedGuiIsShowing;

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
			if (IsPlaying)
			{
				if (currentBall.transform.position.y < -12)
				{
					ShowClearGui();
				}
				else if ((distance = Vector3.Distance(currentBall.transform.position, prevBallPos)) < 0.1)
				{
					knockOutTimer += Time.deltaTime;
				}
				else
				{
					knockOutTimer = 0;
				}

				if (knockOutTimer > 1)
				{
					ShowFailedGui();
				}
				prevBallPos = currentBall.transform.position;
			}
		}

		float distance;

		private void OnGUI()
		{
			GUILayout.Label($"{knockOutTimer} {distance}");
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
				knockOutTimer = 0;
				Destroy(currentBall.GetComponent<Rigidbody2D>());
				aud.clip = bgmEditor;
				aud.PlayOneShot(sfxCancel);
				currentBall.transform.position = currentMap.BallLocation;
			}
			aud.timeSamples = pos;
			aud.Play();
		}

		void ShowClearGui()
		{
			if (clearGuiIsShowing) return;
			clearGui.SetActive(true);
			aud.PlayOneShot(sfxClear);

			clearGuiIsShowing = true;
			StartCoroutine(ClearCoroutine());
		}

		void ShowFailedGui()
		{
			if (failedGuiIsShowing) return;
			failedGui.SetActive(true);
			aud.PlayOneShot(sfxFailed);

			failedGuiIsShowing = true;

			StartCoroutine(FailedCoroutine());
		}

		void HideClearGui()
		{
			clearGui.SetActive(false);
			clearGuiIsShowing = false;
		}

		void HideFailedGui()
		{
			failedGui.SetActive(false);
			failedGuiIsShowing = false;
		}

		IEnumerator ClearCoroutine()
		{
			yield return new WaitForSeconds(3);
			SceneManager.LoadScene("Main");
		}

		IEnumerator FailedCoroutine()
		{
			yield return new WaitForSeconds(3);
			HideFailedGui();
			IsPlaying = false;
		}

		public void PlayClick() => aud.PlayOneShot(sfxClick);


	}

}