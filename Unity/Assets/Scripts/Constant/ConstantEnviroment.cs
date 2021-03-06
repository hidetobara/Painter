﻿#define STAGING
using UnityEngine;
using System.Collections;
using Painter.Site;

namespace Painter
{
	public class ConstantEnviroment : MonoBehaviour
	{
		const string InkBallName = "InkBall";
		const string InkSubBallName = "InkSubBall";
		const string InkScatterName = "InkSplash";
		const string PlayerName = "Player";

		public UnityEngine.Object PrefabInkBall { get; private set; }
		public UnityEngine.Object PrefabSubInkBall { get; private set; }
		public UnityEngine.Object PrefabInkScatter { get; private set; }
		public UnityEngine.Object PrefabPlayer { get; private set; }

		public WeaponTable WeaponTable { get; private set; }
		public NetworkProperty Network { get; private set; }
		public GroupProperty Group { get; private set; }
		public SceneProperty Scene { get; private set; }

		private static ConstantEnviroment _Instance;
		public static ConstantEnviroment Instance
		{
			get
			{
				if (_Instance == null)
				{
					GameObject o = new GameObject("ConstantEnviroment");
					_Instance = o.AddComponent<ConstantEnviroment>();
				}
				return _Instance;
			}
		}

		void Awake()
		{
			Debug.Log("[Enviroment] Awake");
			if (_Instance == null) _Instance = GetComponent<ConstantEnviroment>();

			NetworkManager.Instance.Initialize();

			PrefabInkBall = Resources.Load(InkBallName);
			PrefabSubInkBall = Resources.Load(InkSubBallName);
			PrefabInkScatter = Resources.Load(InkScatterName);
			PrefabPlayer = Resources.Load(PlayerName);

			WeaponTable = new WeaponTable();
			Network = new NetworkProperty();
			Group = new GroupProperty();
			Scene = new SceneProperty();

#if STAGING
			Network = new StagingNetworkProperty();
#endif
		}

		public Color Color1 { get { return Group.GetColor(1); } }
		public Color Color2 { get { return Group.GetColor(2); } }
	}
}