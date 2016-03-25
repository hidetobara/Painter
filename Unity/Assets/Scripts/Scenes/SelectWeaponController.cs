using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
		SceneManager.LoadScene(SceneProperty.Stage2);
	}
}
