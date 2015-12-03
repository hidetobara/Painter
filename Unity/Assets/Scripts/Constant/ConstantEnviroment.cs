using UnityEngine;
using System.Collections;

namespace Painter
{
	public class ConstantEnviroment : MonoBehaviour
	{
		const string InkBallName = "InkBall";
		const string InkScatterName = "InkScatter";

		public UnityEngine.Object PrefabInkBall { get; private set; }
		public UnityEngine.Object PrefabInkScatter { get; private set; }

		public PlayerProperty FriendPlayer { get; private set; }
		public PlayerProperty EnemyPlayer { get; private set; }
		public WeaponProperty MyWeapon { get; private set; }
		public NetworkProperty Network { get; private set; }

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
			NetworkManager.Instance.Initialize();

			PrefabInkBall = Resources.Load(InkBallName);
			PrefabInkScatter = Resources.Load(InkScatterName);

			FriendPlayer = new PlayerProperty();
			EnemyPlayer = new PlayerProperty();
			MyWeapon = new WeaponProperty();
			Network = new NetworkProperty();
		}

	}
}