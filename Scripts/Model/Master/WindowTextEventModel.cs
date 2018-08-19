using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class WindowTextEventModel : EventBaseModel<WindowTextEventModel>{

		protected override string getTableName() { return "window_text_event";}
		
		public string text {get; private set;}
		public int posType {get; private set;}

		public WindowTextEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
