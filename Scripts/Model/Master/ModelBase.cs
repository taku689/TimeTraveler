using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler {
	public class ModelBase<T> {

		public int id;
		protected MasterEntity<T> entity;
		protected DataManager dataManager;

		public virtual void load(DataManager dataManager) {
			MasterEntity<T> entity = new MasterEntity<T>();
			string tableName = this.getTableName();
			entity.load(tableName);
			this.entity = entity;
			this.dataManager = dataManager;
		}

		protected virtual string getTableName() { return ""; }
	}
}
