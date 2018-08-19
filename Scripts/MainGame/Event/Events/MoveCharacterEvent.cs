using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;

namespace TimeTraveler.MainGame{
	public class MoveCharacterEvent : EventBase {

		MoveCharacterEventModel moveCharacterEventModel;
		
		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			this.moveCharacterEventModel = this.dataManager.moveCharacterEventModel.getModelById(eventManagerModel.eventChildId);
			this.gameManager.MoveCharacterByEvent(this.moveCharacterEventModel)
				.Subscribe( _ => { ;}, () => { this.OnMoved(); });
		}

		void OnMoved() {
			if (this.moveCharacterEventModel.isDisappearAfterMoved) {
				this.gameManager.RemoveCharacterByEvent(this.moveCharacterEventModel);
			}
			this.callBack();
		}
		
	}
}
