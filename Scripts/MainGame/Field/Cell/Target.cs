using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TimeTraveler.MainGame {
	public class Target : MonoBehaviour {

		const int displayTargetSec = 3;

		public void turnTarget(float x, float y) {
			transform.localPosition = new Vector3(x, y, -2.0f);
			gameObject.SetActive(true);
		}

		public void turnTargetWithLimit(float x, float y) {
			transform.localPosition = new Vector3(x, y, -2.0f);
			gameObject.SetActive(true);
			StartCoroutine("fadeTargetByLimit");
		}

		IEnumerator fadeTargetByLimit() {
			yield return new WaitForSeconds(displayTargetSec);
			gameObject.SetActive(false);
		}

		public void fadeTarget() {
			gameObject.SetActive(false);
		}

	}
}
