using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeTraveler;

namespace TimeTraveler.MainGame {
	public class Cell : MonoBehaviour {

		const float size = 6.4f;
		public int X { get; private set;}
		public int Y { get; private set;}
		public float PosX { get; private set;}
		public float PosY { get; private set;}
		public int cellNumber {get; private set;}
		public List<int> eventIds {get; private set;}
		public FieldMoveInfo fieldMoveInfo;
		public bool IsFieldMoveCell { get; private set; }
		public bool IsProhibited { get; private set;}
		public bool IsEvent {get; private set;}
		public Character character {get; private set;}

		public void Initialize(int x, int y, CellManager cellManager) {
			this.X = x;
			this.Y = y;
			this.PosX = x  * size;
			this.PosY = -y * size;
			this.transform.localPosition = new Vector3(this.PosX, this.PosY, 0);
			this.cellNumber = cellManager.fieldManager.gameMap.CellNumber(x, y);
			this.IsFieldMoveCell = false;
		}

		public Vector3 Pos() {
			return this.transform.localPosition;
		}

		public void SetFieldMoveInfo(FieldMoveInfo fieldMoveInfo) {
			this.fieldMoveInfo  = fieldMoveInfo;
			this.IsFieldMoveCell = fieldMoveInfo.canMove;
		}

		public void ResetFieldMoveInfo() {
			this.fieldMoveInfo = null;
			this.IsFieldMoveCell = false;
			this.IsProhibited = false;
			this.IsEvent = false;
			this.eventIds = new List<int>();
		}

		public void SetProhibited() {
			this.IsProhibited = true;
		}

		public void SetAllowable() {
			this.IsProhibited = false;
		}

		public void SetEvent(List<int> eventIds) {
			this.IsEvent = true;
			this.eventIds = eventIds;
		}

		public void setCharacter(Character character) {
			this.character = character;
		}

		public void ActionBeforeEvent() {
			if(this.character != null) {
				this.character.MoveTowardHero();
			}
		}

	}
}
