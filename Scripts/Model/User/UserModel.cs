using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeTraveler.Model.User {
	public class UserModel : ModelBase<UserModel>{

		const double MSEC = 1000;
		const double JP_DIFF = 32400000;
		private readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

		protected override string getTableName() { return "user";}

		public int fieldId {get; private set;}
		public int cellNum {get; private set;}
		public int characterId {get; private set;}
		public int date {get; private set;}
		public bool enableSkipButton {get; set;}
		public int saveUnixTime {get; private set;}

		public string SaveLoadTitle() {
			return _UnixTimeToDateTime(saveUnixTime).ToString();
		}

		override public void loadOrCreate(DataManager dataManager, int userId) {
			base.loadOrCreate(dataManager, userId);
			this.entity.list.ForEach(_=>_.Initialize(this.entity, dataManager));
			this.id = userId;
		}

		public UserModel buildModel() {
			List<UserModel> userModels = this.entity.list.Where (_=>_.id == this.id).ToList();
			if (userModels.Count > 0) {
				return userModels[0];
			} else {
				return this.initaialData();
			}
		}

		public void SetUserInfo(int cellNum, int fieldId, int characterId, int saveUserId) {
			this.id = saveUserId;
			this.cellNum = cellNum;
			this.fieldId = fieldId;
			this.characterId = characterId;
			this.saveUnixTime = CurrentUnixTime();
		}

		public void SetSaveData(int userId) {
			this.id = userId;
		}

		public void GoAprilTwelve() {
			this.date = (int)EventManagerModel.DateFlag.AprilTwelve;
  			this.dataManager.userEventModel.resetEndFlagIfHasResetFlag();
		}

		public void GoAprilThirteen() {
			this.date = (int)EventManagerModel.DateFlag.AprilThirteen;
		}

		public bool IsTwelve() {
			return this.date == (int)EventManagerModel.DateFlag.AprilTwelve;
		}

		public bool IsThirteen() {
			return this.date == (int)EventManagerModel.DateFlag.AprilThirteen;
        }
        
        UserModel initaialData() {
			UserModel model = new UserModel();
			int userId = this.dataManager.scheneSwithData.userId;
			model.loadOrCreate (this.dataManager, userId);
			model.id = userId;
			model.fieldId = 201;
			model.cellNum = 33;
			model.characterId = 1001;
			model.enableSkipButton = false;
			model.date = (int)EventManagerModel.DateFlag.AprilTwelve;
			this.entity.list.Add(model);
			return model;
		}

		private double _CurrentUnixTime()
		{
			return _DateTimeToUnixTime(DateTime.Now);
		}
		
		private double _DateTimeToUnixTime(DateTime dt)
		{
			return (dt.ToUniversalTime() - UNIX_EPOCH).TotalMilliseconds;
		}
		private DateTime _UnixTimeToDateTime(int periodSec)
		{
			return UNIX_EPOCH.AddMilliseconds(periodSec * MSEC + JP_DIFF);
		}
		public int CurrentUnixTime()
		{
			return (int)(_CurrentUnixTime() / MSEC);
		}


	}
}
