using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Painter
{
	public class WeaponTable
	{
		Dictionary<string, WeaponProperty> _Table = new Dictionary<string, WeaponProperty>();
		
		public WeaponTable()
		{
			List<WeaponProperty> list = new List<WeaponProperty>();
			foreach (var p in list) { _Table.Add(p.Name, p); }
		}
		public WeaponProperty Get(string name)
		{
			if (_Table.ContainsKey(name)) return _Table[name];
			return new WeaponProperty();
		}
	}

	public class WeaponProperty
	{
		public virtual string Name { get { return "gun"; } }
		// Attack
		public virtual float Interval { get { return 0.5f; } }

		// Bullet
		public virtual float StallTime { get { return 0.3f; } }
		public virtual float BrokenTime { get { return 2.0f; } }
		public virtual float Velocity { get { return 10.0f; } }
	}

	public class WeaponRifleProperty : WeaponProperty
	{
		public virtual string Name { get { return "rifle"; } }
		// Attack
		public override float Interval { get { return 3.0f; } }

		// Bullet
		public override float BrokenTime { get { return 1.0f; } }
		public override float Velocity { get { return 20.0f; } }
	}
}