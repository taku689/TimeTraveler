using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class ScreenTextEventModel : EventBaseModel<ScreenTextEventModel>{
		
		protected override string getTableName() { return "screen_text_event";}
		
		public string text {get; private set;}
		public int posX {get; private set;}
		public int posY {get; private set;}
		public int appearType {get; private set;}

		public ScreenTextEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
