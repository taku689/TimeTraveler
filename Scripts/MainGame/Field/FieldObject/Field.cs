using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame { 
	public class Field : MonoBehaviour {

		float NEIGHBOR_FIELD_DISTANCE = 64f;
		float MOVE_FIELD_VELOCITY = 1.5f;
		List<int> upMoveCellNums    = new List<int>(){1,2,3,4,5,6,7,8};
		List<int> rightMoveCellNums = new List<int>(){19,29,39,49,59,69,79,89};
		List<int> downMoveCellNums  = new List<int>(){91,92,93,94,95,96,97,98};
		List<int> leftMoveCellNums  = new List<int>(){10,20,30,40,50,60,70,80};

		[SerializeField]
		GameObject fieldObjects;
		[SerializeField]
		GameObject blackOutObjects;
		[SerializeField]
		GameObject eventObjects;

		[SerializeField]
		int upFieldId;
		[SerializeField]
		int rightFieldId;
		[SerializeField]
		int downFieldId;
		[SerializeField]
		int leftFieldId;
		[SerializeField]
		List<int> _selfProhibitedArea = null;
		[SerializeField]
		List<int> _initEventIds;

		public List<int> initEventIds {get {return this._initEventIds;}}

		public FieldInfo fieldInfo {get; private set;}
		FieldObject[] fieldObjectComponents;
		BlackOutObject[] blackOutObjectComponents;
		Character[] characterObjectComponents;

		[SerializeField]
		int _fieldId;
		public int FieldId { get { return this._fieldId;}}
		public List<int> selfProhibitedArea {get {return this._selfProhibitedArea;}}
		public void setSelfProhibitedArea (List<int> value) { this._selfProhibitedArea = value; }
		GameManager gameManager;

		bool isMoving = false;
		Vector3 towardPos;

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			this.fieldObjectComponents = fieldObjects.GetComponentsInChildren<FieldObject>();
			if (this.blackOutObjects != null) {
				this.blackOutObjectComponents = this.blackOutObjects.GetComponentsInChildren<BlackOutObject>();
			}
			this.InitializeFieldObjects();


			this.UpdateAsObservable()
				.Where (_ => this.isMoving)
				.Subscribe(_ => this.Move());
		}

		public void InitializeFieldObjects() {
			List<int> prohibitedArea = new List<int>();

			List<FieldObject> visibleFieldObject = new List<FieldObject> ();
			foreach(FieldObject fieldObject in this.fieldObjectComponents) {
				if (fieldObject == null) return;
				fieldObject.Initialize(this.gameManager);
				if (fieldObject.isVisible) visibleFieldObject.Add(fieldObject);
			}

			foreach(FieldObject fieldObject in visibleFieldObject) {
				prohibitedArea = prohibitedArea.Union(fieldObject.getProhibitedArea()).ToList();

			}

			prohibitedArea = prohibitedArea.Union(this._selfProhibitedArea).ToList();

			Dictionary<int, FieldMoveInfo> cellNumTofieldMoveInfo = new Dictionary<int, FieldMoveInfo>();
			cellNumTofieldMoveInfo = this.addHorizonFieldMoveInfo(prohibitedArea, cellNumTofieldMoveInfo);
			cellNumTofieldMoveInfo = this.addBlackOutFieldMoveInfo(prohibitedArea, cellNumTofieldMoveInfo);

			Dictionary<int, List<int>> cellNumToFieldEventIds = this.getCellNumToFieldEventIds(visibleFieldObject);
	
			this.fieldInfo = new FieldInfo(prohibitedArea, cellNumTofieldMoveInfo, cellNumToFieldEventIds);

			this.characterObjectComponents = fieldObjects.GetComponentsInChildren<Character>();
		}

		Dictionary<int, List<int>> getCellNumToFieldEventIds(List<FieldObject> fieldObjects) {
			Dictionary<int, List<int>> cellNumToFieldEventIds = new Dictionary<int, List<int>>();
			// directly eventIds
			if (this.eventObjects != null) {
				EventObject[] eventObjectComponents = this.eventObjects.GetComponentsInChildren<EventObject>();
				foreach(EventObject eventObject in eventObjectComponents) {
					List<int> cellNumbers = eventObject.getArea();
					foreach(int cellNum in cellNumbers) {
						cellNumToFieldEventIds.Add (cellNum, eventObject.eventIds);
					}
				}
			}

			// field Objects eventId
			foreach(FieldObject fieldObject in fieldObjects) {
				if(fieldObject.eventIds.Count == 0) continue;
				List<int> cellNumbers = fieldObject.getEventArea();
				foreach(int cellNum in cellNumbers) {
					cellNumToFieldEventIds.Add (cellNum, fieldObject.eventIds);
				}
			}
			return cellNumToFieldEventIds;
		}

		Dictionary<int, FieldMoveInfo> addHorizonFieldMoveInfo(List<int> prohibitedArea, Dictionary<int, FieldMoveInfo> cellNumToFieldMoveInfo) {
			if (upFieldId != 0) {
				foreach(int cellNum in upMoveCellNums) {
					this.AddHorizonMoveFieldInfo(cellNumToFieldMoveInfo, prohibitedArea, cellNum, cellNum + 90, upFieldId);
				}
			}

			if (rightFieldId != 0) {
				foreach(int cellNum in rightMoveCellNums) {
					this.AddHorizonMoveFieldInfo(cellNumToFieldMoveInfo, prohibitedArea, cellNum, cellNum - 9, rightFieldId);
				}
			}

			if (downFieldId != 0) {
				foreach(int cellNum in downMoveCellNums) {
					this.AddHorizonMoveFieldInfo(cellNumToFieldMoveInfo, prohibitedArea, cellNum, cellNum - 90, downFieldId);
				}
			}

			if (leftFieldId != 0) {
				foreach(int cellNum in leftMoveCellNums) {
					this.AddHorizonMoveFieldInfo(cellNumToFieldMoveInfo, prohibitedArea, cellNum, cellNum + 9, leftFieldId);
				}
			}
			return cellNumToFieldMoveInfo;
		}

		void AddHorizonMoveFieldInfo(Dictionary<int, FieldMoveInfo> cellNumToFieldMoveInfo, List<int> prohibitedArea, int beforeCellNum, int afterCellNum, int moveFieldId) {
			if (!prohibitedArea.Contains(beforeCellNum)) {
				cellNumToFieldMoveInfo.Add (beforeCellNum, new FieldMoveInfo(FieldMoveInfo.MoveType.Horizon, beforeCellNum, afterCellNum, moveFieldId, true));
			}
		}

		Dictionary<int, FieldMoveInfo> addBlackOutFieldMoveInfo(List<int> prohibitedArea, Dictionary<int, FieldMoveInfo> cellNumToFieldMoveInfo) {
			if (this.blackOutObjectComponents == null) return cellNumToFieldMoveInfo;
			foreach(BlackOutObject blackOutObject in this.blackOutObjectComponents) {
				blackOutObject.Initialize(this.gameManager);
				foreach (int cellNumber in blackOutObject.getMoveArea()) {
					this.AddBlackOutMoveFieldInfo(cellNumToFieldMoveInfo, prohibitedArea, cellNumber, blackOutObject.moveFieldCellNumber, blackOutObject.moveFieldId, blackOutObject.isVisible);
				}
			}
			return cellNumToFieldMoveInfo;
		}

		void AddBlackOutMoveFieldInfo(Dictionary<int, FieldMoveInfo> cellNumToFieldMoveInfo, List<int> prohibitedArea, int beforeCellNum, int afterCellNum, int moveFieldId, bool canMove) {
			if (!prohibitedArea.Contains(beforeCellNum)) {
				cellNumToFieldMoveInfo.Add (beforeCellNum, new FieldMoveInfo(FieldMoveInfo.MoveType.BlackOut, beforeCellNum, afterCellNum, moveFieldId, canMove));
			}
		}

	 	void Move() {
			if (Vector3.Distance(towardPos, transform.localPosition) > 0.02f) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, towardPos, MOVE_FIELD_VELOCITY);
			}
			else {
					this.EndMoving();
			}
		}

		void StartMoving() {
			this.isMoving = true;
		}

		void EndMoving() {
			this.isMoving = false;
		}

		public void SetPosition(FieldInfo.FieldPosition fieldPosition) {
			this.SetTowardPos(fieldPosition);
			transform.localPosition = towardPos;
		}

		public IObservable<bool> MoveToTarget(FieldInfo.FieldPosition towardFieldPosition) {
			this.SetTowardPos(towardFieldPosition);
			this.StartMoving();
			return this.UpdateAsObservable()
				.Select(_ => this.isMoving)
				.TakeWhile(_ => this.isMoving );
		}

		void SetTowardPos(FieldInfo.FieldPosition fieldPosition) {
			if (fieldPosition == FieldInfo.FieldPosition.Center) {
				towardPos = new Vector3(0.0f, 0.0f, 0.0f);
			}
			
			if (fieldPosition == FieldInfo.FieldPosition.Up) {
				towardPos = new Vector3(0.0f, NEIGHBOR_FIELD_DISTANCE, 0.0f);
			}
			
			if (fieldPosition == FieldInfo.FieldPosition.Down) {
				towardPos = new Vector3(0.0f, -NEIGHBOR_FIELD_DISTANCE, 0.0f);
			}
			
			if (fieldPosition == FieldInfo.FieldPosition.Right) {
				towardPos = new Vector3(NEIGHBOR_FIELD_DISTANCE, 0.0f, 0.0f);
			}
			
			if (fieldPosition == FieldInfo.FieldPosition.Left) {
				towardPos = new Vector3(-NEIGHBOR_FIELD_DISTANCE, 0.0f, 0.0f);
			}
			if (fieldPosition == FieldInfo.FieldPosition.BlackOut) {
				towardPos = new Vector3(NEIGHBOR_FIELD_DISTANCE, NEIGHBOR_FIELD_DISTANCE, 0.0f);
			}
		}

		public bool hasInitEvent() { return this.initEventIds.Count > 0; }

		public void Destroy() {
			Destroy(this.gameObject);
		}

		public Character[] GetCharacterComponents() {
			return this.characterObjectComponents;
        }
	}
}
