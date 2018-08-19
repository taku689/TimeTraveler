using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;

namespace TimeTraveler.MainGame{
	public class CharacterActionEvent : EventBase {

		enum actionType {
			CHANGE_DIRECTION 	   = 1,
			JUMP_ACTION      	   = 2,
			APPEAR_NEIGHBOUR 	   = 3,
			DISAPPEAR_BACK_TO_HERO = 4,
		};
		
		CharacterActionEventModel characterActionEventModel;

		override public void Launch(OnFinishEvent callBack, DataManager dataManager,  EventModel eventManagerModel, GameManager gameManager) {
			base.Launch(callBack, dataManager, eventManagerModel, gameManager);
			this.characterActionEventModel = this.dataManager.characterActionEventModel.getModelById(eventManagerModel.eventChildId);
			this.LaunchAction();
		}

		void LaunchAction() {
			switch(this.characterActionEventModel.actionType) {
			case (int) actionType.CHANGE_DIRECTION:
				this.ChangeDirectionAction();
				break;
			case (int) actionType.JUMP_ACTION:
				this.JumpAction();
				break;
			case (int) actionType.APPEAR_NEIGHBOUR:
				this.AppearNeighbourAction();
				break;
			case (int) actionType.DISAPPEAR_BACK_TO_HERO:
				this.DisappearBackToHero();
				break;
			default:
				Debug.LogError("undefined Event");
				break;
			}
		}

		void ChangeDirectionAction() {
			this.gameManager.fieldManager.fieldObjectManager.ChangeCharacterDirection(this.characterActionEventModel.characterId, this.characterActionEventModel.arg1);
			this.callBack();
		}

		void JumpAction() {
			this.gameManager.fieldManager.fieldObjectManager.CharacterJump(this.characterActionEventModel.characterId);
			this.callBack();
		}

		void AppearNeighbourAction() {
			this.gameManager.fieldManager.fieldObjectManager.MoveCharacterNextToHero(this.characterActionEventModel.characterId)
				.Subscribe( _ => { ;}, () => { this.AppearNeighbourActionOnMoved(); });
		}

		void AppearNeighbourActionOnMoved() {
			if (this.characterActionEventModel.arg1 == 0) {
				this.gameManager.fieldManager.fieldObjectManager.ChangeCharacterDirectionToHero(this.characterActionEventModel.characterId);
			} else {
				this.gameManager.fieldManager.fieldObjectManager.ChangeCharacterDirection(this.characterActionEventModel.characterId, this.characterActionEventModel.arg1);
			}
			this.callBack();
		}

		void DisappearBackToHero() {
			this.gameManager.fieldManager.fieldObjectManager.MoveCharacterBackToHero(this.characterActionEventModel.characterId)
				.Subscribe( _ => { ;}, () => { this.DisappearBackToHeroActionOnMoved(); });
		}

		void DisappearBackToHeroActionOnMoved() {
			this.gameManager.fieldManager.fieldObjectManager.RemoveCharacter(this.characterActionEventModel.characterId);
			this.callBack();
		}

	}
}
