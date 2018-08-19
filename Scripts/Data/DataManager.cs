using UnityEngine;
using System.Collections;
using TimeTraveler.Model.User;

namespace TimeTraveler {
	public class DataManager : MonoBehaviour {

		public ScheneSwitchData scheneSwithData{ get; private set;}

		// ===Master===
		// Event
		public FieldModel fieldModel {get; private set;}
		public EventModel eventModel {get; private set;}
		public EventManagerModel eventManagerModel {get; private set;}
		public ShowImageEventModel showImageEventModel {get; private set;}
		public ScreenTextEventModel screenTextEventModel {get; private set;}
		public WindowTextEventModel windowTextEventMode {get; private set;}
		public LoadFieldEventModel loadFieldEventModel {get; private set;}
		public AnimationEventModel animationEventModel {get; private set;}
		public IntervalEventModel intervalEventModel {get; private set;}
		public MoveCharacterEventModel moveCharacterEventModel {get; private set;}
		public SoundEventModel soundEventModel { get; private set; }
		public CharacterActionEventModel characterActionEventModel {get; private set;}
		public WindowTextQuestionEventModel windowTextQuestionEventModel{get; private set;}
		public CommonEventModel commonEventModel{get; private set;}
		public WindowTextInputEventModel windowTextInputEventModel{get; private set;}

		// Common
		public CharacterModel characterModel {get; private set;}

		// ===User===
		public UserEventModel userEventModel {get; private set;}
		public UserFlagModel userFlagModel {get; private set;}
		public UserModel userModel {get; private set;}

		void Awake() {
			DontDestroyOnLoad(gameObject);
		}

		void Start () {
			this.loadMasterData();
		}

		public void loadMasterData() {
			this.fieldModel = new FieldModel();
			this.fieldModel.load(this);
			this.eventModel = new  EventModel();
			this.eventModel.load(this);
			this.eventManagerModel = new  EventManagerModel();
			this.eventManagerModel.load(this);
			this.showImageEventModel = new ShowImageEventModel();
			this.showImageEventModel.load(this);
			this.screenTextEventModel = new ScreenTextEventModel();
			this.screenTextEventModel.load(this);
			this.windowTextEventMode = new WindowTextEventModel();
			this.windowTextEventMode.load(this);
			this.loadFieldEventModel = new LoadFieldEventModel();
			this.loadFieldEventModel.load(this);
			this.animationEventModel = new AnimationEventModel();
			this.animationEventModel.load(this);
			this.intervalEventModel = new IntervalEventModel();
			this.intervalEventModel.load(this);
			this.moveCharacterEventModel = new MoveCharacterEventModel();
			this.moveCharacterEventModel.load(this);
			this.soundEventModel = new SoundEventModel();
			this.soundEventModel.load(this);
			this.characterModel = new CharacterModel();
			this.characterModel.load(this);
			this.characterActionEventModel = new CharacterActionEventModel();
			this.characterActionEventModel.load(this);
			this.windowTextQuestionEventModel = new WindowTextQuestionEventModel();
			this.windowTextQuestionEventModel.load(this);
			this.windowTextInputEventModel = new WindowTextInputEventModel();
			this.windowTextInputEventModel.load(this);
			this.commonEventModel = new CommonEventModel();
			this.commonEventModel.load (this);
		}

		public void loadUserData() {
			int userId = this.scheneSwithData.userId;
			this.userModel = new UserModel();
			this.userModel.loadOrCreate(this, userId);
			this.userFlagModel = new UserFlagModel();
			this.userFlagModel.loadOrCreate(this, userId);
			this.eventManagerModel.SetForceEventToFlag();
			this.userEventModel = new UserEventModel();
			this.userEventModel.loadOrCreate(this, userId);
		}

		public void loadUserDataForTitle() {
			this.userModel = new UserModel();
			this.userModel.loadOrCreate(this, 2000);
		}

		public void SaveUserData() {
			int userId = this.userModel.id;
			this.userModel.Save(userId);
			this.userEventModel.Save(userId);
			this.userFlagModel.Save(userId);
		}

		public void ResetUserData() {
			int userId = this.userModel.id;
			this.userModel.Reset(userId);
			this.userEventModel.Reset(userId);
		}

		public void SetScheneSwithData(ScheneSwitchData scheneSwithData) {
			this.scheneSwithData = scheneSwithData;
		}

		public void Destroy() {
			Destroy(gameObject);
		}
		
	}
}
