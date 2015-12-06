using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Painter
{
	using JsonHash = Dictionary<string, object>;
	using JsonList = List<object>;
	public enum NetworkStatus { None, Start, Update, End }

	public class Synchronized
	{
		protected const string NAME = "nam";
		protected const string ID = "id";
		protected const string STATUS = "sta";
		protected const string TIME = "tim";
		protected const string POSITION = "pos";
		protected const string ROTATION = "rot";
		protected const string VELOCITY = "vel";
		protected const string TYPE = "typ";

		public static string MyId;
		public string Id;

		public virtual string Name { get; protected set; }

		public virtual JsonHash ToHash()
		{
			JsonHash hash = new JsonHash();
			hash[NAME] = Name;
			hash[ID] = MyId;
			return hash;
		}

		bool IsMine() { return MyId == Id; }

		protected string VectorToString(Vector3 v)
		{
			return string.Format("{0:f2}|{1:f2}|{2:f2}", v.x, v.y, v.z);
		}
		protected string QuaternionToString(Quaternion q)
		{
			return string.Format("{0:f2}|{1:f2}|{2:f2}|{3:f2}", q.x, q.y, q.z, q.w);
		}

		public static List<Synchronized> Parse(JsonList list)
		{
			if (list == null || list.Count == 0) return null;

			List<Synchronized> instances = new List<Synchronized>() { new SyncStatus(), new SyncPlayer(), new SyncBall() };
			List<Synchronized> syncs = new List<Synchronized>();
			foreach (var o in list)
			{
				JsonHash h = o as JsonHash;
				if (h == null) continue;
				foreach (var i in instances)
				{
					var s = i.Parse(h);
					if (s != null) { syncs.Add(s); break; }
				}
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
		public float Time;
		public override JsonHash ToHash()
		{
			JsonHash hash = base.ToHash();
			hash[STATUS] = Status.ToString().ToLower();
			hash[TIME] = Time;
			return hash;
		}
		protected override Synchronized Parse(JsonHash hash)
		{
			if (ParseString(hash, NAME) != Name) return null;
			return new SyncStatus() { Id = ParseString(hash, ID), Status = ParseEnum<NetworkStatus>(hash, STATUS), Time = ParseFloat(hash, TIME) };
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
