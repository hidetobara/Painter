using UnityEngine;
using System.Collections;

public class PermanentEnvironment : MonoBehaviour
{
	private string PLAYER_NAME_KEY = "player_name";
	private string WEAPON_NAME_KEY = "weapon_name";

	// 永続
	public string PlayerName = "";
	public string WeaponName = "";

	private static PermanentEnvironment _Instance;
	public static PermanentEnvironment Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = FindObjectOfType<PermanentEnvironment>();
			}
			if (_Instance == null)
			{
				GameObject o = new GameObject("PermanentEnviroment");
				_Instance = o.AddComponent<PermanentEnvironment>();
				DontDestroyOnLoad(o);
			}
			return _Instance;
		}
	}

	public void Nop() { }

	private void Awake()
	{
		PlayerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "");
		WeaponName = PlayerPrefs.GetString(WEAPON_NAME_KEY, "");
	}

	public void Save()
	{
		PlayerPrefs.SetString(PLAYER_NAME_KEY, PlayerName);
		PlayerPrefs.SetString(WEAPON_NAME_KEY, WeaponName);
		PlayerPrefs.Save();
	}
}
