using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;

namespace TimeTraveler {
	public class MasterDao {

		string masterPath = "Data/Master/";

		public TextAsset loadMasterDataByTableName(string tableName) {
			string fieldPath = masterPath + tableName;
			return Resources.Load<TextAsset>(fieldPath);
		}
	}
}
