using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InputAdapter : MonoBehaviour
	{
		enum InputState { INACTIVE, NONE, ATTACK, MOVE, ATTACK_AND_MOVE }
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
			InputState state = InputState.NONE;
			if (_IsPressedMove && _IsPressedAttack) state = InputState.ATTACK_AND_MOVE;
			else if (_IsPressedAttack) state = InputState.ATTACK;
			else if (_IsPressedMove) state = InputState.MOVE;

			// 横で立てた持ち方で
			// x: 初期0、右に傾けるとプラス、左に傾けるとマイナス
			// z: 初期0、奥に転がすとプラス、手前に転がすとマイナス
			Vector3 acc = Input.acceleration;
			Log.Instance.AddInfo("acc:" + acc);
#if UNITY_EDITOR || UNITY_STANDALONE
			state = InputState.INACTIVE;
#endif
			HandleLeftMoveRightAttack(state, acc);
		}

		void HandleLeftMoveRightAttack(InputState state,Vector3 acc)
		{
			var player = MyPlayerController.Instance;
			Vector3 weapon = new Vector3(Mathf.Clamp(EulerAsin(-acc.z), -45f, 30f), 0, 0);	// 視線
			const float DUMP = 0.4f;	// 減衰

			if(state == InputState.NONE)
			{
				player.ActAttackEnd();
				if (acc.x > 0) player.TurnRight(acc.x); else player.TurnLeft(-acc.x);
			}
			else if(state == InputState.MOVE)
			{
				player.ActAttackEnd();
				if (acc.z < 0) player.MoveForward(-acc.z); else player.MoveBack(acc.z);
				if (acc.x > 0) player.MoveRight(acc.x); else player.MoveLeft(-acc.x);
				weapon = Vector3.zero;
			}
			else if(state == InputState.ATTACK)
			{
				player.ActAttackStart();
				if (acc.x > 0) player.TurnRight(acc.x); else player.TurnLeft(-acc.x);
			}
			else if(state == InputState.ATTACK_AND_MOVE)
			{
				player.ActAttackStart();
				if (acc.x > 0) player.TurnRight(acc.x * DUMP); else player.TurnLeft(-acc.x * DUMP);
				if (acc.x > 0) player.MoveRight(acc.x * DUMP); else player.MoveLeft(-acc.x * DUMP);
				if (acc.z < 0) player.MoveForward(-acc.z * DUMP); else player.MoveBack(acc.z * DUMP);
			}
			// 視線
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
			return Mathf.Asin(Mathf.Clamp(v, -1, 1)) * 180.0f / Mathf.PI;
		}
		#endregion
	}
}