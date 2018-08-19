using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class CommonEvent : EventBase {
		
		enum CommonType {
			TO_TWELVE = 1,
			TO_THIRTEEN = 2,
			ENABLE_SKIP_BUTTON = 3,
			DISBLE_SKIP_BUTION = 4,
		}
		
		override public void Initialize() {
			base.Initialize();
		}
		
		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			CommonEventModel commonEventModel = this.dataManager.commonEventModel.getModelById(eventManagerModel.eventChildId);
			this.LaunchByType(commonEventModel.type);
			this.callBack();
		}

		void LaunchByType(int type) {
			switch(type) {
			case (int)CommonType.TO_TWELVE:
				this.gameManager.SkipDateToTwelve();
				break;
			case (int)CommonType.TO_THIRTEEN:
				this.gameManager.SkipDateToThirteen();
				break;
			case (int)CommonType.ENABLE_SKIP_BUTTON:
				this.dataManager.userModel.buildModel().enableSkipButton = true;
				break;
			case (int)CommonType.DISBLE_SKIP_BUTION:
				this.dataManager.userModel.buildModel().enableSkipButton = false;
				break;
			default:
				Debug.LogError("undefined Event");
				break;
			}
		}

		override public void OnTouchScreen() {
		}
		
    }
}
