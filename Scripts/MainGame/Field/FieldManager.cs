using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace TimeTraveler.MainGame {
	public class FieldManager : MonoBehaviour {

		GameManager gameManager;
		[SerializeField]
		CellManager _cellManager;
		[SerializeField]
		FieldObjectManager _fieldObjectManager;

		public CellManager cellManager { get {return this._cellManager;} }
		public FieldObjectManager fieldObjectManager { get {return this._fieldObjectManager;} }
		public SoundManager soundManager { get; private set; }
		public GameMap gameMap {get; private set; }

		public void Initialize(GameManager gameManager) {
			this.gameManager = gameManager;
			this.gameMap = new GameMap();
			this.fieldObjectManager.Initialize(gameManager, this);
			this.cellManager.Initialize(gameManager, this);
			this.soundManager = this.soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		}

		public void OnTouchCell(Cell touchCell, List<Cell> routeCells) {
			// TODO fix later
			soundManager.Play(101);

			this.gameManager.gameStateManager.SetMovingState();
			this.fieldObjectManager.hero.currentCell.SetAllowable();
			this.fieldObjectManager.MoveCharacter(touchCell, routeCells)
				.Subscribe(_ => { ;}, () => { this.OnMovedHero(touchCell);});
		}

		public IObservable<bool> MoveCharacterByEvent(MoveCharacterEventModel moveCharacterEventModel) {
			return this.fieldObjectManager.MoveCharacterByEvent(moveCharacterEventModel);
		}

		public void RemoveCharacterByEvent(MoveCharacterEventModel moveCharacterEventModel) {
			this.fieldObjectManager.RemoveCharacter(moveCharacterEventModel.characterId);
		}

		void SetCurrentFieldInfo() {
			this.cellManager.SetCurrentFieldInfo(this._fieldObjectManager.currentFieldInfo);
			this.fieldObjectManager.hero.currentCell.SetProhibited();
			this.gameMap.SetCurrentProhibitedArea(this._fieldObjectManager.currentFieldInfo.ProhibitedArea);
		}

		public void InitializeFieldObjects() {
			this.fieldObjectManager.InitializeFieldObjects();
			this.SetCurrentFieldInfo();
		}



		void OnMovedHero(Cell touchCell) {
			this.fieldObjectManager.hero.currentCell.SetProhibited();
			this.fieldObjectManager.OnMovedHero();
			if (touchCell.IsFieldMoveCell) {
				this.MoveField(touchCell);
			} else if(touchCell.IsEvent) {
				touchCell.ActionBeforeEvent();
				this.gameManager.LaunchEventByField(touchCell.eventIds);
			} else {
				this.gameManager.gameStateManager.SetMoveState();
			}
		}

		void MoveField(Cell touchCell) {
			if (touchCell.fieldMoveInfo.moveType == FieldMoveInfo.MoveType.Horizon) {
				this.fieldObjectManager.MoveFieldHorizontally(touchCell)
				.Subscribe( _ => { ;}, () => { this.OnMovedField(); });
			} else if (touchCell.fieldMoveInfo.moveType == FieldMoveInfo.MoveType.BlackOut) {
				this.fieldObjectManager.ToBlackOut(true)
				.Subscribe( _ => { ;}, () => {
					this.fieldObjectManager.MoveFieldBlackOut(touchCell);
					this.fieldObjectManager.ToBlackOut(false)
					.Subscribe( _ => { ;}, () => { 
						this.OnMovedField();
					});
				});
			}
		}

		void OnMovedField() {
			this._fieldObjectManager.ChangeCurrentField();
			this.SetCurrentFieldInfo();
			Field currentField = this.fieldObjectManager.currentField;
			if (currentField.hasInitEvent()) {
				this.gameManager.LaunchEventByField(currentField.initEventIds);
			} else {
				this.gameManager.gameStateManager.SetMoveState();
			}
		}

		public void LoadField(LoadFieldEventModel loadFieldEventModel) {
			this.fieldObjectManager.LoadFieldObject(loadFieldEventModel);
			this.SetCurrentFieldInfo();
		}
	}
}
