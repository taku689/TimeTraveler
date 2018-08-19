using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler {
	public class ScheneSwitchData {

		public enum LoadType {
			NewGame,
			LoadGame,
		};

		public LoadType loadType {get; private set;}
		public int userId {get; private set;}

		public ScheneSwitchData(LoadType loadType, int userId) {
			this.loadType = loadType;
			this.userId = userId;
		}
	}
}
