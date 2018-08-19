using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler {
	public class FieldModel : ModelBase<FieldModel>{

		public int cell;
		protected override string getTableName() { return "field";}

		public FieldModel getModelById() {
			foreach(FieldModel model in this.entity.list) {
				Debug.Log (model.id);
			}

			return null;
		}



	}
}
