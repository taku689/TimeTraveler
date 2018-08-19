using UnityEngine;
using System;
using System.Collections;

namespace TimeTraveler
{
	public class InteractionEvent : MonoBehaviour 
	{
		
		public Vector3 InitialPos {
			get;
			private set;
		}
		public Vector3 EndPos {
			get;
			private set;
		}
		public event Action<InteractionEvent> TouchDown;
		public event Action<InteractionEvent> TouchUp;

		bool IsDown = false;

		void _OnMouseDown()
		{
			this.InitialPos = Input.mousePosition;
			this.IsDown = true;
			if (this.TouchDown != null) {
				this.TouchDown(this);
			}
		}
		
		void _OnMouseUp()
		{
			this.EndPos = Input.mousePosition;
			if(this.IsDown){
				this.IsDown = false;
				float x = InitialPos.x - EndPos.x;
				float y = InitialPos.y - EndPos.y;
				float force = Vector3.Distance(InitialPos, EndPos);
				if (this.TouchUp != null) {
					this.TouchUp(this);
				}
			}
		}

		void Update()
		{
			#if UNITY_EDITOR
			if(Input.GetMouseButtonDown(0)){
				this._OnMouseDown();
			}

			if(!Input.GetKey (KeyCode.Mouse0) && this.IsDown){
				this._OnMouseUp();
			}
			#endif
			if(Input.touchCount == 0) return;

			var touch = Input.touches[0];
			if(touch.phase == TouchPhase.Began){
				this._OnTouchDown();
			}
			else if(touch.phase == TouchPhase.Ended){
				this._OnTouchUp();
			}
			
		}

		void _OnTouchDown(){
			Vector2 pos = Input.touches[0].position;
			this.InitialPos = new Vector3(pos.x,pos.y,0.0f);
			this.IsDown = true;
			if (this.TouchDown != null) {
				this.TouchDown(this);
			}
		}
		
		void _OnTouchUp(){
			Vector2 pos = Input.touches[0].position;
			this.EndPos = new Vector3(pos.x,pos.y,0.0f);
			if(this.IsDown){
				this.IsDown = false;
				float x = InitialPos.x - EndPos.x;
				float y = InitialPos.y - EndPos.y;
				float force = Vector3.Distance(InitialPos, EndPos);
				if (this.TouchUp != null) {
					this.TouchUp(this);
				}
			}
		}
	}
}