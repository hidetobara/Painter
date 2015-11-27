using UnityEngine;
using System.Collections;

namespace Painter
{
	public class WeaponProperty
	{
		// Attack
		public virtual float Interval { get { return 0.5f; } }

		// Bullet
		public virtual float StallTime { get { return 0.5f; } }
		public virtual float BrokenTime { get { return 3.0f; } }
		public virtual float Velocity { get { return 10.0f; } }
	}
}