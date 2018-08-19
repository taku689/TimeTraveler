using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TimeTraveler.Model.User {
	public class UserEventModel : ModelBase<UserEventModel>{

		enum State {
			NOT_START = 1,
			SEEN      = 2,
			END       = 3,
		};

		public int state {get; private set;}

		public List<EventModel> eventModelList {get; private set;}
		public EventManagerModel eventManagerModel {get; private set;}

		override public void loadOrCreate(DataManager dataManager, int userId) {
			base.loadOrCreate(dataManager, userId);
			this.entity.list.ForEach(_=>_.Initialize(this.entity, dataManager));
		}
		
		protected override string getTableName() { return "user_event";}

		public UserEventModel buildModelById(int id) {
			List<UserEventModel> models = this.entity.list.Where(_ => _.id == id).ToList();
			if (models.Count == 0) {
				return this.initaialData(id);
			} else {
				models[0].build (id);
				return models[0];
			}
		}

		public List<UserEventModel> buildModelsByIds(List<int> ids) {
			List<UserEventModel> models = new List<UserEventModel>();
			foreach(int eventId in ids) {
				models.Add(this.dataManager.userEventModel.buildModelById(eventId));
			}
			return models;
		}

		public void resetEndFlagIfHasResetFlag() {
			foreach(UserEventModel userEventModel in this.entity.list) {
				if (userEventModel.eventManagerModel.canResetEndFlagToTwelve) {
					foreach( var id in userEventModel.eventManagerModel.endFlagEventIds)
					{
						this.dataManager.userFlagModel.FlagOff(id);
					}
					userEventModel.state = (int)State.NOT_START;
				}
			}
		}

		void build(int eventId) {
			this.eventModelList = this.dataManager.eventModel.GetEventByEventId(eventId);
			this.eventManagerModel = this.dataManager.eventManagerModel.GetyByEventId(eventId);
		}

		public bool canLaunchEvent() {
			bool canOpen = this.dataManager.userFlagModel.IsFlagOn(this.eventManagerModel.openFlagEventIds);
			canOpen = canOpen && this.canLaunchEventByDate();
			return canOpen && !this.IsEnd();
		}

		public bool canLaunchForceFlagEvent() {
			bool canOpen = this.dataManager.userFlagModel.IsFlagOn(this.eventManagerModel.forceLaunchFlagEventIds);
			canOpen = canOpen && this.canLaunchEventByDate();
			return canOpen && !this.IsEnd();
		}

		bool canLaunchEventByDate() {
			UserModel userModel = this.dataManager.userModel.buildModel();
			return this.eventManagerModel.CanLaunchEventByDate(userModel.date);
		}

		public void Seen() {
			this.state = (int)State.SEEN;
			List<UserEventModel> endFlagUserEventmodels = this.buildModelsByIds(this.eventManagerModel.endFlagEventIds);
			this.dataManager.userFlagModel.FlagOn(id);
			bool canEnd = this.dataManager.userFlagModel.IsFlagOn(this.eventManagerModel.endFlagEventIds);
			if (endFlagUserEventmodels.Count > 0 && canEnd) {
				this.End ();
			}
		}

		public List<int> GetForceLauncEventIds() {
			List<int> eventIds = new List<int>();
			foreach(int id in this.eventManagerModel.forceLaunchEventIds) {
				if (this.buildModelById(id).canLaunchForceFlagEvent()) {
					eventIds.Add (id);
				}
			}

			return eventIds;
		}

		void End() {
			this.state = (int)State.END;
		}

		public bool IsEnd() {
			return this.state == (int)State.END;
		}

		UserEventModel initaialData(int id) {
			UserEventModel model = new UserEventModel();
			model.id = id;
			model.state = (int)State.NOT_START;
			model.Initialize(this.entity, this.dataManager);
			model.build (id);
			this.entity.list.Add(model);
			return model;
		}
	}
}
