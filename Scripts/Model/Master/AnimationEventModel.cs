using System.Linq;

namespace TimeTraveler {
	public class AnimationEventModel : EventBaseModel<AnimationEventModel>{
		
		protected override string getTableName() { return "animation_event";}
		public bool withNextChildEvent {get; private set;}

		public string prefab {get; private set;}
		public string tag {get; private set;}
		public int animationType {get; private set;}
		public double animationSpeed {get; private set;}
		
		public AnimationEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
