using UnityEngine;
using System.Collections;

namespace TimeTraveler.MainGame{
	public class FieldMoveInfo {
		public enum MoveType {
			Horizon,
			BlackOut
		};

		public FieldMoveInfo(MoveType moveType, int beforeMoveCell, int afterMoveCell, int moveFieldId, bool canMove) {
			this.moveType = moveType;
			this.BeforeMoveCell = beforeMoveCell;
			this.AfterMoveCell = afterMoveCell;
			this.MoveFieldId = moveFieldId;
			this.canMove = canMove;
		}

		public MoveType moveType { get; private set;}
		public int BeforeMoveCell {get; private set;}
		public int AfterMoveCell {get; private set;}
		public int MoveFieldId {get; private set;}
		public bool canMove {get; private set;}

	}
}
