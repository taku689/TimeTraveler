using UnityEngine;
using System.Collections;

namespace TimeTraveler.MainGame {
	public class SpriteController : MonoBehaviour
	{

		// Use this for initialization
		SpriteRenderer spriteRenderer;

		[SerializeField]
		float alpha;

		// Use this for initialization
		void Start()
		{
			this.spriteRenderer = this.GetComponent<SpriteRenderer>();
			this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b, alpha);
		}
	}
}
