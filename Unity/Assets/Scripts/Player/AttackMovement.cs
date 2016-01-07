using UnityEngine;
using System.Collections;


namespace Painter
{
	public class AttackMovement
	{
		public enum AttackStatus { None, Charging }
		private AttackStatus _Status = AttackStatus.None;

		private float EnergyMax { get { return _Property.EnegyMax; } }
		private float ChargeMin { get { return _Property.ChargeMin; } }
		private float ChargeMax { get { return _Property.ChargeMax; } }

		private float _Energy = 0;
		private float _Charge = 0;
		private WeaponProperty _Property;

		public float Energy { get { return _Energy; } }
		public float EnergyRate { get { return _Energy / _Property.EnegyMax; } }

		public AttackMovement(WeaponProperty p)
		{
			_Property = p;
		}

		public void On()
		{
			_Status = AttackStatus.Charging;
		}
		public void Off()
		{
			_Status = AttackStatus.None;
		}

		public float Fire()
		{
			if (_Status == AttackStatus.Charging)
			{
				if (_Charge > ChargeMax)
				{
					_Charge -= ChargeMax;
					return ChargeMax;
				}
				return 0;
			}
			else if(_Status == AttackStatus.None)
			{
				float charge = _Charge;
				_Charge = 0;
				if (charge < ChargeMin) charge = 0;
				return charge;
			}
			return 0;
		}

		public void Update()
		{
			float delta = Time.deltaTime;
			if (_Status == AttackStatus.None)
			{
				_Energy += delta;
				if (_Energy > EnergyMax) _Energy = EnergyMax;
			}
			else if(_Status == AttackStatus.Charging && _Energy > 0)
			{
				_Energy -= delta;
				_Charge += delta;
			}
		}
	}
}