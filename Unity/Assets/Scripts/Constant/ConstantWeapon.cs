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
			List<WeaponProperty> list = new List<WeaponProperty>() { new WeaponProperty(), new WeaponRifleProperty(), new WeaponBombProperty() };
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
		public virtual bool IsAutoReloading { get { return true; } }
		public virtual float ChargeMax { get { return 0.5f; } }
		public virtual float EnegyMax { get { return 30.0f; } }
		public virtual float Damage { get { return 5.0f; } }

		// Bullet
		public virtual float StallTime { get { return 0.05f; } }
		public virtual float BrokenTime { get { return 2.0f; } }
		public virtual float Velocity { get { return 7.5f; } }

		// Scatter
		public virtual int ScatterCount { get { return 1; } }
		public virtual float ScatterRange { get { return 1; } }
		public virtual float ScatterVertical { get { return 0.25f; } }
		public virtual float ScatterHorizontal { get { return 2.0f; } }
	}

	public class WeaponRifleProperty : WeaponProperty
	{
		public override string Name { get { return "rifle"; } }
		// Attack
		public override bool IsAutoReloading { get { return false; } }
		public override float ChargeMax { get { return 3.0f; } }
		public override float Damage { get { return 10.0f; } }

		// Bullet
		public override float StallTime { get { return 1.0f; } }
		public override float BrokenTime { get { return 5.0f; } }
		public override float Velocity { get { return 15.0f; } }

		// Scatter
		public override float ScatterVertical { get { return 0.0f; } }
		public override float ScatterHorizontal { get { return 0.0f; } }
	}

	public class WeaponBombProperty : WeaponProperty
	{
		public override string Name { get { return "bomb"; } }
		// Attack
		public override bool IsAutoReloading { get { return false; } }
		public override float ChargeMax { get { return 5.0f; } }
		public override float Damage { get { return 10.0f; } }

		// Bullet
		public override float StallTime { get { return 0.0f; } }
		public override float BrokenTime { get { return 1.0f; } }
		public override float Velocity { get { return 5.0f; } }

		// Scatter
		public override int ScatterCount { get { return 6; } }
		public override float ScatterRange { get { return 8.0f; } }
		public override float ScatterVertical { get { return 0.0f; } }
		public override float ScatterHorizontal { get { return 0.0f; } }
	}

	public class WeaponSplashProperty : WeaponProperty
	{
		public override string Name { get { return "splash"; } }
		// Attack
		public override float ChargeMax { get { return 0.0f; } }
		public override float Damage { get { return 1.0f; } }

		// Bullet
		public override float StallTime { get { return 0.0f; } }
		public override float BrokenTime { get { return 1.2f; } }
		public override float Velocity { get { return 0.0f; } }

		public override int ScatterCount { get { return 0; } }
	}
}