using UnityEngine;
using System.Collections;

namespace TimeTraveler.MainGame{
	public class FlagEvent : EventBase {

		bool flag;
		int id;

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			if(flag) {
				dataManager.userFlagModel.FlagOn(id);
				var model = dataManager.userFlagModel.buildModelById(id);
				var ids = model.GetForceLauncEventIds();
				if (ids.Count > 0) {
					gameManager.eventManager.forceLaunchEventIdsByFlag = ids;
				}
			}
			else {
				dataManager.userFlagModel.FlagOff(id);
			}
			this.callBack();
		}

		public void SetFlag(bool isOn, int newId)
		{
			flag = isOn;
			id = newId;
		}
	}
}
