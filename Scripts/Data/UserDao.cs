using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;
using System.IO;

namespace TimeTraveler {
	public class UserDao {
		
		public String loadUserDataByTableName(string tableName, int userId) {
			string filePath = this.getFilePath(tableName, userId);
			string data;
			try {
				data = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
			} catch {
				return null;
			}
			return data;
		}

		public void Save(string tableName, string data, int userId) {
			string filePath = this.getFilePath(tableName, userId);
			File.WriteAllText(filePath, data, System.Text.Encoding.UTF8);
		}

		String getFilePath(string tableName, int userId) {
			return (Application.persistentDataPath + "/" + tableName + "_" + userId);
		}

	}
}
