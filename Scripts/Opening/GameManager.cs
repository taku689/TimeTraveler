using UnityEngine;
using System.Collections;
using TimeTraveler;

namespace TimeTraveler.Opening {
	public class GameManager : MonoBehaviour {
		
		[SerializeField]
		InteractionEvent interactoinEvent;
		[SerializeField]
		DataManager dataManager;
		[SerializeField]
		SaveLoad loadModal;
		[SerializeField]
		GameObject confirmModal;

		int NEW_GAME_USER_ID = 1001;

		int loadNum;


		public InteractionManager interactionManager { get; private set;}
		
		void Start() {
			Application.targetFrameRate = 60;
			this.interactionManager = new InteractionManager(this, this.interactoinEvent);
			this.dataManager.loadUserDataForTitle();
		}

		public void OnTouchNewGame() {
			ScheneSwitchData scheneSwithData = new ScheneSwitchData(ScheneSwitchData.LoadType.NewGame, NEW_GAME_USER_ID);
			this.dataManager.SetScheneSwithData(scheneSwithData);
			Application.LoadLevel("main");
		}

		public void OnTouchLoadGame() {
			loadModal.OpenScreen(this.dataManager);
		}

		public void Load() {
			ScheneSwitchData scheneSwithData = new ScheneSwitchData(ScheneSwitchData.LoadType.LoadGame, loadNum);
			this.dataManager.SetScheneSwithData(scheneSwithData);
			Application.LoadLevel("main");
		}

		public void OpenLoadConfirm(int loadNum) {
			this.loadNum = loadNum;
			confirmModal.SetActive(true);
		}

		public void CloseLoadConfirm() {
			confirmModal.SetActive(false);
		}

		public void BackFromLoad() {
			loadModal.CloseScreen();
		}
	}
}
