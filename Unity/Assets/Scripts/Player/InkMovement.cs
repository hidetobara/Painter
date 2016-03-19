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
		private bool IsAutoLoading { get { return _Property.IsAutoReloading; } }
		private float EnergyMax { get { return _Property.EnegyMax; } }
		public float EnergyRate { get { return _Energy / _Property.EnegyMax; } }
		public bool IsDead { get { return _Energy <= 0; } }

		public float CharageMax { get { return _Property.ChargeMax; } }
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

		public float Fire(bool enable)
		{
			if (IsAutoLoading) return FireWithReload(enable);
			else return FireWithoutReload(enable);
		}

		private float FireWithoutReload(bool enable)
		{
			if(!enable)
			{
				float tmp = _Charaged;
				_Charaged = 0;
				return tmp;
			}
			if (_Charaged > CharageMax) return 0;

			float delta = Time.deltaTime;
			_Charaged += delta;
			_Energy -= delta;
			return 0;
		}

		private float FireWithReload(bool enable)
		{
			if (!enable)
			{
				_Charaged = 0;
				return 0;
			}

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
#if UNITY_EDITOR
			delta *= 0.1f;
#endif
			switch (_Plane)
			{
				case PlaneStatus.None: _Energy -= 1f * delta; break;
				case PlaneStatus.Enemies: _Energy -= 3f * delta; break;
			}
		}
	}
}