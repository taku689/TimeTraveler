using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace TimeTraveler {
	public class OverrideButton : MonoBehaviour, IPointerDownHandler {
		public delegate void _Func();
		_Func func;
		
		public void SetOnPointerDown(_Func func) {
			this.func = func;
		}
		
		public void OnPointerDown(PointerEventData data) {
			func();
		}
	}
}
