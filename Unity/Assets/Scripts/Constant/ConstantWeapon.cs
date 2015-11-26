using UnityEngine;
using System.Collections;

namespace Painter
{
	public class ConstantWeapon
	{
		private static ConstantWeapon _Instance;
		public static ConstantWeapon Instance
		{
			get { if (_Instance == null) _Instance = new ConstantWeapon(); return _Instance; }
		}

		public WeaponProperty Default = new WeaponProperty();
	}

	public class WeaponProperty
	{
		// Time
		public virtual float StallTime { get { return 0.5f; } }
		public virtual float BrokenTime { get { return 1.0f; } }

		public virtual float Velocity { get { return 10.0f; } }
	}
}