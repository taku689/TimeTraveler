using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{
	public class AnimationEvent : EventBase {

		/*
		enum AnimationState {
			INITIALIZE,
			PLAYING,
			FINISH
		}
		*/

		Dictionary<string, AnimationController> tagToAnimationControllerMap = new Dictionary<string, AnimationController>();
		AnimationController currentAnimationController;
		
		AnimationEventModel animationEventModel;

		override public void Initialize() {
			this.UpdateAsObservable()
				.Where (_ => this.currentAnimationController != null)
				.Subscribe(_ => this.SubScribeAnimationController());
		}

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			this.animationEventModel = this.dataManager.animationEventModel.getModelById(eventManagerModel.eventChildId);
			this.LoadAnimation(this.dataManager.animationEventModel.getModelById(eventManagerModel.eventChildId));
		}

		void LoadAnimation(AnimationEventModel animationEventModel) {
			if (this.tagToAnimationControllerMap.ContainsKey(animationEventModel.tag)) {
				currentAnimationController = this.tagToAnimationControllerMap[animationEventModel.tag];
				currentAnimationController.Launch(animationEventModel);
			} else {
				GameObject animation = Instantiate(Resources.Load (EVENT_ANIMATION_PATH + animationEventModel.prefab)) as GameObject;
				animation.transform.SetParent(this.transform, false);
				AnimationController animationController = animation.GetComponent<AnimationController>();

				this.tagToAnimationControllerMap.Add (animationEventModel.tag, animationController);
				currentAnimationController = animationController;

				animationController.Launch(animationEventModel);
			}
		}

		void SubScribeAnimationController() {
			if (this.currentAnimationController.currentAnimationState != AnimationController.AnimationState.AUTO_NEXT) return;
			int animationType = this.animationEventModel.animationType;
			if (animationType == (int) AnimationController.AnimationType.OUT || animationType == (int) AnimationController.AnimationType.DELETE || animationType == (int) AnimationController.AnimationType.ALL) {
				this.FinishEvent();
				return;
			}
			this.currentAnimationController = null;
			this.callBack();
		}

		void FinishEvent() {
			this.currentAnimationController.StateFinish();
			this.tagToAnimationControllerMap.Remove(animationEventModel.tag);
			this.currentAnimationController = null;
			this.callBack();
		}

		override public void OnTouchScreen() {
			int animationType = this.animationEventModel.animationType;
			if (this.currentAnimationController.currentAnimationState == AnimationController.AnimationState.FINISH) {
				this.FinishEvent();
			}

			if (animationType == (int) AnimationController.AnimationType.IN) {
				this.callBack();
			}
		}

	}
}
