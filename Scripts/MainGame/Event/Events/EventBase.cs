using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class EventBase : MonoBehaviour {

		protected string EVENT_IMAGE_PATH = "Img/Event/";
		protected string EVENT_ANIMATION_PATH = "Prefab/Animation/";
		protected DataManager dataManager;
		protected GameManager gameManager;

		public delegate void OnFinishEvent();

		protected OnFinishEvent callBack;

		virtual public void Initialize() {
		}

		virtual public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			this.callBack = callBack;
			this.dataManager = dataManager;
			this.gameManager = gameManager;
		}

		virtual public void OnTouchScreen() {
		}
		
	}
}
