using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TimeTraveler.Model.User;

namespace TimeTraveler {
	public class SaveLoad : MonoBehaviour {

		const string NO_DATA = "NO DATA";
		List<int> SAVE_LOAD_USER_IDS = new List<int>(){2001, 2002, 2003, 2004, 2005};
		[SerializeField] List<Text> titles;
		int loadNum;

		public void OpenScreen(DataManager dataManager) {
			List<string> userTitles = new List<string>();
			foreach(var userId in SAVE_LOAD_USER_IDS) {
				if (dataManager.userModel.HasData(userId)) {
					var userModel = new UserModel();
					userModel.loadOrCreate(dataManager, userId);
					userTitles.Add (userModel.buildModel().SaveLoadTitle());
				} else {
					userTitles.Add(NO_DATA);
				}
			}
			SetTitle(userTitles);
			gameObject.SetActive(true);
		}

		public void CloseScreen() {
			gameObject.SetActive(false);
		}

		public void SetTitle(List<string> userTitles) {
			for (int i = 0; i < 5; i++) {
				titles[i].text = userTitles[i];
			}
		}

		public void UpdateTitle(UserModel userModel) {
			int index = SAVE_LOAD_USER_IDS.IndexOf(userModel.id);
			titles[index].text = userModel.SaveLoadTitle();
		}



	}
}
