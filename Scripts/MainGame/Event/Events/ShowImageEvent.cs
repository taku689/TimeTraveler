using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class ShowImageEvent : EventBase {

		[SerializeField]
		SpriteRenderer spriteRenderer;

		ShowImageEventModel showImageEventModel;

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			ShowImageEventModel showImageEventModel = this.dataManager.showImageEventModel.getModelById(eventManagerModel.eventChildId);
			this.showImageEventModel = showImageEventModel;
			this.ShowImage(this.showImageEventModel.imageName);
			if (this.showImageEventModel.withNextChildEvent) {
				this.callBack();
			}
		}

		void ShowImage(string imageName){
			if (imageName == "none") {
				this.HideImage();
				return;
			}
			Sprite sprite  = Resources.Load<Sprite>(EVENT_IMAGE_PATH + imageName);
			this.spriteRenderer.sprite = sprite;
			this.spriteRenderer.enabled = true;
		}

		public void HideImage() {
			this.spriteRenderer.sprite = null;
			this.spriteRenderer.enabled = false;
		}

		override public void OnTouchScreen() {
			if (!this.showImageEventModel.notHide) {
				this.HideImage();
			}
			this.callBack();
		}
		
	}
}
