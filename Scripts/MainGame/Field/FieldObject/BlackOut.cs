using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

namespace TimeTraveler.MainGame {
	public class BlackOut : MonoBehaviour {
		[SerializeField]
		SpriteRenderer sprite;

		bool isTurning = false;
		bool toBlack;
		const float COLOR_CHANGE_RATE = 0.05f;

		public void Initialize() {
			this.UpdateAsObservable()
				.Where (_ => this.isTurning)
				.Subscribe(_ => this.Turn());
		}

		public IObservable<bool> TurnOpacity(bool toBlack) {
			this.isTurning = true;
			this.toBlack = toBlack;
			return this.UpdateAsObservable()
				.Select(_ => this.isTurning)
				.TakeWhile(_ => this.isTurning );
		}

		void Turn() {
			Color color = this.sprite.color;
			float colorChangeRate = COLOR_CHANGE_RATE;
			if (!toBlack) {
				colorChangeRate = -colorChangeRate;	
			}

			color.a += colorChangeRate;
			if (color.a < 0.0f || color.a > 1.0f) {
				if (toBlack) {
					color.a = 1.0f;
				} else {
					color.a = 0.0f;
				}
				this.isTurning = false;
			}
			this.sprite.color = color;
		}

	}
}
