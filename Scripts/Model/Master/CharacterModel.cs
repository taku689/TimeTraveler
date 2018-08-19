using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class CharacterModel : EventBaseModel<CharacterModel>{
		
		protected override string getTableName() { return "character";}
		
		public string name {get; private set;}

		public CharacterModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
