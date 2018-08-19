using UnityEngine;
using System.Collections.Generic;
using TimeTraveler.Model.User;
using System.Linq;

namespace TimeTraveler.MainGame{
	public class EventManager : MonoBehaviour {

		public enum eventType {
			SHOW_IMAGE       	 = 1,
			SCREEN_TEXT      	 = 2,
			WINDOW_TEXT      	 = 3,
			LOAD_FIELD       	 = 4,
			ANIMATION        	 = 5,
			INTERVAL         	 = 6,
			MOVE_CHARACTER   	 = 7,
			SOUND                = 8,
			CHARACTER_ACTION 	 = 9,
			WINDOW_TEXT_QUESTION = 10,
			COMMON 				 = 11,
			WINDOW_TEXT_INPUT    = 12,
			FLAG_ON    			 = 13,
			FLAG_OFF    		 = 14,
		}

		List<EventModel> eventModelList;
		UserEventModel userEventModel;
		EventModel currentEvent;
		EventBase currentEventChild;
		public List<int> forceLaunchEventIdsByFlag = new List<int>();

		const int NEW_GAME_EVENT = 1000001;
		GameManager gameManager;
		DataManager dataManager;

		[SerializeField]
		ShowImageEvent showImageEvent;
		[SerializeField]
		ScreenTextEvent screenTextEvent;
		[SerializeField]
		WindowTextEvent windowTextEvent;
		[SerializeField]
		LoadFieldEvent loadFieldEvent;
		[SerializeField]
		AnimationEvent animationEvent;
		[SerializeField]
		IntervalEvent intervalEvent;
		[SerializeField]
		MoveCharacterEvent moveCharacterEvent;
		[SerializeField]
		SoundEvent soundEvent;
		[SerializeField]
		CharacterActionEvent characterActionEvent;
		[SerializeField]
		WindowTextQuestionEvent windowTextQuestionEvent;
		[SerializeField]
		CommonEvent commonEvent;
		[SerializeField]
		WindowTextInputEvent windowTextInputEvent;
		FlagEvent flagEvent = new FlagEvent();


		[SerializeField]
		List<int> toAprilThirteenEvents;
		[SerializeField]
		List<int> toAprilTwelveEvents;

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			this.dataManager = gameManager.dataManager;
			showImageEvent.Initialize();
			screenTextEvent.Initialize();
			windowTextEvent.Initialize();
			loadFieldEvent.Initialize();
			animationEvent.Initialize();
			intervalEvent.Initialize();
			moveCharacterEvent.Initialize();
			soundEvent.Initialize();
			windowTextQuestionEvent.Initialize();
			commonEvent.Initialize();
			windowTextInputEvent.Initialize();
		}

		public void LaunchNewGameEvent() {
			this.LaunchEvent(new List<int>(){NEW_GAME_EVENT});
		}

		public void LaunchSkipDateEvent() {
			UserModel userModel = this.dataManager.userModel.buildModel();
			if (userModel.date == (int)EventManagerModel.DateFlag.AprilTwelve) {
				this.LaunchSkipAprilThirteen(userModel);
			} else {
				this.LaunchSkipAprilTwelve(userModel);
			}
		}

		void LaunchSkipAprilThirteen(UserModel userModel) {
			this.LaunchEvent (this.toAprilThirteenEvents);
		}

		void LaunchSkipAprilTwelve(UserModel userModel) {
			this.LaunchEvent (this.toAprilTwelveEvents);
        }

        public void LaunchLoadGameEvent() {
		}

		public void LaunchEvent(List<int> eventIds) {
			List<UserEventModel> userEventModels = this.dataManager.userEventModel.buildModelsByIds(eventIds);
			this.userEventModel = this.GetLaunchAbleEvent(userEventModels);
			if (this.userEventModel != null) {
				this.eventModelList = this.userEventModel.eventModelList;
				this.LaunchNextEventChild();
			} else {
				this.gameManager.OnFinishEvent(new List<int>());
			}
		}

		UserEventModel GetLaunchAbleEvent (List<UserEventModel> userEventModels) {
			UserEventModel userEventModel = null;
			foreach(UserEventModel _userEventModel in userEventModels) {
				if(_userEventModel.canLaunchEvent()) {
					userEventModel = _userEventModel;
					break;
				}
			}
			return userEventModel;
		}

		public void LaunchNextEventChild() {
			if (this.eventModelList.Count == 0) {
				this.FinishEvent();
				return;
			}

			this.currentEvent = this.eventModelList[0];
			this.eventModelList.RemoveAt(0);
			this.currentEventChild = this.GetEventChild(this.currentEvent);
			//Debug.Log ("Launch Event CHILD ID:" + this.currentEvent.id.ToString() + ", EVENT TYPE:" + this.currentEvent.eventType.ToString());
			this.currentEventChild.Launch(LaunchNextEventChild, dataManager, this.currentEvent, this.gameManager);
		}

		EventBase GetEventChild(EventModel eventModel) {
			switch(eventModel.eventType) {
			case (int) eventType.SHOW_IMAGE:
				return this.showImageEvent;
			case (int) eventType.SCREEN_TEXT:
				return this.screenTextEvent;
			case (int) eventType.WINDOW_TEXT:
				return this.windowTextEvent;
			case (int) eventType.LOAD_FIELD:
				return this.loadFieldEvent;
			case (int) eventType.ANIMATION:
				return this.animationEvent;
			case (int) eventType.INTERVAL:
				return this.intervalEvent;
			case (int) eventType.MOVE_CHARACTER:
				return this.moveCharacterEvent;
			case (int) eventType.SOUND:
				return this.soundEvent;
			case (int) eventType.CHARACTER_ACTION:
				return this.characterActionEvent;
			case (int) eventType.WINDOW_TEXT_QUESTION:
				return this.windowTextQuestionEvent;
			case (int) eventType.COMMON:
				return this.commonEvent;
			case (int) eventType.WINDOW_TEXT_INPUT:
				return this.windowTextInputEvent;
			case (int) eventType.FLAG_ON:
				flagEvent.SetFlag(true, eventModel.eventChildId);
				return flagEvent;
			case (int) eventType.FLAG_OFF:
				flagEvent.SetFlag(false, eventModel.eventChildId);
				return flagEvent;
			default:
				Debug.LogError("undefined Event");
				return null;
			}
		}

		void FinishEvent() {
			this.userEventModel.Seen();
			this.showImageEvent.HideImage();
			List<int> forceLaunchEventIds = this.userEventModel.GetForceLauncEventIds();
			if (forceLaunchEventIdsByFlag.Count > 0) {
				forceLaunchEventIds.AddRange(forceLaunchEventIdsByFlag);
				forceLaunchEventIdsByFlag = new List<int>();
			}
			this.gameManager.OnFinishEvent(forceLaunchEventIds);
		}

		public void OnTouchScreen(InteractionEvent interactionEvent) {
			if (this.currentEventChild != null) {
				this.currentEventChild.OnTouchScreen();
			}
		}
	}
}
