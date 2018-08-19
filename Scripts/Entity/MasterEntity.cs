using UnityEngine;
using System.Collections.Generic;
using LitJson;

namespace TimeTraveler {
	public class MasterEntity<T> {

		public List<T> list;

		public MasterEntity() {
		}

		public void load(string tableName) {
			MasterDao masterDao = new MasterDao();
			TextAsset data = masterDao.loadMasterDataByTableName(tableName);
			EntityBase<T> jsonData = LitJson.JsonMapper.ToObject<EntityBase<T>>(data.ToString());
			this.list = jsonData.list;
		}
	}
}
