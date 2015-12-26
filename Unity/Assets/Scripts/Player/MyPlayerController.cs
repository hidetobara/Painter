using UnityEngine;
using System.Collections;


namespace Painter
{
	public class MyPlayerController : PlayerController
	{
		public Camera MainCamera;
		private Vector3 _WeaponBias = new Vector3(0, 0, 4);
		private Vector3 _CameraBias = new Vector3(0, 4, -4);

		private GameObject Weapon;
		private Vector3 _WeaponAngle;
		public Vector3 WeaponAngle
		{
			set { Weapon.transform.rotation = gameObject.transform.rotation; Weapon.transform.Rotate(value); }
			get { return _WeaponAngle; }
		}

		private PlayerMovement _Movement;

		private static MyPlayerController _Instance;
		public static MyPlayerController Instance
		{
			get
			{
				if (_Instance == null) _Instance = FindObjectOfType<MyPlayerController>();
				return _Instance;
			}
		}

		void Awake()
		{
			Weapon = transform.FindChild("Weapon").gameObject;
		}

		void Start()
		{
			// 固定設定
			_Player = ConstantEnviroment.Instance.FriendPlayer;
			_Weapon = ConstantEnviroment.Instance.MyWeapon;
			// グループ
			_Player.Group = GroupProperty.GROUP1;
			HubController.Get(_Player.Group).Restart(this);
			// 動き
			_Movement = new PlayerMovement(_Player);

			DebugDialog.Instance.Initialize();
		}

		public void SetID(int group, string id)
		{
			_Player.Group = group;
			_Player.ID = id;
			//gameObject.name = _Player.ID;
		}

		void Update()
		{
			_Movement.Update();

			// Around
			Vector3 angle = gameObject.transform.rotation.eulerAngles;
			angle += new Vector3(0, _Movement.GetAround(), 0);
			gameObject.transform.rotation = Quaternion.Euler(angle);

			// Move
			Vector3 move = gameObject.transform.rotation * new Vector3(_Movement.GetVelocitySide(), 0, _Movement.GetVelocityForward());
			gameObject.transform.position += move;

			// Camera
			Vector3 target = gameObject.transform.position + Weapon.transform.rotation * _WeaponBias;
			MainCamera.transform.position = gameObject.transform.position + Weapon.transform.rotation * _CameraBias;
			MainCamera.transform.LookAt(target);

			// Attack
			if(_IsAttacking && _AttackInterval > _Weapon.Interval)
			{
				ActAttack();
				_AttackInterval = 0;
			}
			_AttackInterval += Time.deltaTime;
			WeaponAngle *= 0.9f;

			// Network
			NetworkManager.Instance.AddNotify(this);
		}

		void OnCollisionStay(Collision collision)
		{
			int damp = 0;
			foreach(var contact in collision.contacts)
			{
				GameObject o = contact.otherCollider.gameObject;
				InkPlaneController plane = o.GetComponent<InkPlaneController>();
				if (plane == null) continue;

				int group = plane.CalclateGroup(contact.point);
				if (group == 0) continue;

				if (group == _Player.Group) damp++; else damp--;
			}

			if (damp < 0) _Movement.SetPlane(PlayerMovement.PlaneStatus.Enemies);
			else if (damp > 0) _Movement.SetPlane(PlayerMovement.PlaneStatus.Friends);
			else _Movement.SetPlane(PlayerMovement.PlaneStatus.None);
		}

		#region 移動
		public void MoveForward(float rate = 1)
		{
			_Movement.MoveForward(rate);
		}
		public void MoveBack(float rate = 1)
		{
			_Movement.MoveBack(rate);
		}
		public void TurnLeft(float rate = 1)
		{
			_Movement.TurnLeft(rate);
		}
		public void TurnRight(float rate = 1)
		{
			_Movement.TurnRight(rate);
		}
		#endregion

		#region 攻撃
		public void ActAttack()
		{
			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			Rigidbody r = o.GetComponent<Rigidbody>();
			o.transform.position = Weapon.transform.position + Weapon.transform.rotation * new Vector3(0, 0, 1);
			o.transform.rotation = Weapon.transform.rotation;
			r.velocity = o.transform.rotation * new Vector3(RandomSide(_Weapon.ScatterHorizontal), RandomSide(_Weapon.ScatterVertical), _Weapon.Velocity);

			InkBallController controller = o.GetComponent<InkBallController>();
			controller.Initialize(_Player.Group, _Weapon);

			NetworkManager.Instance.AddNotify(controller);
		}
		float RandomSide(float v) { return Random.Range(-v, v); }

		float _AttackInterval = 0;
		bool _IsAttacking = false;
		public void ActAttackStart()
		{
			_IsAttacking = true;
		}
		public void ActAttackEnd()
		{
			_IsAttacking = false;
		}
		#endregion
	}
}