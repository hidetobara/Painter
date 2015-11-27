using System;
using System.Collections.Generic;
using UnityEngine;


namespace Painter
{
	public class PlayerProperty
	{
		public virtual Color MainColor { get { return Color.red; } }

		public virtual float ForwardRate { get { return 0.5f; } }
		public virtual float BackRate { get { return 0.25f; } }
		public virtual float AroundRate { get { return 5f; } }
	}
}
