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

		private float _MoveDamper = 0.5f;

		private static MyPlayerController _Instance;
		public static MyPlayerController Instance
		{
			get
			{
				if (_Instance == null) _Instance = FindObjectOfType<MyPlayerController>();
				return _Instance;
			}
		}

		void Start()
		{
			_Player = ConstantEnviroment.Instance.FriendPlayer;
			_Weapon = ConstantEnviroment.Instance.MyWeapon;
			DebugDialog.Instance.Initialize();

			Weapon = transform.FindChild("Weapon").gameObject;

			_Player.Group = GroupProperty.GROUP1;
		}

		public void SetID(int group, string id)
		{
			_Player.Group = group;
			_Player.ID = id;
			//gameObject.name = _Player.ID;
		}

		void Update()
		{
			// Around
			Vector3 angle = gameObject.transform.rotation.eulerAngles;
			angle += new Vector3(0, _Around, 0);
			gameObject.transform.rotation = Quaternion.Euler(angle);
			_Around *= 0.75f;

			// Move
			Vector3 move = gameObject.transform.rotation * new Vector3(0, 0, _Velocity);
			gameObject.transform.position += move;
			_Velocity *= 0.8f;

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
			float goal = 0.5f;
			foreach(var contact in collision.contacts)
			{
				GameObject o = contact.otherCollider.gameObject;
				InkPlaneController plane = o.GetComponent<InkPlaneController>();
				if (plane == null) continue;

				int group = plane.CalclateGroup(contact.point);
				if (group == 0) continue;

				goal = (group == _Player.Group) ? 1f : 0.1f;
			}
			_MoveDamper = (_MoveDamper + goal) / 2.0f;
		}

		#region 移動
		float _Velocity = 0;
		float _Around = 0;
		public void ActForward(float rate = 1)
		{
			_Velocity = _Player.ForwardRate * Mathf.Clamp(rate, 0, 1) * _MoveDamper;
		}
		public void ActLeft(float rate = 1)
		{
			_Around = -_Player.AroundRate * Mathf.Clamp(rate, 0, 1);
		}
		public void ActRight(float rate = 1)
		{
			_Around = _Player.AroundRate * Mathf.Clamp(rate, 0, 1);
		}
		public void ActBack(float rate = 1)
		{
			_Velocity = -_Player.BackRate * Mathf.Clamp(rate, 0, 1) * _MoveDamper;
		}
		#endregion

		#region 攻撃
		public void ActAttack()
		{
			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			Rigidbody r = o.GetComponent<Rigidbody>();
			o.transform.position = Weapon.transform.position + Weapon.transform.rotation * new Vector3(0, 0, 1);
			o.transform.rotation = Weapon.transform.rotation;
			r.velocity = o.transform.rotation * new Vector3(0, 0, _Weapon.Velocity);

			InkBallController controller = o.GetComponent<InkBallController>();
			controller.Initialize(_Player.Group, _Weapon);

			NetworkManager.Instance.AddNotify(controller);
		}

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