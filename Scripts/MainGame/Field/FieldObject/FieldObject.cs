using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TimeTraveler.Model.User;

namespace TimeTraveler.MainGame{ 
	public class FieldObject : MonoBehaviour {

		[SerializeField]
		int _width;
		[SerializeField]
		int _height;
		[SerializeField]
		int cellNumber;
		[SerializeField]
		List<int> _eventIds;
		[SerializeField]
		List<int> _allowArea = null;
		[SerializeField]
		Character character;
		[SerializeField]
		List<int> visibleFlagEventIds;
		[SerializeField]
		List<int> invisibleFlagEventIds;
		[SerializeField]
		EventManagerModel.DateFlag dateFlag;

		const float Z_POS = -2.0f;

		public int width  {get{ return this._width;}}
		public int height {get{ return this._height;}}
		public List<int> eventIds {get{ return this._eventIds;}}
		public List<int> allowArea {get {return this._allowArea;}}
		bool _isVisible;
		public bool isVisible {
			get { return this._isVisible;}
			private set {
				this.gameObject.SetActive(value);
				this._isVisible = value;
			}
		}

		GameManager gameManager;

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			if (this.character != null) {
				Cell initCell = gameManager.fieldManager.cellManager.GetCellByCellNum(this.cellNumber);
				this.character.Initialize(gameManager, initCell);
			}
			this.calcVisible();
			this.calcInVisibleByDate();
			this.SetPosition();
		}

		void calcVisible() {
			// CheckVisible
			bool isVisible = true;
			if (this.visibleFlagEventIds.Count == 0) {
			} else {
				isVisible = this.gameManager.dataManager.userFlagModel.IsFlagOn(this.visibleFlagEventIds);
			}

			// CheckInvisible
			bool isInvisible = false;
			if (this.invisibleFlagEventIds.Count == 0) {
				this.isVisible = isVisible;
				return;
			} else {
				if (this.invisibleFlagEventIds.Count > 0)
				{
					isInvisible = this.gameManager.dataManager.userFlagModel.IsFlagOn(this.invisibleFlagEventIds);
				}
			}

			if (isInvisible == false && isVisible == true) {
				this.isVisible = true;
			} else {
				this.isVisible = false;
			}
		}

		void calcInVisibleByDate() {
			if (this.dateFlag == 0 || this.dateFlag == EventManagerModel.DateFlag.AllDate) {
				return;
			}

			if ((int)this.dateFlag != this.gameManager.dataManager.userModel.buildModel().date) {
				this.isVisible = false;
			}
		}

		public void setCellNumber(int value) { this.cellNumber = value; }
		public void setAllowArea (List<int> value) { this._allowArea = value; }

		public List<int> getProhibitedArea() {
			List<int> prohibitedArea = new List<int>();
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					int num = this.cellNumber + x + y * 10;
					if (!this.allowArea.Contains(num)) prohibitedArea.Add(this.cellNumber + x + y * 10);
				}
			}
			return prohibitedArea;
		}

		public List<int> getEventArea() {
			List<int> area = new List<int>();
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					area.Add(this.cellNumber + x + y * 10);
				}
			}
			return area;
		}

		void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
		}

		void SetPosition() {
			Character character = gameObject.GetComponent<Character>();
			if (character == null) {
				Vector3 currentPos = this.transform.localPosition;
				this.transform.localPosition = new Vector3(currentPos.x, currentPos.y, Z_POS);
			}
		}



	}
}
