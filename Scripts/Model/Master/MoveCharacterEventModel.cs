using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class MoveCharacterEventModel : EventBaseModel<MoveCharacterEventModel>{
		
		protected override string getTableName() { return "move_character_event";}
		
		public int characterId {get; private set;}
		public int initCellNum {get; private set;}
		public int moveTargetCellNum {get; private set;}
		public bool isMoveToHero {get; private set;}
		public double moveVelocity;
		public float MoveVelocity { get { return (float)this.moveVelocity;} }
		public bool isDisappearAfterMoved {get; private set;}

		public MoveCharacterEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
