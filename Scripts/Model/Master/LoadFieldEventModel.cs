using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TimeTraveler.Model.User;

namespace TimeTraveler {
	public class LoadFieldEventModel : EventBaseModel<LoadFieldEventModel>{
		
		protected override string getTableName() { return "load_field_event";}
		
		public int fieldId {get; private set;}
		public int characterId {get; private set;}
		public int initCellNum {get; private set;}
		public bool withNextChildEvent {get; private set;}
		
		public LoadFieldEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}

		public LoadFieldEventModel buildByUserData() {
			UserModel userModel = this.dataManager.userModel.buildModel();
			LoadFieldEventModel loadFieldEventModel = new LoadFieldEventModel();
			loadFieldEventModel.Initialize(userModel.fieldId, userModel.characterId, userModel.cellNum, true);
			return loadFieldEventModel;
		}

		public void Initialize(int fieldId, int characterId, int initCellNum, bool withNextChildEvent) {
			this.fieldId = fieldId;
			this.characterId = characterId;
			this.initCellNum = initCellNum;
			this.withNextChildEvent = withNextChildEvent;
		}
	}
}
