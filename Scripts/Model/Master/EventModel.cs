using UnityEngine;
using System.Collections.Generic;
using TimeTraveler.MainGame;
using System.Linq;

namespace TimeTraveler {
	public class  EventModel : ModelBase< EventModel>{

		protected override string getTableName() { return "event";}
		
		public int eventId {get; private set;}
		public int eventType {get; private set;}
		public int eventChildId {get; private set;}

		public List< EventModel> GetEventByEventId(int eventId) {
			return this.entity.list.Where(_ => _.eventId == eventId).ToList();
		}

	}
}
