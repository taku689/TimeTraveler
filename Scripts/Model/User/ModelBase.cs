using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler.Model.User {
	public class ModelBase<T> {

		public int id;
		protected UserEntity<T> entity;
		protected DataManager dataManager;

		virtual protected void Initialize(UserEntity<T> entity, DataManager dataManager) {
			this.entity = entity;
			this.dataManager = dataManager;
		}

		virtual public void loadOrCreate(DataManager dataManager, int userId) {
			UserEntity<T> entity = new UserEntity<T>();
			string tableName = this.getTableName();
			this.Initialize (entity, dataManager);
			entity.loadOrCreate(tableName, userId);
		}

		public void load(int userId) {
			UserEntity<T> entity = new UserEntity<T>();
			string tableName = this.getTableName();
			entity.load(tableName, userId);
			this.entity = entity;
		}

		public bool HasData(int userId) {
			if (this.entity == null) {
				return false;
			} else {
				UserEntity<T> entity = new UserEntity<T>();
				if (entity.hasUserData(this.getTableName(), userId)) {
					return true;
				} else {
					return false;
				}
			}
		}

		public void Save(int userId) {
			this.entity.Save(this.getTableName(), userId);
		}

		public void Reset(int userId) {
			this.entity.Reset(this.getTableName(), userId);
		}

		protected virtual string getTableName() { return ""; }
	}
}