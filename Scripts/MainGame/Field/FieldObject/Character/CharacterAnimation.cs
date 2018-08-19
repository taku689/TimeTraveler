using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame {
		public class CharacterAnimation : MonoBehaviour {

		public enum characterAnimateState {
			UP    = 1,
			RIGHT = 2,
			DOWN  = 3,
			LEFT  = 4
		};

		public Sprite up1 {get; set;} 
		public Sprite up2 {get; set;} 
		public Sprite right1 {get; set;} 
		public Sprite right2 {get; set;} 
		public Sprite down1 {get; set;} 
		public Sprite down2 {get; set;} 
		public Sprite left1 {get; set;} 
		public Sprite left2 {get; set;} 
		public Sprite sprite1, sprite2;
		int FRAME = 30;
		bool isStart = false;

		SpriteRenderer spriteRenderer;

		int cnt = 0;
		bool isSprite1 = false;


		characterAnimateState _prevAnimateState;
		characterAnimateState _currentAnimateState;
		public  characterAnimateState currentAnimateState { 
			get {
				return this._currentAnimateState;
			}
			set { 
				this._prevAnimateState = this._currentAnimateState;
				this._currentAnimateState = value;
			} 
		}

		public void Initialize(CharacterModel characterModel) {
			this.loadSprites(characterModel);
			this.spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			this.spriteRenderer.sprite = down1;
			currentAnimateState = characterAnimateState.DOWN;
			_prevAnimateState = characterAnimateState.DOWN;
			isStart = true;
		}

		void loadSprites(CharacterModel characterModel) {
			string stringId = characterModel.id.ToString();
			this.up1  = Resources.Load<Sprite>("Img/Character/" + stringId + "/up1");
			this.up2  = Resources.Load<Sprite>("Img/Character/" + stringId + "/up2");
			this.right1  = Resources.Load<Sprite>("Img/Character/" + stringId + "/right1");
			this.right2  = Resources.Load<Sprite>("Img/Character/" + stringId + "/right2");
			this.down1  = Resources.Load<Sprite>("Img/Character/" + stringId + "/down1");
			this.down2  = Resources.Load<Sprite>("Img/Character/" + stringId + "/down2");
			this.left1  = Resources.Load<Sprite>("Img/Character/" + stringId + "/left1");
			this.left2  = Resources.Load<Sprite>("Img/Character/" + stringId + "/left2");
		}

		void Update () {
			if(!isStart) return;
			switch (_currentAnimateState){
			case characterAnimateState.UP:
				this.Up();
				break;
			case characterAnimateState.RIGHT:
				this.Right();
				break;
			case characterAnimateState.DOWN:
				this.Down();
				break;
			case characterAnimateState.LEFT:
				this.Left();
				break;
			}

			if (this.cnt >= FRAME) {
				if (isSprite1) {
					this.spriteRenderer.sprite = sprite2;
					isSprite1 = false;
				} else {
					this.spriteRenderer.sprite = sprite1;
					isSprite1 = true;
				}
				this.cnt = 0;
			}
			this.cnt++;
		}

		public void SetAnimationSpeed(bool isMoving) {
			if (isMoving) {
				FRAME = 10;
			} else {
				FRAME = 30;
			}
		}

		void Up () {
			sprite1 = up1;
			sprite2 = up2;
			if (_prevAnimateState != characterAnimateState.UP) {
				this.cnt = FRAME;
				_prevAnimateState = characterAnimateState.UP;
			}
		}

		void Right() {
			sprite1 = right1;
			sprite2 = right2;
			if (_prevAnimateState != characterAnimateState.RIGHT) {
				this.cnt = FRAME;
				_prevAnimateState = characterAnimateState.RIGHT;
			}
		}

		void Down() {
			sprite1 = down1;
			sprite2 = down2;
			if (_prevAnimateState != characterAnimateState.DOWN) {
				this.cnt = FRAME;
				_prevAnimateState = characterAnimateState.DOWN;
			}
		}

		void Left() {
			sprite1 = left1;
			sprite2 = left2;
			if (_prevAnimateState != characterAnimateState.LEFT) {
				this.cnt = FRAME;
				_prevAnimateState = characterAnimateState.LEFT;
			}
		}

		public void Jump() {
		}

	}
}
