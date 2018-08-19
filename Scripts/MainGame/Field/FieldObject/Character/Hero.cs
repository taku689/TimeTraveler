using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame {
	public class Hero : Character {

		bool isMovingField = false;

		public override void Initialize (GameManager gameManager, Cell initCell, int characterId) {
			base.Initialize (gameManager, initCell, characterId);

			this.UpdateAsObservable()
				.Where (_ => this.isMovingField)
				.Subscribe(_ => this.MoveField());
		}
		
		void StartFieldMoving(FieldInfo.FieldPosition towardPosition) {
			this.isMovingField = true;
			this.characterAnimation.currentAnimateState = this.fieldPositionToCharacterAnimation[towardPosition];
		}
		
		void EndFieldMoving() {
			this.isMovingField = false;
		}
		
		public void MoveField() {
			if (Vector3.Distance(towardPos, transform.localPosition) > 0.02f) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, towardPos, MOVE_FIELD_VELOCITY);
			}
			else {
				this.EndFieldMoving();
			}
		}
		
		public IObservable<bool> MoveToFieldHorizontally(Cell cell, FieldInfo.FieldPosition towardPosition) {
			this.MoveFieldBase(cell);

			this.StartFieldMoving(towardPosition);
			return this.UpdateAsObservable()
				.Select(_ => this.isMovingField)
				.TakeWhile(_ => this.isMovingField );
		}
		
		public void MoveToFieldBlackOut(Cell cell, FieldInfo.FieldPosition towardPosition) {
			this.MoveFieldBase(cell);
			transform.localPosition = this.towardPos;
		}

		void MoveFieldBase(Cell cell) {
			this.towardPos = cell.Pos();
			this.towardPos.z = -1.0f;
			this.SetCurrentCell(cell);
		}

	}
}