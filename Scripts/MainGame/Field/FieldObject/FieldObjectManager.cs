using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{ 
	public class FieldObjectManager : MonoBehaviour {

		string FIELD_OBJECT_PATH = "Prefab/Field/";
		string HERO_PATH    = "Prefab/Character/Hero";
		string CHARACTER_PATH    = "Prefab/Character/Character";
		int HERO_CHARACTER_ID = 1;
		[SerializeField]
		Target target;
		[SerializeField]
		BlackOut blackOut;

		FieldManager fieldManager;
		GameManager gameManager;
		public Hero hero {get; private set;}
		public Field currentField {get; private set;}
		public FieldInfo currentFieldInfo {get; private set;}
		Dictionary<int, Character> characterIdToCharacter = new Dictionary<int, Character>(){};
		Dictionary<int, Field> fieldIdToField = new Dictionary<int, Field>(){};

		Dictionary<FieldInfo.FieldPosition, FieldInfo.FieldPosition> upsideFieldPosition = new Dictionary<FieldInfo.FieldPosition, FieldInfo.FieldPosition>() {
			{FieldInfo.FieldPosition.Up, FieldInfo.FieldPosition.Down},
			{FieldInfo.FieldPosition.Down, FieldInfo.FieldPosition.Up},
			{FieldInfo.FieldPosition.Right, FieldInfo.FieldPosition.Left},
			{FieldInfo.FieldPosition.Left, FieldInfo.FieldPosition.Right}
		};

		public void Initialize(GameManager gameManager, FieldManager fieldManager) {
			this.gameManager = gameManager;
			this.fieldManager = fieldManager;
			this.blackOut.Initialize();
		}

		public void LoadFieldObject(LoadFieldEventModel loadFieldEventModel) {
			Cell initCell = this.fieldManager.cellManager.GetCellByCellNum(loadFieldEventModel.initCellNum);
			// 既に指定フィールドにいたら主人公の位置を変えるだけ
			if ((this.currentField != null) && (this.currentField.FieldId == loadFieldEventModel.fieldId)) {
				this.hero.SetFieldPos(initCell);
				return;
			}

			Field initialField = this.LoadField(loadFieldEventModel.fieldId, FieldInfo.FieldPosition.Center);
			this.currentField = initialField;
			this.ChangeCurrentField();

			// 主人公が既にいるなら新しくロードしない
			if (this.hero == null) {
				this.hero = this.LoadHero();
				this.characterIdToCharacter.Add(this.hero.GetCharacterId(), this.hero);
				this.hero.Initialize(this.gameManager, initCell, loadFieldEventModel.characterId);
			} else {
				this.hero.SetFieldPos(initCell);
			}
		}

		public void ChangeCurrentField() {
			this.currentFieldInfo = this.currentField.fieldInfo;
			this.RemoveExtraFields();
			this.LoadNeighborField();
			this.ChangeLoadedCharacter();
		}

		public void LoadNeighborField() {
			foreach(KeyValuePair<int, FieldInfo.FieldPosition> fieldIdToFieldPosition in this.currentFieldInfo.NeighborFieldIdToPosition) {
				int fieldId = fieldIdToFieldPosition.Key;
				FieldInfo.FieldPosition fieldPosition = fieldIdToFieldPosition.Value;
				if (this.IsAlreadyLoaded(fieldId)) {
					if (fieldId != this.currentField.FieldId) {
						this.fieldIdToField[fieldId].SetPosition(fieldPosition);
					}
				} else {
					this.LoadField(fieldId, fieldPosition);
				}
			}
		}

		bool IsAlreadyLoaded(int fieldId) {
			return this.fieldIdToField.ContainsKey(fieldId);
		}

		Hero LoadHero() {
			GameObject hero = Instantiate(Resources.Load (HERO_PATH)) as GameObject;
			hero.transform.SetParent(this.transform, false);
			
			Hero heroScript = hero.GetComponent<Hero>();
			return heroScript;
		}

		Character LoadCharacter(int characterId) {
			GameObject character = Instantiate(Resources.Load (CHARACTER_PATH)) as GameObject;
			character.transform.SetParent(this.transform, false);

			Character characterScript = character.GetComponent<Character>();
			this.characterIdToCharacter.Add (characterId, characterScript);
			return characterScript;
		}

		public void RemoveCharacter(int characterId) {
			Character character = this.characterIdToCharacter[characterId];
			this.characterIdToCharacter.Remove(characterId);
			character.Destroy();
		}

		Field LoadField(int fieldId, FieldInfo.FieldPosition fieldPosition) {
			GameObject field = Instantiate(Resources.Load (FIELD_OBJECT_PATH + fieldId.ToString())) as GameObject;
			field.transform.SetParent(this.transform, false);

			Field fieldScript = field.GetComponent<Field>();
			fieldScript.SetPosition(fieldPosition);
			fieldScript.Initialize(this.gameManager);

			this.fieldIdToField.Add(fieldScript.FieldId, fieldScript);
			return fieldScript;
		}

		public IObservable<bool> MoveCharacter(Cell touchCell, List<Cell> routeCells) {
			return this.hero.MoveToTarget(routeCells);
		}

		public IObservable<bool> MoveCharacterByCharacter(Character character, List<int> route, float moveVelocity, bool isForceMove = false) {
			if (route == null) {
				return this.UpdateAsObservable()
					.Select(_ => false)
						.TakeWhile(_ => false);
			}
			
			List<Cell> routeCells = this.fieldManager.cellManager.GetRouteCells(route);
			return character.MoveToTarget(routeCells, moveVelocity, isForceMove);
		}

		public IObservable<bool> MoveCharacterByEvent(MoveCharacterEventModel moveCharacterEventModel) {
			Character character = this.GetCharacterWithLoad(moveCharacterEventModel.characterId, moveCharacterEventModel.initCellNum);
			List<int> route = this.GetRouteByEvent(moveCharacterEventModel, character);

			return MoveCharacterByCharacter(character, route, moveCharacterEventModel.MoveVelocity);
		}

		public IObservable<bool> MoveCharacterNextToHero(int characterId) {
			Character character = this.GetCharacterWithLoad(characterId, this.hero.currentCell.cellNumber);
			List<int> route = this.GetNextToHero();

			return this.MoveCharacterByCharacter(character, route, character.normalMoveVelocityFactor);
        }

		public IObservable<bool> MoveCharacterBackToHero(int characterId) {
			Character character = this.GetCharacterWithLoad(characterId, this.hero.currentCell.cellNumber);
			List<int> route = this.fieldManager.gameMap.Route(character.currentCell.cellNumber, this.hero.currentCell.cellNumber, false);

			return this.MoveCharacterByCharacter(character, route, character.normalMoveVelocityFactor, true);
        }

		List<int> GetNextToHero() {
			return this.fieldManager.gameMap.NextToHeroRoute(this.hero.currentCell.cellNumber);
		}

		public void ChangeCharacterDirection(int characterId, int direction) {
			Character character = this.characterIdToCharacter[characterId];
			character.SetDirection(direction);
		}

		public void ChangeCharacterDirectionToHero(int characterId) {
			Character character = this.characterIdToCharacter[characterId];
			character.MoveTowardHero();
		}

		Character GetCharacterWithLoad(int characterId, int initCellNum) {
			Character character;
			if (characterIdToCharacter.ContainsKey(characterId)) {
				character = this.characterIdToCharacter[characterId];
			} else {
				character = this.LoadCharacter(characterId);
				Cell cell = this.fieldManager.cellManager.GetCellByCellNum(initCellNum);
				character.Initialize(this.gameManager, cell, characterId);
			}
			return character;
		}

		List<int> GetRouteByEvent(MoveCharacterEventModel moveCharacterEventModel, Character character) {
			List<int> route;
			if (moveCharacterEventModel.isMoveToHero) {
				route = this.fieldManager.gameMap.Route(character.currentCell.cellNumber, hero.currentCell.cellNumber, false);
			} else {
				route = this.fieldManager.gameMap.Route(character.currentCell.cellNumber, moveCharacterEventModel.moveTargetCellNum, false);
			}
			return route;
		}

		public void OnMovedHero() {
			this.target.fadeTarget();
		}

		public IObservable<bool> MoveFieldHorizontally(Cell touchCell) {
			FieldMoveInfo fieldMoveInfo = touchCell.fieldMoveInfo;
			FieldInfo.FieldPosition towardPosition = currentFieldInfo.NeighborFieldIdToPosition[fieldMoveInfo.MoveFieldId];
			this.hero.MoveToFieldHorizontally(this.fieldManager.cellManager.GetCellByCellNum(fieldMoveInfo.AfterMoveCell), towardPosition);

			Field moveField = this.fieldIdToField[fieldMoveInfo.MoveFieldId];
			moveField.InitializeFieldObjects();
			currentField.MoveToTarget(this.upsideFieldPosition[currentFieldInfo.NeighborFieldIdToPosition[fieldMoveInfo.MoveFieldId]]);
			this.currentField = moveField;
			return moveField.MoveToTarget(FieldInfo.FieldPosition.Center);
		}

		public IObservable<bool> ToBlackOut(bool toBlack) {
			return this.blackOut.TurnOpacity(toBlack);
		}

		public void MoveFieldBlackOut(Cell touchCell) {
			FieldMoveInfo fieldMoveInfo = touchCell.fieldMoveInfo;
			FieldInfo.FieldPosition towardPosition = currentFieldInfo.NeighborFieldIdToPosition[fieldMoveInfo.MoveFieldId];
			this.hero.MoveToFieldBlackOut(this.fieldManager.cellManager.GetCellByCellNum(fieldMoveInfo.AfterMoveCell), towardPosition);

			Field moveField = this.fieldIdToField[fieldMoveInfo.MoveFieldId];
			moveField.InitializeFieldObjects();
			this.currentField = moveField;
			this.currentField.SetPosition(FieldInfo.FieldPosition.Center);
			this.ChangeCurrentField();
		}

		void RemoveExtraFields() {
			List<Field> fields = new List<Field>(this.fieldIdToField.Values);
			foreach(Field field in fields) {
				if (this.currentField.FieldId != field.FieldId && !this.currentFieldInfo.NeighborFieldIdToPosition.ContainsKey(field.FieldId)) {
					fieldIdToField.Remove(field.FieldId);
					field.Destroy();
				}
			}
		}

		public void InitializeFieldObjects() {
			this.currentField.InitializeFieldObjects();
			this.currentFieldInfo = this.currentField.fieldInfo;
			this.LoadNeighborField();
		}

		void ChangeLoadedCharacter() {
			characterIdToCharacter.Clear();
			Character[] characterScriptArray = this.currentField.GetCharacterComponents();
			foreach (Character characterScript in characterScriptArray) {
				this.characterIdToCharacter.Add(characterScript.GetCharacterId(), characterScript);
			}
			// 無条件で主人公は入れる
			if (this.hero != null) { 
				this.characterIdToCharacter.Add(this.hero.GetCharacterId(), this.hero);
			}
        }

		public void CharacterJump(int characterId) {
			Character character = this.characterIdToCharacter[characterId];
			character.Jump();
		}
	}
}
