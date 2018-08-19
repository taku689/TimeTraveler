using UnityEngine;
using System.Collections;

namespace TimeTraveler.MainGame {
		public class SoundObject : MonoBehaviour {

		public int soundId;

		AudioSource audioSource;

		const float DETECTION_TIME = 0.001f;

		float initialVol;

		public void Awake() {
			this.audioSource = this.GetComponent<AudioSource>();
			initialVol = this.audioSource.volume;
		}

		public void Play (bool isLoop) {
			this.audioSource.loop = isLoop;
			this.audioSource.Play();
		}

		public void Play(SoundEventModel soundEventModel) {
			this.audioSource.loop = soundEventModel.isLoop;

			// FadeTimeが設定されていない場合すぐに再生
			if (soundEventModel.fadeTime == 0) {
				this.audioSource.Play();
				return;
			}

			StartCoroutine("Fadein", soundEventModel);
		}

		public void Stop (SoundEventModel soundEventModel) {
			if (!this.IsPlaying()) {
				return;
			}

			// FadeTimeが設定されていない場合すぐに停止
			if (soundEventModel.fadeTime == 0) {
				this.audioSource.Stop();
				return;
			}

			StartCoroutine("Fadeout", soundEventModel);
		}

		public void Stop() {
			if (this.IsPlaying()) { 
				this.audioSource.Stop();
			}
		}

		public void Pause(SoundEventModel soundEventModel) {
			this.audioSource.Pause();
		}

		public void Mute() {
			this.audioSource.volume = 0;
		}

		public void CancelMute()
		{
			this.audioSource.volume = initialVol;
		}

		public bool IsPlaying() {
			return this.audioSource.isPlaying;
		}

		// フェードイン
		IEnumerator Fadein(SoundEventModel soundEventModel) {
			float currentTime = 0.0f;
			float waitTime = DETECTION_TIME;
			float originalVol = audioSource.volume;

			// msecに修正
			float duration = (float)(soundEventModel.fadeTime / 1000.0f);

			audioSource.volume = 0.0f;
			audioSource.Play();

			while (duration > currentTime) {
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Clamp01(originalVol * currentTime / duration);
				yield return new WaitForSeconds(waitTime);
			}
		}

		// フェードアウト
		IEnumerator Fadeout(SoundEventModel soundEventModel) {
			float currentTime = 0.0f;
			float waitTime = DETECTION_TIME;
			float originalVol = audioSource.volume;

			// msecに修正
			float duration = (float)(soundEventModel.fadeTime / 1000.0f);
			while (duration > currentTime) {
				currentTime += Time.deltaTime;
				audioSource.volume = Mathf.Clamp01(originalVol * (duration - currentTime) / duration);
				yield return new WaitForSeconds(waitTime);
			}

			audioSource.Stop();

			// 次回再生時のためにVol戻し
			audioSource.volume = originalVol;
		}
	}
}