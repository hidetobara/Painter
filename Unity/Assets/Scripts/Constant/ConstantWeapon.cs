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
		public virtual float ChargeMax { get { return 0.5f; } }
		public virtual float ChargeMin { get { return 0.1f; } }
		public virtual float EnegyMax { get { return 5.0f; } }

		// Bullet
		public virtual float StallTime { get { return 0.2f; } }
		public virtual float BrokenTime { get { return 2.0f; } }
		public virtual float Velocity { get { return 10.0f; } }

		// Scatter
		public virtual float ScatterVertical { get { return 2.0f; } }
		public virtual float ScatterHorizontal { get { return 0.5f; } }
	}

	public class WeaponRifleProperty : WeaponProperty
	{
		public virtual string Name { get { return "rifle"; } }
		// Attack
		public override float ChargeMax { get { return 5.1f; } }

		// Bullet
		public override float BrokenTime { get { return 1.0f; } }
		public override float Velocity { get { return 20.0f; } }

		// Scatter
		public virtual float ScatterVertical { get { return 0.0f; } }
		public virtual float ScatterHorizontal { get { return 0.0f; } }
	}
}