using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneProperty
{
	public enum SceneName { Home = 55000, Stage1 = 55001, Stage2 = 55002, Stage3 = 55003 }

	public int GetPort(string name)
	{
		foreach(SceneName n in Enum.GetValues(typeof(SceneName)))
		{
			if(n.ToString() == name) return (int)n;
		}
		return 55000;	// とりあえず
	}

	public int GetPort(SceneName scene)
	{
		return (int)scene;
	}
}
