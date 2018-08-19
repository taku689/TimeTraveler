using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame {
	public class Character : MonoBehaviour {

		protected float MOVE_FIELD_VELOCITY = 1.40f;
		protected float MOVE_VELOCITY = 100f;
		protected float JUMP_HEIGHT = 5.0f;
		protected float JUMP_VELOCITY = 0.5f;
		const float NORMAL_MOVE_VELOCITY_FACTOR = 1.0f;
		public float normalMoveVelocityFactor { get { return NORMAL_MOVE_VELOCITY_FACTOR;} }

		protected CharacterAnimation characterAnimation;
		protected GameManager gameManager;

		protected Cell nextTargetCell;
		protected bool isMoving = false;
		protected bool isJumping = false;
		protected bool isForceMove = false;
		protected List<Cell> routeCells;

		protected Vector3 towardPos;

		public Cell currentCell{get; protected set;}
		public CharacterModel characterModel {get; protected set;}
		float moveVelocityFactor;
		FieldObject fieldObjectComponent;

		const float Z_POS = -1.0f;

		[SerializeField]
		CharacterAnimation.characterAnimateState direction = CharacterAnimation.characterAnimateState.DOWN;
		[SerializeField]
		int characterId;

		Vector3 currentPosition;
		Vector3 jumpHeightPosition;
		bool isUpping = false;

		IDisposable moveDispose = null;
		IDisposable jumpDispose = null;

		protected Dictionary<FieldInfo.FieldPosition, CharacterAnimation.characterAnimateState> fieldPositionToCharacterAnimation = new Dictionary<FieldInfo.FieldPosition, CharacterAnimation.characterAnimateState>{
			{FieldInfo.FieldPosition.Up, CharacterAnimation.characterAnimateState.UP},
			{FieldInfo.FieldPosition.Right, CharacterAnimation.characterAnimateState.RIGHT},
			{FieldInfo.FieldPosition.Down, CharacterAnimation.characterAnimateState.DOWN},
			{FieldInfo.FieldPosition.Left, CharacterAnimation.characterAnimateState.LEFT},
		};

		public virtual void Initialize(GameManager gameManager, Cell initCell, int characterId = 0) {
			this.gameManager = gameManager;
			this.SetFieldPos(initCell);
			characterId = this.characterId == 0 ? characterId : this.characterId;
			this.characterModel = this.gameManager.dataManager.characterModel.getModelById(characterId);
			this.characterAnimation = gameObject.GetComponent<CharacterAnimation>();
			this.characterAnimation.Initialize(characterModel);
			this.characterAnimation.currentAnimateState = this.direction;
			fieldObjectComponent = this.GetComponent<FieldObject>();
			this.currentPosition = transform.localPosition;
			this.SetPos();

			if (moveDispose == null) {
			moveDispose = this.UpdateAsObservable()
				.Where (_ => this.isMoving)
				.Subscribe(_ => this.Move());
			}

			if (jumpDispose == null) {
			this.UpdateAsObservable()
				.Where (_ => this.isJumping)
				.Subscribe(_ => this.Jumping());
			}
        }

		protected void SetCurrentCell(Cell cell) {
			if (this.currentCell != null) {
				this.currentCell.setCharacter(null);
			}
			this.currentCell = cell;
			this.currentCell.setCharacter(this);

			if (fieldObjectComponent != null) {
				fieldObjectComponent.setCellNumber(this.currentCell.cellNumber);
			}
		}

		public void SetDirection(int direction) {
			this.direction = (CharacterAnimation.characterAnimateState) direction;
			this.characterAnimation.currentAnimateState = this.direction;
		}

		public void SetFieldPos(Cell cell) {
			this.SetCurrentCell(cell);
			transform.localPosition = new Vector3(cell.PosX, cell.PosY, -1.0f);
		}

		public void Move() {
			var deltaTime = Time.deltaTime;
			if (this.nextTargetCell.IsProhibited && !this.isForceMove) {
				this.characterAnimation.currentAnimateState = characterDirection(this.currentCell, this.nextTargetCell);
				this.EndMoving();
				return;
			}

			Vector3 towardPos = new Vector3(this.nextTargetCell.PosX, this.nextTargetCell.PosY, -1.0f);

			if (Vector2.Distance(towardPos, transform.localPosition) > 0.72f) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, towardPos, MOVE_VELOCITY * this.moveVelocityFactor * Time.deltaTime);
			} 
			else {
				if (this.routeCells.Count != 0) {
					this.SetCurrentCell(this.nextTargetCell);
					this.nextTargetCell = routeCells[0];
					routeCells.RemoveAt (0);
					this.characterAnimation.currentAnimateState = characterDirection(this.currentCell, this.nextTargetCell);
				} else {
					this.SetCurrentCell(this.nextTargetCell);
					this.EndMoving();
				}
			}
		}

		public virtual CharacterAnimation.characterAnimateState characterDirection(Cell currentCell, Cell towardCell) {
			if (towardCell.X > currentCell.X){
				return CharacterAnimation.characterAnimateState.RIGHT;
			}
			else if (towardCell.X < currentCell.X){
				return CharacterAnimation.characterAnimateState.LEFT;
			}
			else if (towardCell.Y < currentCell.Y){
				return CharacterAnimation.characterAnimateState.UP;
			}
			return CharacterAnimation.characterAnimateState.DOWN;
		}

		public void MoveTowardHero() {
			this.characterAnimation.currentAnimateState = this.characterDirection(this.currentCell, this.gameManager.fieldManager.fieldObjectManager.hero.currentCell);
		}

		public virtual void StartMoving() {
			this.isMoving = true;
			this.characterAnimation.SetAnimationSpeed(true);
		}

		public IObservable<bool> MoveToTarget(List<Cell> routeCells, float moveVelocityFactor = NORMAL_MOVE_VELOCITY_FACTOR, bool isForceMove = false) {
			this.routeCells = routeCells;
			this.moveVelocityFactor = moveVelocityFactor;
			this.isForceMove = isForceMove;
			Cell currentCell = routeCells[0];
			routeCells.RemoveAt (0);
			if (routeCells.Count > 0) {
				this.nextTargetCell = routeCells[0];
				routeCells.RemoveAt (0);
			} else {
				// move toward only
				this.nextTargetCell = currentCell;
			}
			
			this.characterAnimation.currentAnimateState = characterDirection(currentCell, this.nextTargetCell);
			this.StartMoving();
			
			return this.UpdateAsObservable()
				.Select(_ => this.isMoving)
				.TakeWhile(_ => this.isMoving );
        }
        
        void EndMoving() {
			this.isMoving = false;
			this.isForceMove = false;
			this.characterAnimation.SetAnimationSpeed(false);
		}

		public void Destroy() {
			Destroy(this.gameObject);
		}

		public int GetCharacterId() {
			return characterId;
        }

		public void Jump() {
			this.currentPosition = this.transform.localPosition;
			this.jumpHeightPosition = new Vector3(this.currentPosition.x, this.currentPosition.y + JUMP_HEIGHT, this.currentPosition.z);
			this.isUpping  = true;
			this.isJumping = true;
		}

		void Jumping() {
			if (isUpping) {
				if (Vector2.Distance(jumpHeightPosition, transform.localPosition) > 0.05f) {
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, jumpHeightPosition, JUMP_VELOCITY);
	            } else {
					transform.localPosition = this.jumpHeightPosition;
					isUpping = false;
				}
			} else {
				if (Vector2.Distance(this.currentPosition, transform.localPosition) > 0.05f) {
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.currentPosition, JUMP_VELOCITY);
				} else {
					transform.localPosition = this.currentPosition;
					this.isJumping = false;
                }
            }
        }

		void SetPos() {
			Vector3 currentPos = this.transform.localPosition;
			this.transform.localPosition = new Vector3(currentPos.x, currentPos.y, Z_POS);
		}
	}
}