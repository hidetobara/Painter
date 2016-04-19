using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace Painter
{
	public class StageController : MonoBehaviour
	{
		const string UI_UNITY = "UIShoot";

		void Start()
		{
			SceneManager.LoadScene(UI_UNITY, LoadSceneMode.Additive);
		}
	}
}