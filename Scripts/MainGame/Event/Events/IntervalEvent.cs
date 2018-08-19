using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class IntervalEvent : EventBase {
		
		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			IntervalEventModel intervalEventModel = this.dataManager.intervalEventModel.getModelById(eventManagerModel.eventChildId);
			StartCoroutine("waitTime", intervalEventModel.time);
		}
		
		IEnumerator waitTime(double waitTime) {
			yield return new WaitForSeconds((float)waitTime);
			this.callBack();
		}
		
		override public void OnTouchScreen() {
		}
		
	}
}
