using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TimeTraveler.MainGame{
	public class LoadFieldEvent : EventBase {
		
		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			LoadFieldEventModel loadFieldEventModel = this.dataManager.loadFieldEventModel.getModelById(eventManagerModel.eventChildId);
			this.gameManager.LoadFieldByEvent(loadFieldEventModel);
			if (loadFieldEventModel.withNextChildEvent) {
				this.callBack();
			}
		}
		
		override public void OnTouchScreen() {
			this.callBack();
		}
		
	}
}
