using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class SoundEventModel : EventBaseModel<SoundEventModel> {

		protected override string getTableName() { return "sound_event";}

		public int soundId {get; private set;}
		public int operationType {get; private set;}
		public bool isLoop {get; private set;}
		public int fadeTime { get; private set;}

		public SoundEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
