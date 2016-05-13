using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Painter
{
	public class SelectWeaponController : MonoBehaviour
	{
		void Start()
		{
			PermanentEnvironment.Instance.Nop();
		}

		public void OnPressGun()
		{
			PermanentEnvironment.Instance.WeaponName = "gun";
		}

		public void OnPressRifle()
		{
			PermanentEnvironment.Instance.WeaponName = "rifle";
		}

		public void OnPressGoStage()
		{
			Debug.Log(PermanentEnvironment.Instance.WeaponName);
			PermanentEnvironment.Instance.Save();
			SceneProperty.SceneName name = SceneProperty.SceneName.Home;
			switch (DateTime.Now.Hour % 2)
			{
				case 0: name = SceneProperty.SceneName.Stage1; break;
				case 1: name = SceneProperty.SceneName.Stage2; break;
			}
			//name = SceneProperty.SceneName.Stage3;
			SceneManager.LoadScene(name.ToString());
		}
	}
}