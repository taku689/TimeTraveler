using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeTraveler;
using UniRx;
using TimeTraveler.Model.User;
using UnityEngine.UI;

namespace TimeTraveler.MainGame {
	public class GameManager : MonoBehaviour {

		[SerializeField]
		InteractionEvent interactoinEvent;
		[SerializeField]
		FieldManager _fieldManager;
		[SerializeField]
		EventManager _eventManager;
		[SerializeField]
		GameObject skipDate;
		[SerializeField]
		Sprite twelve;
		[SerializeField]
		Sprite thirteen;
		[SerializeField]
		GameObject date;
		[SerializeField]
		Sprite twelveDate;
		[SerializeField]
		Sprite thirteenDate;
		[SerializeField]
		SaveLoad saveScreen;
		[SerializeField]
		SaveLoad loadScreen;
		[SerializeField]
		GameObject saveConfirmModal;
		[SerializeField]
		GameObject loadConfirmModal;
        
        public DataManager dataManager{get; private set;}
		public GameStateManager gameStateManager { get; private set;}
		public InteractionManager interactionManager { get; private set;}

		public FieldManager fieldManager { get {return this._fieldManager;}}
		public EventManager eventManager {get {return this._eventManager;}}
		public SoundManager soundManager { get; private set; }

		int saveLoadNum;

		void Start() {
			Application.targetFrameRate = 60;
			this.dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
			this.eventManager.Initialize(this);
			this.fieldManager.Initialize(this);
			this.soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
			this.soundManager.Initialize(this);

			this.interactionManager = new InteractionManager(this.interactoinEvent, this.fieldManager.cellManager, this.eventManager);
			this.gameStateManager = new GameStateManager(GameStateManager.gameState.DEMO, this);
			this.dataManager.loadUserData();

			// for debug
			if (this.dataManager.scheneSwithData == null) {
				this.dataManager.loadMasterData();
				this.dataManager.loadUserData();
				this.NewGame();
				return;
			}

			if (this.dataManager.scheneSwithData.loadType == ScheneSwitchData.LoadType.NewGame) {
				this.NewGame();
			} else {
				this.LoadGame();
			}
		}

		// TODO fix later
		void NewGame() {
			this.dataManager.ResetUserData();
			this.eventManager.LaunchNewGameEvent();
		}

		// TODO fix later
		void LoadGame() {
			this.LoadFieldByEvent(this.dataManager.loadFieldEventModel.buildByUserData());
			this.soundManager.Play(2);
			this.SetUIButton();
			this.gameStateManager.SetMoveState();
		}

		public void OnFinishEvent(List<int> forceLaunchEventIds) {
			this.SetUIButton();
			this.fieldManager.InitializeFieldObjects();
			if (forceLaunchEventIds.Count != 0) {
				this.LaunchEventByField(forceLaunchEventIds);
			} else {
				this.gameStateManager.SetMoveState();
			}
		}

		public void LoadFieldByEvent(LoadFieldEventModel loadFieldEventModel) {
			this.fieldManager.LoadField(loadFieldEventModel);
		}

		public IObservable<bool> MoveCharacterByEvent(MoveCharacterEventModel moveCharacterEventModel) {
			return this.fieldManager.MoveCharacterByEvent(moveCharacterEventModel);
		}

		public void RemoveCharacterByEvent(MoveCharacterEventModel moveCharacterEventModel) {
			this.fieldManager.RemoveCharacterByEvent(moveCharacterEventModel);
		}

		public void LaunchEventByField(List<int> eventIds) {
			this.gameStateManager.SetMessageState();
			this.eventManager.LaunchEvent(eventIds);
		}

		public void LaunchEventByEvent(List<int> eventIds) {
			this.gameStateManager.SetMessageState();
			this.eventManager.LaunchEvent(eventIds);
		}

		// TODO fix later
		public void ToTitle() {
			Application.LoadLevel("Opening");
			this.dataManager.Destroy();
		}

		public void Save() {
			this.SetUserData(saveLoadNum);
			this.dataManager.SaveUserData();
			saveScreen.UpdateTitle(this.dataManager.userModel.buildModel());
			this.saveConfirmModal.SetActive(false);
		}

		public void SkipDate() {
			this.gameStateManager.SetMessageState();
			this.eventManager.LaunchSkipDateEvent();
		}

		public void SkipDateToTwelve() {
			UserModel userModel = this.dataManager.userModel.buildModel();
			userModel.GoAprilTwelve();
		}

		public void SkipDateToThirteen() {
			UserModel userModel = this.dataManager.userModel.buildModel();
			userModel.GoAprilThirteen();
		}

		public void OpenSaveScreen() {
			saveScreen.OpenScreen(this.dataManager);
			this.gameStateManager.SetModalState();
		}

		public void OpenLoadScreen() {
			loadScreen.OpenScreen(this.dataManager);
			this.gameStateManager.SetModalState();
		}

		public void BackFromSave() {
			saveScreen.CloseScreen();
			this.gameStateManager.SetMoveState();
		}

		public void BackFromLoad() {
			loadScreen.CloseScreen();
			this.gameStateManager.SetMoveState();
		}

		public void OpenSaveConfirm(int saveLoadNum) {
			this.saveLoadNum = saveLoadNum;
			saveConfirmModal.SetActive(true);
		}

		public void OpenLoadConfirm(int saveLoadNum) {
			this.saveLoadNum = saveLoadNum;
			loadConfirmModal.SetActive(true);
		}
		
		public void CloseSaveConfirm() {
			saveConfirmModal.SetActive(false);
		}

		public void CloseLoadConfirm() {
			loadConfirmModal.SetActive(false);
		}



		public void Load() {
			ScheneSwitchData scheneSwithData = new ScheneSwitchData(ScheneSwitchData.LoadType.LoadGame, saveLoadNum);
			this.dataManager.SetScheneSwithData(scheneSwithData);
			Application.LoadLevel("main");
		}

		void SetUserData(int saveUserId) {
			UserModel userModel = this.dataManager.userModel.buildModel();
			int cellNum = this.fieldManager.fieldObjectManager.hero.currentCell.cellNumber;
			int fieldId = this.fieldManager.fieldObjectManager.currentField.FieldId;
			int characterId = this.fieldManager.fieldObjectManager.hero.characterModel.id;
			userModel.SetUserInfo(cellNum, fieldId, characterId, saveUserId);
			this.dataManager.userModel.SetSaveData(saveUserId);
		}

		void SetUIButton() {
			if (this.dataManager.userModel.buildModel().enableSkipButton) {
				this.skipDate.SetActive(true);
				if (this.dataManager.userModel.buildModel().IsTwelve()) {
					this.skipDate.GetComponent<Image>().sprite = twelve;
				} else {
					this.skipDate.GetComponent<Image>().sprite = thirteen;
				}
			} else {
				this.skipDate.SetActive(false);
			}

			if (this.dataManager.userModel.buildModel().IsTwelve()) {
				this.date.GetComponent<Image>().sprite = twelveDate;
			} else {
				this.date.GetComponent<Image>().sprite = thirteenDate;
            }
        }
    }
}
