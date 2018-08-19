using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{
	public class FieldInfo{

		public enum FieldPosition {
			Up,
			Right,
			Down,
			Left,
			NotInclude,
			Center,
			BlackOut
		};

		Dictionary<int, FieldPosition> _neighborFieldIdToPosition = new Dictionary<int, FieldPosition>();
		public List<int> ProhibitedArea {get; private set;}
		public Dictionary<int, List<int>> CellNumToFieldEventIds {get; private set;}
		public Dictionary<int, FieldMoveInfo> CellNumToFieldMoveInfo {get; private set;}
		public Dictionary<int, FieldPosition> NeighborFieldIdToPosition { get{ return this._neighborFieldIdToPosition; }}

		public FieldInfo(List<int> prohibitedArea, Dictionary<int, FieldMoveInfo> cellNumToFieldMoveInfo, Dictionary<int, List<int>> cellNumToFieldEventIds) {
			this.ProhibitedArea = prohibitedArea;
			this.CellNumToFieldMoveInfo  = cellNumToFieldMoveInfo;
			this.CellNumToFieldEventIds  = cellNumToFieldEventIds;
			List<int> neighborFieldNumbers = new List<int>();

			foreach (KeyValuePair<int, FieldMoveInfo> pair in this.CellNumToFieldMoveInfo) {
				if (!neighborFieldNumbers.Contains(pair.Value.MoveFieldId)) {
					int fieldId = pair.Value.MoveFieldId;
					this._neighborFieldIdToPosition.Add(fieldId, this.calcFieldPosition(pair.Value.BeforeMoveCell, pair.Value.moveType));
					neighborFieldNumbers.Add (fieldId);
				}
			}
		}

		FieldPosition calcFieldPosition(int cellNumber, FieldMoveInfo.MoveType moveType) {
			if (moveType == FieldMoveInfo.MoveType.BlackOut) {
				return FieldPosition.BlackOut;
			}
			if ( 0 < cellNumber  && cellNumber < 10) {
				return FieldPosition.Up;
			}
			if (90 < cellNumber && cellNumber < 99) {
				return FieldPosition.Down;
			}
			if (cellNumber % 10 == 9 && 10 < cellNumber && cellNumber < 90) {
				return FieldPosition.Right;
			}
			if (cellNumber % 10 == 0 && 0 < cellNumber && cellNumber < 90) {
				return FieldPosition.Left;
			}
			return FieldPosition.NotInclude;
		}

	}
}
