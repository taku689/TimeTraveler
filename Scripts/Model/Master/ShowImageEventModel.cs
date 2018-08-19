using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class ShowImageEventModel : EventBaseModel<ShowImageEventModel>{
		
		protected override string getTableName() { return "show_image_event";}

		public string imageName {get; private set;}
		public bool isAuto {get; private set;}
		public bool notHide {get; private set;}
		public bool withNextChildEvent {get; private set;}
		
		public ShowImageEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
