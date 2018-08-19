using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class CharacterActionEventModel : EventBaseModel<CharacterActionEventModel>{
		
		protected override string getTableName() { return "character_action_event";}
		
		public int characterId {get; private set;}
		public int actionType {get; private set;}
		public int arg1 {get; private set;}
		public int arg2 {get; private set;}
		public int arg3 {get; private set;}

		public CharacterActionEventModel getModelById(int id) {
            return this.entity.list.Where(_ => _.id == id).ToList()[0];
        }
    }
}
