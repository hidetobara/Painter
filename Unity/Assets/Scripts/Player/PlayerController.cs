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
			transform.position = p.Position;
			transform.rotation = p.Rotation;
		}
		public SyncPlayer Send()
		{
			return new SyncPlayer() { Id = _Player.ID, Position = transform.position, Rotation = transform.rotation };
		}
	}
}