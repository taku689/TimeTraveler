using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{
	public class ScreenTextEvent : EventBase {

		enum TextState {
			INITIALIZE,
			DISPLAYING,
			DISPLAY,
			WAIT_NEXT,
			WAIT_TOUCH,
			FINISH
		}

		enum AppearType {
			ESCAPEMENT = 1,
			FADE_IN    = 2,
		}

		[SerializeField]
		Text text;
		[SerializeField]
		RectTransform rectTransform;

		TextState currentTextState;
		const int TEXT_SPPED_PER_FRAME = 9;
		const int WAIT_TIME_IN_DISPLAY = 20;
		int currentFrame = 0;
		int currentStringNum = 0;
		int currentTextNum = 0;
		int currentWaitTimeInDisplay = 0;
		string[] texsts;



		ScreenTextEventModel screenTextEventModel;

		override public void Initialize() {
			currentTextState = TextState.INITIALIZE;
			this.text.text = "";
			this.UpdateAsObservable()
				.Where (_ => this.currentTextState == TextState.DISPLAYING)
				.Subscribe(_ => this.ShowingText());

			this.UpdateAsObservable()
				.Where (_ => this.currentTextState == TextState.DISPLAY)
				.Subscribe(_ => this.DisplayWait());
		}

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			this.screenTextEventModel = dataManager.screenTextEventModel.getModelById(eventManagerModel.eventChildId);
			this.texsts = this.screenTextEventModel.text.Split(new string[]{"{n}"}, System.StringSplitOptions.None);
			this.ShowText();
		}

		void ShowText() {
			this.rectTransform.localPosition = new Vector3((float)this.screenTextEventModel.posX, (float)this.screenTextEventModel.posY, this.rectTransform.position.z);
			this.text.enabled = true;
			this.currentFrame = 0;
			this.currentStringNum = 0;
			this.currentTextNum = 0;
			this.currentWaitTimeInDisplay = 0;
			this.currentTextState = TextState.DISPLAYING;
			if (this.screenTextEventModel.appearType == (int)AppearType.FADE_IN) {
				this.currentStringNum = this.texsts[this.currentTextNum].Length;
			}
		}

		void SetText() {
			this.currentFrame = 0;
			this.currentStringNum = 0;
			if (this.screenTextEventModel.appearType == (int)AppearType.FADE_IN) {
				this.currentStringNum = this.texsts[this.currentTextNum].Length;
			}
			this.currentWaitTimeInDisplay = 0;
		}

		void ShowingText() {
			this.text.text = this.texsts[this.currentTextNum].Substring(0, this.currentStringNum);
			if (this.currentStringNum == this.texsts[this.currentTextNum].Length) {
				this.currentTextNum++;
				if (this.currentTextNum >= this.texsts.Length) {
					this.currentTextState = TextState.DISPLAY;
				} else {
					this.currentTextState = TextState.WAIT_NEXT;
				}
				return;
			}
			this.currentFrame++;
			if (this.currentFrame >= TEXT_SPPED_PER_FRAME) {
				this.currentStringNum++;
				this.currentFrame = 0;
			}
		}

		void SkipText() {
			this.text.text = this.texsts[this.currentTextNum];
			this.currentStringNum = this.texsts[this.currentTextNum].Length;
			this.currentTextNum++;
			if (this.currentTextNum >= this.texsts.Length) {
				this.currentTextState = TextState.DISPLAY;
			} else {
				this.currentTextState = TextState.WAIT_NEXT;
			}
		}

		void DisplayWait() {
			if (this.currentWaitTimeInDisplay > WAIT_TIME_IN_DISPLAY) {
				this.currentTextState = TextState.WAIT_TOUCH;
			}
			this.currentWaitTimeInDisplay++;
		}

		void HideText() {
			this.text.text = "";
			this.text.enabled = false;
		}

		void NextText() {
			this.SetText();
			this.currentTextState = TextState.DISPLAYING;
		}
		
		override public void OnTouchScreen() {
			switch(currentTextState) {
			case TextState.WAIT_TOUCH:
				this.HideText();
				this.callBack();
				return;
			case TextState.DISPLAYING:
				this.SkipText();
				return;
			case TextState.WAIT_NEXT:
				this.NextText();
				return;
			default:
				return;
			}
		}
		
	}
}
