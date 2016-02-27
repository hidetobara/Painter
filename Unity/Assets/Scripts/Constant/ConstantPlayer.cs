using System;
using System.Collections.Generic;
using UnityEngine;


namespace Painter
{
	public class PlayerProperty
	{
		public int Group;
		public string ID = "Any";

		public virtual float ForwardRate { get { return 0.12f; } }
		public virtual float AroundRate { get { return 2f; } }
	}
}
