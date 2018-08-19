using UnityEngine;
using System.Collections.Generic;

namespace TimeTraveler.MainGame{ 
	public class EventObject : MonoBehaviour {
		
		[SerializeField]
		int _width;
		[SerializeField]
		int _height;
		[SerializeField]
		int cellNumber;
		[SerializeField]
		List<int> _eventIds;
		
		public int width  {get{ return this._width;}}
		public int height {get{ return this._height;}}
		public List<int> eventIds {get {return this._eventIds;}}

		public void setCellNumber(int value) { this.cellNumber = value; }
		public void setWidth(int value)      { this._width     = value; }
		public void setHeight(int value)     { this._height    = value; }

		public List<int> getArea() {
			List<int> area = new List<int>();
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					area.Add(this.cellNumber + x + y * 10);
				}
			}
			return area;
		}
		
	}
}
