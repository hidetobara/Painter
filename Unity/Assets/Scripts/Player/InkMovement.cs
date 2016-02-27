using UnityEngine;
using System.Collections;


namespace Painter
{
	/*
	 * インク量と体力を同じにした
	 */
	public class InkMovement
	{
		private WeaponProperty _Property;

		float _Energy;
		private float EnergyMax { get { return _Property.EnegyMax; } }
		public float EnergyRate { get { return _Energy / _Property.EnegyMax; } }
		public bool IsDead { get { return _Energy <= 0; } }

		private float CharageMax { get { return _Property.ChargeMax; } }
		float _Charaged;

		PlaneStatus _Plane;

		public InkMovement(WeaponProperty p)
		{
			_Property = p;
			Reset();
		}

		public void BecomeDead() { _Energy = 0; }
		public void Reset() { _Energy = EnergyMax; _Plane = PlaneStatus.Friends; }
		public void SetPlane(PlaneStatus p) { _Plane = p; }

		public float Fire()
		{
			float delta = Time.deltaTime;
			_Charaged += delta;
			_Energy -= delta;
			if (_Charaged > CharageMax)
			{
				_Charaged -= CharageMax;
				return CharageMax;
			}
			return 0;
		}

		public void Update()
		{
			// 消費
			float delta = Time.deltaTime;
			switch (_Plane)
			{
				case PlaneStatus.None: _Energy -= 1f * delta; break;
				case PlaneStatus.Enemies: _Energy -= 3f * delta; break;
			}
		}
	}
}