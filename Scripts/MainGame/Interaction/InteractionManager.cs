using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeTraveler;

namespace TimeTraveler.MainGame {
	public class InteractionManager {

		InteractionEvent interactionEvent;
		CellManager cellManager;
		EventManager eventManager;

		public InteractionManager(InteractionEvent interactionEvent, CellManager cellManager, EventManager eventManager) {
			this.interactionEvent = interactionEvent;
			this.cellManager = cellManager;
			this.eventManager = eventManager;
		}

		public void changeInteractionByGameState (GameStateManager.gameState gameState) {
			switch (gameState) {
			case GameStateManager.gameState.DEMO:
				this.setDemoInteraction();
				break;
			case GameStateManager.gameState.MESSAGE:
				this.setMessageInteraction();
				break;
			case GameStateManager.gameState.MOVE:
				this.setMoveInteraction();
				break;
			case GameStateManager.gameState.MENU:
				this.setMenuInteraction();
				break;
			case GameStateManager.gameState.MOVING:
				this.setMovingInteraction();
				break;
			case GameStateManager.gameState.MODAL:
				this.setModalInteraction();
				break;
			}

		}

		void setDemoInteraction() {
			this.resetAllInteraction();
			interactionEvent.TouchDown += eventManager.OnTouchScreen;
		}

		void setMessageInteraction() {
			this.resetAllInteraction();
			interactionEvent.TouchDown += eventManager.OnTouchScreen;
		}

		void setMoveInteraction() {
			this.resetAllInteraction();
			interactionEvent.TouchDown += cellManager.OnTouchCell;
		}

		void setMenuInteraction() {
			this.resetAllInteraction();
		}

		void setMovingInteraction() {
			this.resetAllInteraction();
		}

		void setModalInteraction() {
			this.resetAllInteraction();
		}

		void resetAllInteraction() {
			interactionEvent.TouchDown -= cellManager.OnTouchCell;
			interactionEvent.TouchDown -= eventManager.OnTouchScreen;
		}


	}
}
