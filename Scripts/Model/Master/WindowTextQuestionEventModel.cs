using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TimeTraveler {
	public class WindowTextQuestionEventModel : EventBaseModel<WindowTextQuestionEventModel>{
		
		protected override string getTableName() { return "window_text_question_event";}
		
		public string text {get; private set;}

		public string answer1 {get; private set;}
		public string answer2 {get; private set;}
		public string answer3 {get; private set;}
		public string answer4 {get; private set;}
		public string answer5 {get; private set;}

		public int eventId1 {get; private set;}
		public int eventId2 {get; private set;}
		public int eventId3 {get; private set;}
		public int eventId4 {get; private set;}
		public int eventId5 {get; private set;}
		
		public WindowTextQuestionEventModel getModelById(int id) {
			return this.entity.list.Where(_ => _.id == id).ToList()[0];
        }
    }
}
