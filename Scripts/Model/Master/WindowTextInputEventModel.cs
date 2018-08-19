using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class WindowTextInputEventModel : EventBaseModel<WindowTextInputEventModel>{
		
		protected override string getTableName() { return "window_text_input_event";}
		
		public string text {get; private set;}
		public string answer {get; private set;}
		public int eventIdSuccess {get; private set;}
		public int eventIdFail {get; private set;}

		public WindowTextInputEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
