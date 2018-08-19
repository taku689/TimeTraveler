using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class CommonEventModel : EventBaseModel<CommonEventModel> {
		
		protected override string getTableName() { return "common_event";}
		
		public int type {get; private set;}

		public CommonEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
