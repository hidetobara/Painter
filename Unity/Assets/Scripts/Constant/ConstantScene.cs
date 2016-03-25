﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneProperty
{
	public const string Select = "SelectProperty";
	public const string Home = "Home";
	public const string Stage1 = "Stage1";
	public const string Stage2 = "Stage2";

	private Dictionary<string, int> _TablePort = new Dictionary<string, int>()
	{
		{ Home, 55000 },
		{ Stage1, 55001 }, { Stage2, 55002 }
	};

	public int GetPort(string scene)
	{
		if (!_TablePort.ContainsKey(scene)) return 55000;
		return _TablePort[scene];
	}
}