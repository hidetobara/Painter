using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Painter
{
	using Hash = Dictionary<string, object>;

	public class NetworkManager : MonoBehaviour
	{
		private static NetworkManager _Instance;
		public static NetworkManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					GameObject o = new GameObject("NetworkManager");
					_Instance = o.AddComponent<NetworkManager>();
				}
				return _Instance;
			}
		}

		List<Synchronized> _List = new List<Synchronized>();
		WebSocket _WebSocket;

		public void Initialize() { }

		void Start()
		{
			StartCoroutine(Starting());
		}
		IEnumerator Starting()
		{
			var socket = new WebSocket(new Uri(ConstantEnviroment.Instance.Network.Address));
			yield return StartCoroutine(socket.Connect());

			Hash hash = new SyncStatus() { Status = "Connect", Time = 0 }.ToHash();
			socket.SendString(Json.Serialize(hash));
			yield return null;

			_WebSocket = socket;
		}

		void LateUpdate()
		{
			if (_WebSocket == null)
			{
				_List.Clear();
				return;
			}

			List<object> list = new List<object>();
			foreach (var o in _List) list.Add(o.ToHash());
			string json = Json.Serialize(list);
			_WebSocket.SendString(json);
			Debug.Log(json);
			json = _WebSocket.RecvString();
			Debug.Log(json);

			_List.Clear();
		}

		public void AddPlayer(GameObject o)
		{
			_List.Add(new SyncPlayer() { ID = o.name, Position = o.transform.position, Rotation = o.transform.rotation });
		}
		public void AddBall(GameObject o)
		{
			Rigidbody r = o.GetComponent<Rigidbody>();
			_List.Add(new SyncBall() { Type = o.name, Position = o.transform.position, Rotation = o.transform.rotation, Velocity = r.velocity });
		}

		public class Synchronized
		{
			public virtual string Name { get; protected set; }
			public Vector3 Position;
			public Quaternion Rotation;

			public virtual Hash ToHash()
			{
				Hash hash = new Hash();
				hash["nam"] = Name;
				hash["pos"] = VectorToString(Position);
				hash["rot"] = QuaternionToString(Rotation);
				return hash;
			}
			protected string VectorToString(Vector3 v)
			{
				return string.Format("{0:f2}|{1:f2}|{2:f2}", v.x, v.y, v.z);
			}
			protected string QuaternionToString(Quaternion q)
			{
				return string.Format("{0:f2}|{1:f2}|{2:f2}|{3:f2}", q.x, q.y, q.z, q.w);
			}
		}

		public class SyncStatus : Synchronized
		{
			public override string Name { get { return "sta"; } }
			public string Status;
			public float Time;
			public override Hash ToHash()
			{
				Hash hash = base.ToHash();
				hash["sta"] = Status;
				hash["tim"] = Time;
				return hash;
			}
		}

		public class SyncPlayer : Synchronized
		{
			public override string Name { get { return "pla"; } }
			public string ID;
			public override Hash ToHash()
			{
				Hash hash = base.ToHash();
				hash["id"] = ID;
				return hash;
			}
		}

		public class SyncBall : Synchronized
		{
			public override string Name { get { return "bal"; } }
			public string Type;
			public Vector3 Velocity;
			public override Hash ToHash()
			{
				Hash hash = base.ToHash();
				hash["typ"] = Type;
				hash["vol"] = VectorToString(Velocity);
				return hash;
			}
		}
	}
}