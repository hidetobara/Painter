using UnityEngine;
using System.Collections;


namespace Painter
{
	public class MyPlayerController : PlayerController
	{
		public Camera MainCamera;
		private Vector3 _WeaponBias = new Vector3(0, 0, 2);	// (0,0,4)
		private Vector3 _CameraBias = new Vector3(0, 3, -6);	// (0,4,-4)

		private GameObject Weapon;
		private Vector3 _WeaponAngle;
		public Vector3 WeaponAngle
		{
			set { _WeaponAngle = value; Weapon.transform.rotation = transform.rotation; Weapon.transform.Rotate(value); }
			get { return _WeaponAngle; }
		}

		private PlayerMovement _PlayerMovement;
		private InkMovement _InkMovement;
		bool _IsAttacking = false;

		private Log _Log;

		private static MyPlayerController _Instance;
		public static MyPlayerController Instance
		{
			get
			{
				if (_Instance == null) _Instance = FindObjectOfType<MyPlayerController>();
				return _Instance;
			}
		}

		public int Group { get { return _Player.Group; } }

		void Awake()
		{
			_Log = Log.Instance;
			Weapon = transform.FindChild("Weapon").gameObject;
		}

		void Start()
		{
			// 固定設定
			_Player = new PlayerProperty();
			_Weapon = ConstantEnviroment.Instance.WeaponTable.Get(PermanentEnvironment.Instance.WeaponName);
			// 動き
			_PlayerMovement = new PlayerMovement(_Player);
			// インク
			_InkMovement = new InkMovement(_Weapon);

			DebugDialog.Instance.Initialize();
		}

		public void SetID(int group, string id)
		{
			_Player.Group = group;
			_Player.ID = id;

			BecomeStarting();
		}

		public void BecomeDead()
		{
			NetworkManager.Instance.AddStatus(NetworkStatus.Dead);
			// 間合いがあった方がいいか？
			BecomeStarting();
		}

		public void BecomeStarting()
		{
			HubController.Get(_Player.Group).Restart(this);
			_PlayerMovement.BecomeStarting();
			_InkMovement.Reset();
			NetworkManager.Instance.AddStatus(NetworkStatus.Born);
		}

		void Update()
		{
			// 共通処理
			_PlayerMovement.Update();
			_Log.AddInfo(_PlayerMovement.PrintStatus());

			// Camera
			Vector3 target = gameObject.transform.position + Weapon.transform.rotation * _WeaponBias;
			MainCamera.transform.position = gameObject.transform.position + Weapon.transform.rotation * _CameraBias;
			MainCamera.transform.LookAt(target);

			if (!_PlayerMovement.IsPlaying()) return;

			// 死亡した時
			if(_InkMovement.IsDead)
			{
				BecomeDead();
				return;
			}

			// Around
			Vector3 angle = gameObject.transform.rotation.eulerAngles;
			angle += new Vector3(0, _PlayerMovement.GetAround(), 0);
			gameObject.transform.rotation = Quaternion.Euler(angle);

			// Move
			Vector3 move = gameObject.transform.rotation * new Vector3(_PlayerMovement.GetVelocitySide(), 0, _PlayerMovement.GetVelocityForward());
			gameObject.transform.position += move;

			// Attack
			_InkMovement.Update();
			float fire = 0;
			fire = _InkMovement.Fire(_IsAttacking);
			if (fire > 0) ActAttack(fire / _InkMovement.CharageMax);
			WeaponPanel.Instance.Value = _InkMovement.EnergyRate;
			// Network
			NetworkManager.Instance.AddNotify(this);
		}

		void OnCollisionStay(Collision collision)
		{
			if (!_PlayerMovement.IsPlaying()) return;

			foreach(var contact in collision.contacts)
			{
				GameObject o = contact.otherCollider.gameObject;
				// 移動判定
				InkPlaneController ink = o.GetComponent<InkPlaneController>();
				if (ink != null) ink.CalclateGroup(contact.point, OnGrouped);
				// 死亡判定
				DeathPlaneController death = o.GetComponent<DeathPlaneController>();
				if (death != null) _InkMovement.BecomeDead();
			}
		}

		void OnGrouped(int group)
		{
			// 移動判定
			PlaneStatus plane = PlaneStatus.None;
			if (group > 0)
			{
				if (group != _Player.Group) plane = PlaneStatus.Enemies; else plane = PlaneStatus.Friends;
			}
			_PlayerMovement.SetPlane(plane);
			_InkMovement.SetPlane(plane);
		}

		#region 移動
		public void MoveForward(float rate = 1) { _PlayerMovement.MoveForward(rate); }
		public void MoveBack(float rate = 1) { _PlayerMovement.MoveBack(rate); }
		public void MoveLeft(float rate = 1) { _PlayerMovement.MoveLeft(rate); }
		public void MoveRight(float rate = 1) { _PlayerMovement.MoveRight(rate); }
		public void TurnLeft(float rate = 1) { _PlayerMovement.TurnLeft(rate); }
		public void TurnRight(float rate = 1) { _PlayerMovement.TurnRight(rate); }
		#endregion

		#region 攻撃
		public void ActAttack(float power = 1f)
		{
			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			Rigidbody r = o.GetComponent<Rigidbody>();
			o.transform.position = Weapon.transform.position + Weapon.transform.rotation * new Vector3(0, 0, 1);
			o.transform.rotation = Weapon.transform.rotation;
			r.velocity = o.transform.rotation * new Vector3(RandomSide(_Weapon.ScatterHorizontal), RandomSide(_Weapon.ScatterVertical), _Weapon.Velocity * power);

			InkBallController controller = o.GetComponent<InkBallController>();
			controller.Initialize(_Player.Group, _Weapon);

			NetworkManager.Instance.AddNotify(controller);
		}
		float RandomSide(float v) { return Random.Range(-v, v); }

		public void ActAttackStart()
		{
			_IsAttacking = true;
			_PlayerMovement.SetAct(ActStatus.Attacking);
		}
		public void ActAttackEnd()
		{
			_IsAttacking = false;
			_PlayerMovement.SetAct(ActStatus.None);
		}
		#endregion
	}
}