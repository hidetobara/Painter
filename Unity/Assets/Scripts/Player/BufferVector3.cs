using System;
using UnityEngine;

namespace Painter
{
	public class BufferVector3
	{
		public Vector3 Real { set; get; }
		public Vector3 Buffered { get; private set; }

		public void Update()
		{
			Buffered = Vector3.Lerp(Buffered, Real, 0.5f);
		}
	}
}
