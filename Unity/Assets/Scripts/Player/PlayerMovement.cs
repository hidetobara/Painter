using UnityEngine;
using System.Collections;


namespace Painter
{
	public class PlayerMovement
	{
		public enum LifeStatus { Preparing, Starting, Playing }
		public enum PlaneStatus { None, Friends, Enemies }
		public enum ActStatus { None, Attacking }

		const float STARTING_TIME_LIMIT = 3.0f;

		private LifeStatus _Life = LifeStatus.Preparing;
		private float _StartingTime = 0;
		public void BecomeStarting() { _Life = LifeStatus.Starting; _StartingTime = 0; }
		public void BecomePlaying() { _Life = LifeStatus.Playing; }

		private PlaneStatus _Plane = PlaneStatus.None;
		public void SetPlane(PlaneStatus s) { _Plane = s; }

		private ActStatus _Walk;
		public void SetAct(ActStatus s) { _Walk = s; }

		PlayerProperty _Property;
		float _VelocityForward;
		float _VelocitySide;
		float _Around;

		public string PrintStatus() { return _Life + " " + _Plane + " " + _Walk; }

		public PlayerMovement(PlayerProperty property)
		{
			_Property = property;
		}

		public void MoveForward(float v = 1)
		{
			_VelocityForward = _Property.ForwardRate * Mathf.Clamp(v, 0, 1);
		}
		public void MoveBack(float v = 1)
		{
			_VelocityForward = -_Property.ForwardRate * Mathf.Clamp(v, 0, 1) * 0.5f;
		}
		public void MoveLeft(float v = 1)
		{
			_VelocitySide = _Property.ForwardRate * Mathf.Clamp(v, 0, 1);
		}
		public void MoveRight(float v = 1)
		{
			_VelocitySide = -_Property.ForwardRate * Mathf.Clamp(v, 0, 1);
		}
		public void TurnLeft(float v = 1)
		{
			_Around = -_Property.AroundRate * Mathf.Clamp(v, 0, 1);
		}
		public void TurnRight(float v = 1)
		{
			_Around = _Property.AroundRate * Mathf.Clamp(v, 0, 1);
		}

		public void Update()
		{
			if(_Life == LifeStatus.Preparing || _Life == LifeStatus.Starting)
			{
				if (_StartingTime > STARTING_TIME_LIMIT) _Life = LifeStatus.Playing;	// 開始する

				Clear();
				_StartingTime += Time.deltaTime;
				return;
			}

			Damp();
		}

		private void Clear()
		{
			_VelocityForward = 0;
			_VelocitySide = 0;
			_Around = 0;
			_Walk = ActStatus.None;
		}

		private void Damp()
		{
			// 床による影響
			switch(_Plane)
			{
				case PlaneStatus.Enemies:
					_VelocityForward *= 0.1f;
					_VelocitySide *= 0.1f;
					_Around *= 0.5f;
					break;
				case PlaneStatus.Friends:
					_VelocityForward *= 0.8f;
					_VelocitySide *= 0.8f;
					_Around *= 0.8f;
					break;
				default:
					_VelocityForward *= 0.5f;
					_VelocitySide *= 0.5f;
					_Around *= 0.8f;
					break;
			}

			// 行動による影響
			if(_Walk == ActStatus.Attacking)
			{
				_VelocityForward *= 0.75f;
				_VelocitySide *= 0.75f;
			}

			// 状態更新
			if (IsFew(_VelocityForward) && IsFew(_VelocitySide) && IsFew(_Around))
			{
				Clear();
			}
		}

		public float GetVelocityForward()
		{
			return _VelocityForward;
		}
		public float GetVelocitySide()
		{
			return _VelocitySide;
		}
		public float GetAround()
		{
			return _Around;
		}

		private bool IsFew(float v) { return Mathf.Abs(v) < 0.01f; }
	}
}