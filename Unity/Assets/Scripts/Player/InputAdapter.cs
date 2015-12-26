using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InputAdapter : MonoBehaviour
	{
		private bool _IsPressedAttack = false;
		private bool _IsPressedMove = false;
		private Vector3 _BufferWeaponAngle;

		void Awake()
		{
			Input.gyro.enabled = true;
			_BufferWeaponAngle = Vector3.zero;
		}

		void Update()
		{
			var player = MyPlayerController.Instance;
#if !UNITY_EDITOR && !UNITY_STANDALONE
			float ax = Input.acceleration.x;	// 初期0、右に傾けるとプラス、左に傾けるとマイナス
			float ay = Input.acceleration.y;	// 初期0、奥に転がすとプラス、手前に転がすとマイナス

			Vector3 weapon = new Vector3(EulerAsin(ay) + 30f, 0, 0);
			_BufferWeaponAngle = _BufferWeaponAngle * 0.9f + weapon * 0.1f;
			player.WeaponAngle = _BufferWeaponAngle;

			if (ax > 0.1f) player.TurnRight(ax);
			else if (ax < 0.1f) player.TurnLeft(-ax);
#endif
			if (_IsPressedMove) player.MoveForward();
		}

		public void OnPressDownForAttack()
		{
			_IsPressedAttack = true;
			MyPlayerController.Instance.ActAttackStart();
		}
		public void OnPressUpForAttack()
		{
			_IsPressedAttack = false;
			MyPlayerController.Instance.ActAttackEnd();
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
		float EulerAsin(float v) { return Mathf.Asin(Input.acceleration.y) * 180.0f / Mathf.PI; }
		#endregion
	}
}