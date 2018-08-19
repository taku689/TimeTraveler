using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler.Model.User {
	public class UserFlagModel : ModelBase<UserFlagModel> {

		enum State {
			OFF = 1,
			ON  = 2,
		};

		public int state {get; private set;}

		private List<int> forceLaunchEventIds = new List<int>();

		protected override string getTableName() { return "user_flag";}

		override public void loadOrCreate(DataManager dataManager, int userId) {
			base.loadOrCreate(dataManager, userId);
			this.entity.list.ForEach(_=>_.Initialize(this.entity, dataManager));
		}

        public List<UserFlagModel> buildModelsByIds(List<int> ids) {
			List<UserFlagModel> models = new List<UserFlagModel>();
			foreach(int eventId in ids) {
				models.Add(this.dataManager.userFlagModel.buildModelById(eventId));
			}
			return models;
		}

		public UserFlagModel buildModelById(int id) {
			List<UserFlagModel> models = this.entity.list.Where(_ => _.id == id).ToList();
			if (models.Count == 0) {
				return this.initaialData(id);
			} else {
				return models[0];
			}
		}

		public List<int> GetForceLauncEventIds() {
			List<int> eventIds = new List<int>();
			foreach(int id in forceLaunchEventIds) {
				if (dataManager.userEventModel.buildModelById(id).canLaunchForceFlagEvent()) {
					eventIds.Add (id);
				}
			}
			
			return eventIds;
		}

		public void FlagOn(int id)
		{
			var model = buildModelById(id);
			model.state = (int)State.ON;
		}

		public void FlagOff(int id)
		{
			var model = buildModelById(id);
			model.state = (int)State.OFF;
		}

		public bool IsFlagOn(List<int> ids)
		{
			var models = buildModelsByIds(ids);
			return models.All(_ => _.state == (int)State.ON);
		}

		public void AddForceLaunchEventId(int id) {
			forceLaunchEventIds.Add(id);
		}

		UserFlagModel initaialData(int id) {
			UserFlagModel model = new UserFlagModel();
			model.id = id;
			model.state = (int)State.OFF;
			model.Initialize(this.entity, this.dataManager);
			this.entity.list.Add(model);
			return model;
		}
	}
}
