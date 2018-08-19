using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame{
	public class WindowTextQuestionEvent : EventBase {
		
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

		const float ANSWER_MESSAGE_BG_MIN_POS_Y = 0.35f;
		const float ANSWER_MESSAGE_BG_MIN_SCALE_Y = 0.6f;
		const float ANSWER_MESSAGE_DIFF_HEIGHT = 0.25f;
		int currentFrame = 0;
		int currentStringNum = 0;
		int currentTextNum = 0;
		int currentWaitTimeInDisplay = 0;
		string[] texsts;
		bool isAlreadyAnswerSet = false;
		
		[SerializeField]
		Text text;
		[SerializeField]
		SpriteRenderer windowSprite;
		[SerializeField]
		GameObject backGround;
		[SerializeField]
		List<GameObject> answerButtons;

		WindowTextQuestionEventModel windowTextQuestionEventModel;

		
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
			this.windowTextQuestionEventModel = dataManager.windowTextQuestionEventModel.getModelById(eventManagerModel.eventChildId);
			string text = this.windowTextQuestionEventModel.text.Replace("{e}", "\n");
			this.texsts = text.Split(new string[]{"{n}"}, System.StringSplitOptions.None);
			this.ShowText();
		}

		void SetAnswers() {
			if (this.windowTextQuestionEventModel.answer1 != "") {
				this.SetAnswer(this.windowTextQuestionEventModel.answer1, this.windowTextQuestionEventModel.eventId1, answerButtons[0]);
			}

			if (this.windowTextQuestionEventModel.answer2 != "") {
				this.SetAnswer(this.windowTextQuestionEventModel.answer2, this.windowTextQuestionEventModel.eventId2, answerButtons[1]);
			}

			int heightIncreaseCnt = 0;
			if (this.windowTextQuestionEventModel.answer3 != "") {
				this.SetAnswer(this.windowTextQuestionEventModel.answer3, this.windowTextQuestionEventModel.eventId3, answerButtons[2]);
				heightIncreaseCnt++;
			}

			if (this.windowTextQuestionEventModel.answer4 != "") {
				this.SetAnswer(this.windowTextQuestionEventModel.answer4, this.windowTextQuestionEventModel.eventId4, answerButtons[3]);
				heightIncreaseCnt++;
			}

			if (this.windowTextQuestionEventModel.answer5 != "") {
				this.SetAnswer(this.windowTextQuestionEventModel.answer5, this.windowTextQuestionEventModel.eventId5, answerButtons[4]);
				heightIncreaseCnt++;
			}

			Vector3 localPosition = this.backGround.transform.localPosition;
			Vector3 localScale = this.backGround.transform.localScale;
			this.backGround.transform.localPosition = new Vector3(localPosition.x, ANSWER_MESSAGE_BG_MIN_POS_Y - ANSWER_MESSAGE_DIFF_HEIGHT * heightIncreaseCnt, localPosition.z);
			this.backGround.transform.localScale = new Vector3(localScale.x, ANSWER_MESSAGE_BG_MIN_SCALE_Y + ANSWER_MESSAGE_DIFF_HEIGHT * heightIncreaseCnt, localScale.z);
			this.backGround.SetActive(true);
			this.isAlreadyAnswerSet = true;
		}

		void showAnswers() {
		}

		void SetAnswer(string answerText, int eventId, GameObject answerButton) {
			answerButton.SetActive(true);
			answerButton.GetComponentInChildren<Text>().text = answerText;
			if (!this.isAlreadyAnswerSet) {
				answerButton.GetComponent<Button>().onClick.AddListener(() => {
					this.OnClickAnswer(eventId);
				});
			}
		}

		void OnClickAnswer(int eventId) {
			this.backGround.SetActive(false);
			this.answerButtons.ForEach(_ => _.SetActive(false));
			this.HideText();
			this.callBack();
			if (eventId != 0) {
				this.gameManager.LaunchEventByEvent(new List<int>() {eventId});
			}
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
			this.currentStringNum = this.windowTextQuestionEventModel.text.Length;
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
		
		override public void OnTouchScreen() {
			switch(currentTextState) {
			case TextState.WAIT_TOUCH:
				this.SetAnswers();
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
