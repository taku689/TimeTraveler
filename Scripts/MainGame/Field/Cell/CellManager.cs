using UnityEngine;
using System.Collections;
using TimeTraveler;
using System.Collections.Generic;

namespace TimeTraveler.MainGame {
	public class CellManager : MonoBehaviour {
		public GameObject cellObject;
		float minCellX = 0.0f;
		float maxCellX = 640.0f;
		float minCellY = 260.0f;
		float maxCellY = 900.0f;
		float cellSize = 64.0f;
		float iPhoneXMultiple = 3.2f;
		float iPhoneYMultiple = 3.25f;
		float cellMultiple = 3.2f;
		Cell[,] cellArray = new Cell [10, 10];
		[SerializeField]
		Target target;

		public GameManager gameManager { get; private set; }
		public FieldManager fieldManager { get; private set; }

		void Awake () {
			#if UNITY_EDITOR
				this.setForEditor();
			#endif


			this.setCell();
		}

		public void Initialize (GameManager gameManager, FieldManager fieldManager) {
			this.gameManager = gameManager;
			this.fieldManager = fieldManager;
			this.GenerateCells();
		}

		void setForEditor() {
			this.minCellX /= iPhoneXMultiple;
			this.maxCellX /= iPhoneXMultiple;
			this.minCellY /= iPhoneYMultiple;
			this.maxCellY /= iPhoneYMultiple;
			this.cellSize /= cellMultiple;
		}

		void setCell() {
			int width = Screen.width;
			int height = Screen.height;
			this.maxCellX = width;
			this.minCellY = (height - width) / 2;
			this.maxCellY = this.minCellY + this.maxCellX;
			this.cellSize = this.maxCellX / 10;
		}

		void GenerateCells () {
			for (int y = 0; y < 10; y++) {
				for (int x = 0; x < 10; x++) {
					this.GenerateCell(x, y);
				}
			}
		}

		void GenerateCell (int x, int y) {
			GameObject cell = Instantiate(cellObject);	
			cell.transform.SetParent(this.transform);
			Cell cellScript = cell.transform.GetComponent<Cell>();
			cellScript.Initialize(x, y, this);
			cellArray[x,y] = cellScript;
		}

		public void OnTouchCell (InteractionEvent interactionEvent) {
			Cell touchCell = this.calcTouchCell(interactionEvent.InitialPos);
			if (touchCell == null) return;

			if (touchCell.IsProhibited && !touchCell.IsEvent)  {
				this.target.turnTargetWithLimit(touchCell.PosX, touchCell.PosY);
				return;
			}

			Character hero = this.fieldManager.fieldObjectManager.hero;
			List<int> route = this.fieldManager.gameMap.Route(hero.currentCell.cellNumber, touchCell.cellNumber, touchCell.IsProhibited);
			if (route ==  null) {
				if (touchCell.IsEvent) {
					route = this.GetAdjustReachableRoute(touchCell, hero);
					if (route == null) return;
				} else {
					this.target.turnTargetWithLimit(touchCell.PosX, touchCell.PosY);
					return;
				}
			}

			List<Cell> routeCells = this.GetRouteCells(route);

			this.fieldManager.OnTouchCell(touchCell, routeCells);
			this.target.turnTarget(touchCell.PosX, touchCell.PosY);
		}

		List<int> GetAdjustReachableRoute(Cell touchCell, Character hero) {
			List<int> adjustValues = new List<int>(){1, -1, 10, -10};
			List<int> addValues = new List<int>(){1, -1, 10, -10};
			int maxCnt = 5;
			int cnt = 0;
			while(true) {
				foreach(int adjustValue in adjustValues) {
					int adjustCellNum = touchCell.cellNumber + adjustValue;
					Cell adjustCell = this.GetCellByCellNum(adjustCellNum);
					List<int> route = this.fieldManager.gameMap.Route(hero.currentCell.cellNumber, adjustCell.cellNumber, adjustCell.IsProhibited);
					if (route != null) {
						return route;
					}
				}
				for (int i = 0; i < 4; i++) {
					adjustValues[i] += addValues[i];
				}
				if (cnt > maxCnt) {
					return null;
				}
				cnt++;
			}
		}

		public List<Cell> GetRouteCells(List<int> route) {
			List<Cell> routeCells = new List<Cell>();

			if (route == null) {
				return routeCells;
			}

			foreach(int cellNumber in route) {
				Vector2 xyPoint = this.fieldManager.gameMap.XYpoint(cellNumber);
				routeCells.Add(this.cellArray[(int)xyPoint.x, (int)xyPoint.y]);
			}
			return routeCells;
		}

		Cell calcTouchCell(Vector3 pos) {
			if (pos.x < minCellX || pos.x > maxCellX || pos.y < minCellY || pos.y > maxCellY) return null;
			int cellX = (int) (pos.x / cellSize);
			int cellY = (int) (Mathf.Abs(pos.y - maxCellY) / cellSize);
			return this.cellArray[cellX,cellY];
		}

		public void SetCurrentFieldInfo(FieldInfo fieldInfo) {
			this.ResetCellInfo();
			this.SetMoveFieldCell(fieldInfo);
			this.SetFieldEvent(fieldInfo);
		}

		void ResetCellInfo() {
			for (int y = 0; y < 10; y++) {
				for (int x = 0; x < 10; x++) {
					this.cellArray[x,y].ResetFieldMoveInfo();
				}
			}
		}

		void SetMoveFieldCell(FieldInfo fieldInfo) {
			foreach(int cellNum in fieldInfo.ProhibitedArea) {
				Cell cell = this.GetCellByCellNum(cellNum);
				cell.SetProhibited();
			}
			foreach(KeyValuePair<int, FieldMoveInfo> cellNumToFieldMoveInfo in fieldInfo.CellNumToFieldMoveInfo) {
				Vector2 xyPoint = this.fieldManager.gameMap.XYpoint(cellNumToFieldMoveInfo.Key);
				this.cellArray[(int)xyPoint.x, (int)xyPoint.y].SetFieldMoveInfo(cellNumToFieldMoveInfo.Value);
			}
		}

		void SetFieldEvent(FieldInfo fieldInfo) {
			foreach(int cellNum in fieldInfo.CellNumToFieldEventIds.Keys) {
				Cell cell = this.GetCellByCellNum(cellNum);
				cell.SetEvent(fieldInfo.CellNumToFieldEventIds[cellNum]);
			}
		}

		public Cell GetCellByCellNum(int cellNum) {
			Vector2 xyPoint = this.fieldManager.gameMap.XYpoint(cellNum);
			return this.cellArray[(int)xyPoint.x, (int)xyPoint.y];
		}

		public void MoveField(FieldMoveInfo fieldMoveInfo) {
		}
	}
}
