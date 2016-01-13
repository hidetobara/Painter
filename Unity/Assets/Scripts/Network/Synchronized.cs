using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Painter
{
	using JsonHash = Dictionary<string, object>;
	using JsonList = List<object>;
	public enum NetworkStatus { None, Joining, Accept, Update, End }

	public class Synchronized
	{
		protected const string NAME = "nam";
		protected const string GROUP = "grp";
		protected const string ID = "id";
		protected const string STATUS = "sta";
		protected const string TIME = "tim";
		protected const string POSITION = "pos";
		protected const string ROTATION = "rot";
		protected const string VELOCITY = "vel";
		protected const string TYPE = "typ";

		public static string MyId;
		public string Id;
		public string ShortId { get { return Id.Substring(0, 4); } }
		public int Group;
		public int Time;

		public virtual string Name { get; protected set; }

		public virtual JsonHash ToHash()
		{
			JsonHash hash = new JsonHash();
			hash[NAME] = Name;
			hash[ID] = MyId;
			hash[GROUP] = Group;
			//hash[TIME] = Time;
			return hash;
		}

		public bool IsMine() { return MyId == Id; }

		protected string VectorToString(Vector3 v)
		{
			return string.Format("{0:f2}|{1:f2}|{2:f2}", v.x, v.y, v.z);
		}
		protected string QuaternionToString(Quaternion q)
		{
			return string.Format("{0:f2}|{1:f2}|{2:f2}|{3:f2}", q.x, q.y, q.z, q.w);
		}

		static List<Synchronized> _Instances = new List<Synchronized>() { new SyncStatus(), new SyncPlayer(), new SyncBall() };
		public static Synchronized ParseHash(JsonHash hash)
		{
			if (hash == null) return null;
			foreach (var i in _Instances)
			{
				var s = i.Parse(hash);
				if (s != null) { return s; }
			}
			return null;
		}
		public static List<Synchronized> ParseList(JsonList list)
		{
			if (list == null || list.Count == 0) return null;

			List<Synchronized> syncs = new List<Synchronized>();
			foreach (var o in list)
			{
				Synchronized s = ParseHash(o as JsonHash);
				if (s != null) syncs.Add(s);
			}
			return syncs;
		}
		protected virtual Synchronized Parse(JsonHash hash)
		{
			return null;
		}

		protected string ParseString(JsonHash hash, string key)
		{
			if (!hash.ContainsKey(key) || hash[key] == null) return null;
			return hash[key].ToString();
		}
		protected int ParseInt(JsonHash hash, string key)
		{
			if (!hash.ContainsKey(key) || hash[key] == null) return 0;
			int value = 0;
			if (int.TryParse(hash[key].ToString(), out value)) return value;
			return 0;
		}
		protected float ParseFloat(JsonHash hash, string key)
		{
			if (!hash.ContainsKey(key) || hash[key] == null) return 0;
			float value = 0;
			if (float.TryParse(hash[key].ToString(), out value)) return value;
			return 0;
		}
		protected Vector3 ParseVector3(JsonHash hash, string key)
		{
			string str = ParseString(hash, key);
			if (str == null) return Vector3.zero;
			string[] cells = str.Split('|');
			try { return new Vector3(float.Parse(cells[0]), float.Parse(cells[1]), float.Parse(cells[2])); }
			catch { return Vector3.zero; }
		}
		protected Quaternion ParseQuaternion(JsonHash hash, string key)
		{
			string str = ParseString(hash, key);
			if (str == null) return Quaternion.identity;
			string[] cells = str.Split('|');
			try { return new Quaternion(float.Parse(cells[0]), float.Parse(cells[1]), float.Parse(cells[2]), float.Parse(cells[3])); }
			catch { return Quaternion.identity; }
		}
		protected T ParseEnum<T>(JsonHash hash, string key)
		{
			string str = ParseString(hash, key);
			if (str == null) return default(T);
			foreach (T t in Enum.GetValues(typeof(T)))
			{
				if (t.ToString().ToLower() == str.ToLower()) return t;
			}
			return default(T);
		}
	}

	public class SyncStatus : Synchronized
	{
		public override string Name { get { return "sta"; } }
		public NetworkStatus Status;
		public override JsonHash ToHash()
		{
			JsonHash hash = base.ToHash();
			hash[STATUS] = Status.ToString().ToLower();
			return hash;
		}
		protected override Synchronized Parse(JsonHash hash)
		{
			if (ParseString(hash, NAME) != Name) return null;
			return new SyncStatus() { Id = ParseString(hash, ID), Status = ParseEnum<NetworkStatus>(hash, STATUS), Group = ParseInt(hash, GROUP) };
		}
	}

	public class SyncSpace : Synchronized
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public override JsonHash ToHash()
		{
			JsonHash hash = base.ToHash();
			hash[POSITION] = VectorToString(Position);
			hash[ROTATION] = QuaternionToString(Rotation);
			return hash;
		}
		protected virtual SyncSpace Retrieve(JsonHash hash)
		{
			Id = ParseString(hash, ID);
			Position = ParseVector3(hash, POSITION);
			Rotation = ParseQuaternion(hash, ROTATION);
			Group = ParseInt(hash, GROUP);
			return this;
		}
	}
	public class SyncPlayer : SyncSpace
	{
		public override string Name { get { return "pla"; } }
		protected override Synchronized Parse(JsonHash hash)
		{
			if (ParseString(hash, NAME) != Name) return null;
			return new SyncPlayer().Retrieve(hash) as SyncPlayer;
		}
	}
	public class SyncBall : SyncSpace
	{
		public override string Name { get { return "bal"; } }
		public string Type;
		public Vector3 Velocity;
		public override JsonHash ToHash()
		{
			JsonHash hash = base.ToHash();
			hash[TYPE] = Type;
			hash[VELOCITY] = VectorToString(Velocity);
			return hash;
		}
		protected override Synchronized Parse(JsonHash hash)
		{
			if (ParseString(hash, NAME) != Name) return null;
			SyncBall i = new SyncBall() { Type = ParseString(hash, TYPE), Velocity = ParseVector3(hash, VELOCITY) };
			i.Retrieve(hash);
			return i;
		}
	}
}
