using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{
	public class AnimationController : MonoBehaviour {
		
		public enum AnimationState {
			INITIALIZE,
			PLAYING,
			FINISH,
			AUTO_NEXT,
		}
		
		public enum AnimationType {
			ALL     = 1,
			IN      = 2,
			PLAY    = 3,
			REVERSE = 4,
			OUT     = 5,
			DELETE  = 6,
		}
		
		public AnimationState currentAnimationState;
		
		AnimationEventModel animationEventModel;
		
		Animator animator;
		
		void Initialize() {
			currentAnimationState = AnimationState.INITIALIZE;
			this.UpdateAsObservable()
				.Where (_ => this.currentAnimationState == AnimationState.PLAYING)
				.Subscribe(_ => this.PlayingAnimation());
		}
		
		public void Launch(AnimationEventModel animationEventModel) {
			this.animationEventModel = animationEventModel;
			this._Launch();
		}

		public void LoadAnimation() {
			if (this.animator == null) {
				this.Initialize();
				this.animator = this.gameObject.GetComponent<Animator>();
			}
			if (this.animationEventModel.withNextChildEvent && this.animationEventModel.animationType == (int)AnimationType.IN) {
				this.currentAnimationState = AnimationState.AUTO_NEXT;
			}

			this.animator.speed = 0;
		}
		
		
		void _Launch() {
			int animationType = this.animationEventModel.animationType;
			if (animationType == (int) AnimationType.ALL || animationType == (int) AnimationType.IN || animationType == (int) AnimationType.PLAY || animationType == (int) AnimationType.OUT) {
				this.LoadAnimation();
			}
			
			if (animationType == (int) AnimationType.ALL || animationType == (int) AnimationType.PLAY || animationType == (int) AnimationType.OUT || animationType == (int) AnimationType.REVERSE) {
				this.Play();
			}

			if (animationType == (int) AnimationType.DELETE) {
				this.Delete ();
			}
		}

		void Delete() {
			this.currentAnimationState = AnimationState.FINISH;
			if (this.animationEventModel.withNextChildEvent) {
				this.currentAnimationState = AnimationState.AUTO_NEXT;
			}
		}
		
		void Play() {
			this.currentAnimationState = AnimationState.PLAYING;
			this.animator.speed = (float)this.animationEventModel.animationSpeed;
			this.animator.enabled = true;
			if (this.animationEventModel.animationType == (int) AnimationType.REVERSE) {
				this.animator.StartPlayback();
				this.animator.speed = -(float)this.animationEventModel.animationSpeed;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && this.animationEventModel.animationType == (int) AnimationType.OUT) {
				this.currentAnimationState = AnimationState.FINISH;
				if (this.animationEventModel.withNextChildEvent) {
					this.currentAnimationState = AnimationState.AUTO_NEXT;
				}
			}
		}

		void PlayingAnimation() {
			int animationType = this.animationEventModel.animationType;
			if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && animationType != (int)AnimationType.REVERSE) return;
			if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.0f && animationType == (int)AnimationType.REVERSE) return;
			this.animator.enabled = false;
			this.currentAnimationState = AnimationState.FINISH;
			if (this.animationEventModel.withNextChildEvent && animationType == (int) AnimationType.PLAY ||
			    animationType == (int) AnimationType.REVERSE || animationType == (int) AnimationType.ALL) {
				this.currentAnimationState = AnimationState.AUTO_NEXT;
			}
		}

		public void StateFinish() {
			int animationType = this.animationEventModel.animationType;
			if (animationType == (int) AnimationType.ALL || animationType == (int) AnimationType.OUT || animationType == (int) AnimationType.DELETE) this.Out();
		}

		void Out() {
			Destroy (gameObject);
		}
		
	}
}
