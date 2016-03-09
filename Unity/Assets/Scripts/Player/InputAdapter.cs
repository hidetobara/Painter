using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InputAdapter : MonoBehaviour
	{
		private bool _IsPressedAttack = false;	// Right
		private bool _IsPressedMove = false;	// Left
		private Vector3 _BufferWeaponAngle;

		void Awake()
		{
			Input.gyro.enabled = true;
			_BufferWeaponAngle = Vector3.zero;
		}

		void Update()
		{
			HandleRotate();
		}

		private void HandleLeftRightRotate()
		{
			var player = MyPlayerController.Instance;
			if (_IsPressedMove && _IsPressedAttack) player.MoveForward();
			if (_IsPressedMove && !_IsPressedAttack) player.TurnLeft();
			if (!_IsPressedMove && _IsPressedAttack) player.TurnRight();
			if (!_IsPressedMove && !_IsPressedAttack) player.ActAttackStart(); else player.ActAttackEnd();

#if !UNITY_EDITOR && !UNITY_STANDALONE
			float ay = Input.acceleration.y;	// 初期0、奥に転がすとプラス、手前に転がすとマイナス

			Vector3 weapon = new Vector3(Mathf.Clamp(EulerAsin(ay) + 30f, -30f, 30f), 0, 0); Log.Instance.AddInfo("weapon:" + weapon);
			_BufferWeaponAngle = _BufferWeaponAngle * 0.9f + weapon * 0.1f;
			player.WeaponAngle = _BufferWeaponAngle;
#endif
		}

		private void HandleLeftMoveRightAttack()
		{
			var player = MyPlayerController.Instance;
#if !UNITY_EDITOR && !UNITY_STANDALONE
			float ax = Input.acceleration.x;	// 初期0、右に傾けるとプラス、左に傾けるとマイナス
			float ay = Input.acceleration.y;	// 初期0、奥に転がすとプラス、手前に転がすとマイナス

			Vector3 weapon = new Vector3(Mathf.Clamp(EulerAsin(ay) + 30f, -30f, 30f), 0, 0); Log.Instance.AddInfo("weapon:" + weapon);
			_BufferWeaponAngle = _BufferWeaponAngle * 0.9f + weapon * 0.1f;
			player.WeaponAngle = _BufferWeaponAngle;

			if (ax > 0.1f) player.TurnRight(ax);
			else if (ax < 0.1f) player.TurnLeft(-ax);
#endif
			if (_IsPressedMove) player.MoveForward();
		}

		private void HandleRotate()
		{
			Vector3 weapon = Vector3.zero;

			var player = MyPlayerController.Instance;
#if !UNITY_EDITOR && !UNITY_STANDALONE
			float ax = Input.acceleration.x;	// 初期0、右に傾けるとプラス、左に傾けるとマイナス
			float ay = Input.acceleration.y;	// 初期0、奥に転がすとプラス、手前に転がすとマイナス
			if (!_IsPressedAttack && !_IsPressedMove)
			{
				if (ax > 0) player.MoveRight(ax); else player.MoveLeft(-ax);
				if (ay > 0) player.MoveForward(ay); else player.MoveBack(-ay);
				player.ActAttackEnd();
			}
			else
			{
				if (ax > 0.1f) player.TurnRight(ax);
				else if (ax < 0.1f) player.TurnLeft(-ax);
				player.ActAttackStart();

				weapon = new Vector3(Mathf.Clamp(EulerAsin(ay), -45f, 15f), 0, 0);
			}
#endif
			_BufferWeaponAngle = _BufferWeaponAngle * 0.9f + weapon * 0.1f;
			player.WeaponAngle = _BufferWeaponAngle;
		}

		public void OnPressDownForAttack()
		{
			_IsPressedAttack = true;
		}
		public void OnPressUpForAttack()
		{
			_IsPressedAttack = false;
		}

		public void OnPressDownForMove()
		{
			_IsPressedMove = true;
		}
		public void OnPressUpForMove()
		{
			_IsPressedMove = false;
		}

		#region 便利
		float EulerAsin(float v)
		{
			return Mathf.Asin(Mathf.Clamp(Input.acceleration.y, -1, 1)) * 180.0f / Mathf.PI;
		}
		#endregion
	}
}