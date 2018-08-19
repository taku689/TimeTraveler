using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TimeTraveler.Model.User;

namespace TimeTraveler.MainGame{ 
	public class BlackOutObject : MonoBehaviour {
		
		[SerializeField]
		int _width;
		[SerializeField]
		int _height;
		[SerializeField]
		int cellNumber;
		[SerializeField]
		int _moveFieldCellNumber;
		[SerializeField]
		int _moveFieldId;
		[SerializeField]
		List<int> visibleFlagEventIds;
		[SerializeField]
		List<int> invisibleFlagEventIds;
		[SerializeField]
		EventManagerModel.DateFlag dateFlag;
		bool _isVisible;


		GameManager gameManager;
		public bool isVisible { get; private set;}

		public int width  {get{ return this._width;}}
		public int height {get{ return this._height;}}
		public int moveFieldCellNumber {get {return this._moveFieldCellNumber;}}
		public int moveFieldId {get {return this._moveFieldId;}}
		
		public void setCellNumber(int value) { this.cellNumber = value; }
		public void setWidth(int value)      { this._width     = value; }
		public void setHeight(int value)     { this._height    = value; }

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			this.calcVisible();
			this.calcInvisible();
			this.calcInVisibleByDate();
		}
	
		public List<int> getMoveArea() {
			List<int> moveArea = new List<int>();
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					moveArea.Add(this.cellNumber + x + y * 10);
				}
			}
			return moveArea;
		}

		void calcVisible() {
			if (this.visibleFlagEventIds.Count == 0) {
				this.isVisible = true;
				return;
			}

			/*
			List<UserEventModel> userEventModels = this.gameManager.dataManager.userEventModel.buildModelsByIds(this.visibleFlagEventIds);
			bool isVisible = userEventModels.All (_ => _.IsSeen());
			*/
			bool isVisible = this.gameManager.dataManager.userFlagModel.IsFlagOn(this.visibleFlagEventIds);
			this.isVisible = isVisible;
		}
	
		void calcInvisible() {
			if (this.invisibleFlagEventIds.Count == 0) {
				return;
			}
	
			List<UserEventModel> userEventModels = this.gameManager.dataManager.userEventModel.buildModelsByIds(this.invisibleFlagEventIds);
			bool isInvisible = false;
			if (userEventModels.Count > 0) {
				//isInvisible = userEventModels.All (_ => _.IsSeen());
				isInvisible = this.gameManager.dataManager.userFlagModel.IsFlagOn(this.invisibleFlagEventIds);
			}
			this.isVisible = !isInvisible;
		}

		void calcInVisibleByDate() {
			if (this.dateFlag == 0 || this.dateFlag == EventManagerModel.DateFlag.AllDate) {
				return;
			}

			if ((int)this.dateFlag != this.gameManager.dataManager.userModel.buildModel().date) {
				this.isVisible = false;
			}
		}
		
	}
}
