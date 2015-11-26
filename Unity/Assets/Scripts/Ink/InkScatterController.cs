using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InkScatterController : MonoBehaviour
	{
		public void SetColor(Color color)
		{
			var particle = GetComponent<ParticleSystem>();
			if (particle == null) return;
			particle.startColor = color;
		}

		void Start()
		{
			Destroy(this.gameObject, 1.1f);
		}
	}
}