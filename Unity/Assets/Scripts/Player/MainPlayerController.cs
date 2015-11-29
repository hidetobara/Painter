using UnityEngine;
using System.Collections;


namespace Painter
{
	public class MainPlayerController : MonoBehaviour
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

		// 定数
		private PlayerProperty _Player;
		private WeaponProperty _Weapon;

		private static MainPlayerController _Instance;
		public static MainPlayerController Instance
		{
			get
			{
				if (_Instance == null) _Instance = FindObjectOfType<MainPlayerController>();
				return _Instance;
			}
		}

		void Start()
		{
			ConstantEnviroment.Instance.Initialize();
			_Player = ConstantEnviroment.Instance.FriendPlayer;
			_Weapon = ConstantEnviroment.Instance.MyWeapon;
			DebugDialog.Instance.Initialize();

			Weapon = transform.FindChild("Weapon").gameObject;
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
		}

		#region 移動
		float _Velocity = 0;
		float _Around = 0;
		public void ActForward(float rate = 1)
		{
			_Velocity = _Player.ForwardRate * Mathf.Clamp(rate, 0, 1);
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
			_Velocity = -_Player.BackRate * Mathf.Clamp(rate, 0, 1);
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
			controller.Initialize(_Player, _Weapon);
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