using System.Linq;

namespace TimeTraveler {
	public class IntervalEventModel : EventBaseModel<IntervalEventModel>{
		
		protected override string getTableName() { return "interval_event";}
		public double time {get; private set;}
		
		public IntervalEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
		}
	}
}
