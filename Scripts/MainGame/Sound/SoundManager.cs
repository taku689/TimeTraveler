using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeTraveler;

namespace TimeTraveler.MainGame {
	public class SoundManager : MonoBehaviour {

		// シングルトンインスタンス
		private static SoundManager instance;

		public static SoundManager GetInstance {
			get {
				if (instance == null) {
					instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

					if (instance == null) {
						Debug.LogWarning("No " + typeof(SoundManager) + "object could be found");
					}
				}
				return instance;
			}
		}

		public enum SoundType {
			BGM = 1,
			SE = 2,
		}

		GameManager gameManager;

		[SerializeField]
		GameObject soundData;

		[SerializeField]
		bool muteMode;

		bool oldMuteMode = false;

		Dictionary<int, SoundObject> soundObject = new Dictionary<int, SoundObject>();

		const int SOUNDID_THRESHOLD = 100;

		public void Awake() {
			DontDestroyOnLoad(SoundManager.GetInstance);
		}

		public void Update() {
			if (oldMuteMode == false && muteMode == true) {
				MuteAll();
			}
			else if (oldMuteMode == true && muteMode == false) {
				CancelMuteAll();
			}
			oldMuteMode = muteMode;
		}

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			this.soundObject = new Dictionary<int, SoundObject>();

			SoundObject[] soundObjectArray = this.soundData.GetComponentsInChildren<SoundObject>();
			foreach (SoundObject soundObjectComponent in soundObjectArray) {
				this.soundObject.Add(soundObjectComponent.soundId, soundObjectComponent);
			}
		}

		// 再生(なんでも)
		public void Play(SoundEventModel soundEventModel) {
			this.soundObject[soundEventModel.soundId].Play(soundEventModel);
		}

		// 再生(簡易版)
		public void Play(int soundId) {
			bool isLoop = false;
			if (SoundType.BGM == this.GetSoundType(soundId)) {
				isLoop = true;
			}
			this.soundObject[soundId].Play(isLoop);
		}

		// 停止
		public void Stop(SoundEventModel soundEventModel) {
			this.soundObject[soundEventModel.soundId].Stop(soundEventModel);
		}

		// 全BGM停止
		public void StopAllBGM(SoundEventModel soundEventModel) {
			foreach (KeyValuePair<int, SoundObject> pair in this.soundObject) {
				if (SoundType.BGM == this.GetSoundType(pair.Key)) {
					pair.Value.Stop(soundEventModel);
				}
			}
		}

		// 一時停止
		public void Pause(SoundEventModel soundEventModel) {
			this.soundObject[soundEventModel.soundId].Pause(soundEventModel);
		}

		// 全ミュート
		public void MuteAll() {
			foreach (KeyValuePair<int, SoundObject> pair in this.soundObject) {
				pair.Value.Mute();
			}
		}

		// 全ミュート解除
		public void CancelMuteAll() {
			foreach (KeyValuePair<int, SoundObject> pair in this.soundObject) {
				pair.Value.CancelMute();
			}
		}

		// 再生確認
		public bool IsPlaying(int soundId) {
			return this.soundObject[soundId].IsPlaying();
		}

		// サウンドタイプ判定
		public SoundType GetSoundType(int soundId) {
			if (soundId >= SOUNDID_THRESHOLD) {
				return SoundType.SE;
			}
			else {
				return SoundType.BGM;
			}
		}
	}
}
