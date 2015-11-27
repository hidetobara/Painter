using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InputAdapter : MonoBehaviour
	{
		void Awake()
		{
			Input.gyro.enabled = true;
		}

		void Update()
		{
			float forward = Input.acceleration.y;	// 奥に転がすとプラス、手前に転がすとマイナス
			float around = Input.acceleration.x;	// 右に傾けるとプラス、左に傾けるとマイナス

			var player = MainPlayerController.Instance;
			if (forward > 0.1f) player.ActForward(forward);
			else if (forward < -0.1f) player.ActBack(-forward);
			if (around > 0.1f) player.ActRight(around);
			else if (around < 0.1f) player.ActLeft(-around);
		}

		public void OnPressDown()
		{
			MainPlayerController.Instance.ActAttackStart();
		}
		public void OnPressUp()
		{
			MainPlayerController.Instance.ActAttackEnd();
		}
	}
}