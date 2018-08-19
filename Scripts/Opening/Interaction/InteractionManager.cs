using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TimeTraveler;

namespace TimeTraveler.Opening {
	public class InteractionManager {

		GameManager gameManager;
		InteractionEvent interactionEvent;

		public InteractionManager(GameManager gameManager, InteractionEvent interactionEvent) {
			this.gameManager = gameManager;
			this.interactionEvent = interactionEvent;
			this.setInteraction();
		}
		
		void setInteraction() {
			this.resetAllInteraction();
		}
		
		void resetAllInteraction() {
		}
		
		
	}
}
