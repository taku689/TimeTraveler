using UnityEngine;
using System.Collections.Generic;
using TimeTraveler.MainGame;
using System.Linq;

namespace TimeTraveler {
	public class  EventManagerModel : ModelBase< EventManagerModel>{
		public enum DateFlag {
			AllDate       = 999,
			AprilTwelve   = 1,
			AprilThirteen = 2,
		};

		protected override string getTableName() { return "event_manager";}

		public List<int> endFlagEventIds { get; private set;}
		public List<int> openFlagEventIds { get; private set;}
		public List<int> forceLaunchFlagEventIds { get; private set;}
		public int dateFlag { get; private set;}
		public bool canResetEndFlagToTwelve {get; private set;}

		List<int> _forceLaunchEventIds = new List<int>();
		public List<int> forceLaunchEventIds {
			get {
				return this._forceLaunchEventIds;
			}
			private set {
				this._forceLaunchEventIds = value;
			}
		}

		public override void load(DataManager dataManager) {
			base.load(dataManager);
			foreach(EventManagerModel eventManagerModel in this.entity.list) {
				eventManagerModel.forceLaunchFlagEventIds.ForEach((id) => {
					EventManagerModel _eventManagerModel = this.GetyByEventId(id);
					//TODO: あるイベントが終わった後に発火するのではなく、フラグが立った後に発火するように修正する
					if (_eventManagerModel == null) {
						return;
					}
					_eventManagerModel._forceLaunchEventIds.Add (eventManagerModel.id);
				});
			}
		}

		public void SetForceEventToFlag() {
			foreach(EventManagerModel eventManagerModel in this.entity.list) {
				eventManagerModel.forceLaunchFlagEventIds.ForEach((id) => {
					EventManagerModel _eventManagerModel = this.GetyByEventId(id);
					// 強制フラグイベントは、イベントへの紐付けとフラグへの紐付けのパターンがあり、フラグの場合 
					if (_eventManagerModel == null) {
						var model = dataManager.userFlagModel.buildModelById(id);
						model.AddForceLaunchEventId(eventManagerModel.id);
                    }
                });
            }
	
		}

		public List<EventManagerModel> GetModels() {
			return this.entity.list;
		}

		public EventManagerModel GetyByEventId(int eventId) {
			var models = this.entity.list.Where(_ => _.id == eventId).ToList();
			if (models.Count == 0) {
				return null;
			}
			return models[0];
		}

		public bool CanLaunchAprilTwelve() {
			return this.dateFlag == (int)DateFlag.AprilTwelve || this.dateFlag == (int)DateFlag.AllDate;
		}

		public bool CanLaunchAprilThirteen() {
			return this.dateFlag == (int)DateFlag.AprilThirteen || this.dateFlag == (int)DateFlag.AllDate;
		}

		public bool CanLaunchEventByDate(int date) {
			if (this.dateFlag == (int)DateFlag.AllDate || date == this.dateFlag) {
				return true;
			}

			return false;
		}
	}
}
