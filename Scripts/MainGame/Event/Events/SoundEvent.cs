using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class SoundEvent : EventBase {

		enum OperationType {
			PLAY = 1,
			STOP = 2,
			PAUSE = 3,
			STOP_ALLBGM = 4,
		}

		protected SoundManager soundManager;

		override public void Initialize() {
			base.Initialize();
			soundManager = SoundManager.GetInstance;
		}

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			SoundEventModel SoundEventModel = this.dataManager.soundEventModel.getModelById(eventManagerModel.eventChildId);
			this.Sound(SoundEventModel);
			this.callBack();
		}
		
		override public void OnTouchScreen() {
		}

		// ÉCÉxÉìÉgÇ…ÇÊÇÈëÄçÏ
		private void Sound(SoundEventModel soundEventModel) {
			switch (soundEventModel.operationType) {
				case (int)OperationType.PLAY:
					this.soundManager.Play(soundEventModel);
					break;
				case (int)OperationType.STOP:
					this.soundManager.Stop(soundEventModel);
					break;
				case (int)OperationType.PAUSE:
					this.soundManager.Pause(soundEventModel);
					break;
				case (int)OperationType.STOP_ALLBGM:
					this.soundManager.StopAllBGM(soundEventModel);
					break;
				default:
					break;
			}
		}
	}
}
