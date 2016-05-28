using UnityEngine;
using System.Collections;

namespace Painter
{
	public class WallController : MonoBehaviour
	{
		public Texture2D WallImage;

		void Start()
		{
			if (WallImage == null) return;
			var render = GetComponent<Renderer>();
			render.material.mainTexture = WallImage;
		}
	}
}