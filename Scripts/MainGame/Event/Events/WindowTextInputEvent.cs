using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{
	public class WindowTextInputEvent : EventBase {
		
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
		
		TextState currentTextState;
		const int TEXT_SPPED_PER_FRAME = 3;
		const int WAIT_TIME_IN_DISPLAY = 20;
		
		int currentFrame = 0;
		int currentStringNum = 0;
		int currentTextNum = 0;
		int currentWaitTimeInDisplay = 0;
		string[] texsts;
		
		[SerializeField]
		Text text;
		[SerializeField]
		SpriteRenderer windowSprite;
		[SerializeField]
		GameObject InputField;
		[SerializeField]
		InputField inputFieldComponent;

		WindowTextInputEventModel windowTextInputEventModel;
		
		
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
			this.windowTextInputEventModel = dataManager.windowTextInputEventModel.getModelById(eventManagerModel.eventChildId);
			string text = this.windowTextInputEventModel.text.Replace("{e}", "\n");
			this.texsts = text.Split(new string[]{"{n}"}, System.StringSplitOptions.None);
			this.ShowText();
		}
		
		void ShowText() {
			this.text.enabled = true;
			windowSprite.enabled = true;
			this.currentFrame = 0;
			this.currentStringNum = 0;
			this.currentTextNum = 0;
			this.currentWaitTimeInDisplay = 0;
			this.currentTextState = TextState.DISPLAYING;
		}
		
		void SetText() {
			this.currentFrame = 0;
			this.currentStringNum = 0;
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
			this.currentStringNum = this.windowTextInputEventModel.text.Length;
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
		
		void NextText() {
			this.SetText();
			this.currentTextState = TextState.DISPLAYING;
		}
		
		void HideText() {
			this.text.text = "";
			this.text.enabled = false;
			windowSprite.enabled = false;
		}

		void SetInputWindow() {
			this.InputField.SetActive(true);
		}

		public void DecideInput() {
			int eventId;
			if (this.inputFieldComponent.text == this.windowTextInputEventModel.answer) {
				eventId = this.windowTextInputEventModel.eventIdSuccess;
			} else {
				eventId = this.windowTextInputEventModel.eventIdFail;
			}
			this.inputFieldComponent.text = "";
			this.InputField.SetActive(false);
			this.HideText();
			this.callBack();
			if (eventId != 0) {
				this.gameManager.LaunchEventByEvent(new List<int>() {eventId});
			}
		}
		
		override public void OnTouchScreen() {
			switch(currentTextState) {
			case TextState.WAIT_TOUCH:
				this.SetInputWindow();
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
