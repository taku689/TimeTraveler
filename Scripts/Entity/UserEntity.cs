using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using LitJson;

namespace TimeTraveler {
	public class UserEntity<T> {

		public List<T> list = new List<T>();
		UserDao userDao;

		public UserEntity() {
			this.userDao = new UserDao();
		}

		public void loadOrCreate(string tableName, int userId) {
			this.userDao = new UserDao();
			string data = this.userDao.loadUserDataByTableName(tableName, userId);
			if (data == null) {
				this.create(tableName, userId);
			} else {
				this.load(data);
			}
		}

		public bool hasUserData(string tableName, int userId) {
			string data = this.userDao.loadUserDataByTableName(tableName, userId);
			if (data == null) {
				return false;
			}
			this.load(data);
			if (this.list.Count > 0) {
				return true;
			} else {
				return false;
			}
		}

		void create(string tableName, int userId) {
			string jsonString = LitJson.JsonMapper.ToJson(new UserEntity<T>());
			this.userDao.Save(tableName, jsonString, userId);
		}

		void load(string data) {
			EntityBase<T> jsonData = LitJson.JsonMapper.ToObject<EntityBase<T>>(data);
			this.list = jsonData.list;
		}

		public void load(string tableName, int userId) {
			this.userDao = new UserDao();
			string data = this.userDao.loadUserDataByTableName(tableName, userId);
			this.load (data);
		}

		public void Reset(string tableName, int userId) {
			this.create(tableName, userId);
		}

		public void Save(string tableName, int userId) {
			string jsonString = LitJson.JsonMapper.ToJson(this);
			this.userDao.Save(tableName, jsonString, userId);
		}
	}
}