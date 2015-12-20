#define STAGING
using UnityEngine;
using System.Collections;
using Painter.Site;

namespace Painter
{
	public class ConstantEnviroment : MonoBehaviour
	{
		const string InkBallName = "InkBall";
		const string InkScatterName = "InkScatter";
		const string PlayerName = "Player";

		public UnityEngine.Object PrefabInkBall { get; private set; }
		public UnityEngine.Object PrefabInkScatter { get; private set; }
		public UnityEngine.Object PrefabPlayer { get; private set; }

		public PlayerProperty FriendPlayer { get; private set; }
		public PlayerProperty EnemyPlayer { get; private set; }
		public WeaponProperty MyWeapon { get; private set; }
		public WeaponTable WeaponTable { get; private set; }
		public NetworkProperty Network { get; private set; }
		public GroupProperty Group { get; private set; }

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
			PrefabPlayer = Resources.Load(PlayerName);

			FriendPlayer = new PlayerProperty();
			EnemyPlayer = new PlayerProperty();
			MyWeapon = new WeaponProperty();
			WeaponTable = new WeaponTable();
			Network = new NetworkProperty();
			Group = new GroupProperty();
#if STAGING
			Network = new StagingNetworkProperty();
#endif
		}

		public Color FriendColor { get { return Group.GetColor(FriendPlayer.Group); } }
		public Color EnemyColor { get { return Group.GetColor(EnemyPlayer.Group); } }
	}
}