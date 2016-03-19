using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Painter
{
	public class PlayerController : MonoBehaviour
	{
		#region Players
		static Dictionary<string, PlayerController> _Players = new Dictionary<string, PlayerController>();
		private static bool Register(PlayerController p)
		{
			if (_Players.ContainsKey(p._Player.ID)) return false;
			_Players[p._Player.ID] = p;
			return true;
		}
		public static PlayerController Get(string id)
		{
			if (!_Players.ContainsKey(id)) return null;
			return _Players[id];
		}
		#endregion

		protected PlayerProperty _Player = new PlayerProperty();
		protected WeaponProperty _Weapon = new WeaponProperty();
		private BufferVector3 _Position = new BufferVector3();

		private float _Energy;

		void Start()
		{
			_Energy = _Weapon.EnegyMax;
		}

		void Update()
		{
			_Position.Update();
			transform.position = _Position.Buffered;

			_Energy -= Time.deltaTime * 0.5f;
			if(_Energy < 0)
			{
				Debug.Log("[dead by energy] ID=" + _Player.ID);
				this.Alive = false;	// この端末上では死亡した
			}
		}

		public bool Alive
		{
			set { gameObject.SetActive(value); }
			get { return gameObject.activeSelf; }
		}

		public bool Register(SyncPlayer s)
		{
			var player = Get(s.Id);
			if (player != null) return false;

			_Player.ID = s.Id;
			Register(this);
			return true;
		}

		public void Recieve(SyncPlayer p)
		{
			if (p == null || p.Id != _Player.ID) return;
			if (!Alive) return;

			//transform.position = p.Position;
			_Position.Real = p.Position;
			transform.rotation = p.Rotation;
		}
		public SyncPlayer Send()
		{
			return new SyncPlayer() { Id = _Player.ID, Group = _Player.Group, Position = transform.position, Rotation = transform.rotation };
		}

		void OnCollisionEnter(Collision collision)
		{
			var controller = collision.gameObject.GetComponent<InkBallController>();
			if (controller == null) return;
			if (_Player.Group == controller.Group) return;

			_Energy -= controller.WeaponDamage;
		}
	}
}