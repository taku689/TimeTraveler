using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{ 


	public class GameStateManager {

		public enum gameState {
			DEMO,
			MESSAGE,
			MOVE,
			MOVING,
			MENU,
			MODAL,
		};

		GameManager gameManager;

		public GameStateManager (gameState state, GameManager gameManager) {
			this.gameManager = gameManager;
			currentGameState = state;
		}

		gameState _currentGameState;
		public gameState currentGameState {
			get { return this._currentGameState; } 
			private set {
				this._currentGameState = value;
				gameManager.interactionManager.changeInteractionByGameState(this._currentGameState);
			}
		}

		public void SetDemoState() {
			this.currentGameState = gameState.DEMO;
		}

		public void SetMessageState() {
			this.currentGameState = gameState.MESSAGE;
		}

		public void SetMoveState() {
			this.currentGameState = gameState.MOVE;
		}

		public void SetMovingState() {
			this.currentGameState = gameState.MOVING;
		}

		public void SetMenuState() {
			this.currentGameState = gameState.MENU;
		}

		public void SetModalState() {
			this.currentGameState = gameState.MODAL;
		}
	}
}
