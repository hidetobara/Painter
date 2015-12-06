using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Painter
{
	using JsonHash = Dictionary<string, object>;
	using JsonList = List<object>;

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

			JsonHash hash = new SyncStatus() { Status = NetworkStatus.Start, Time = 0 }.ToHash();
			socket.SendString(Json.Serialize(new JsonList() { hash }));

			while (true)
			{
				yield return null;
				var recv = socket.RecvString(); Debug.Log(recv);
				var list = Synchronized.Parse(Json.Deserialize(recv) as JsonList);
				if (list == null) continue;

				foreach (var sync in list)
				{
					SyncStatus status = sync as SyncStatus;
					if(status.Status == NetworkStatus.Start)
					{
						Synchronized.MyId = status.Id;
						_WebSocket = socket;
						Debug.Log("MyId=" + Synchronized.MyId);
						yield break;
					}
				}
			}
		}

		void LateUpdate()
		{
			if (_WebSocket == null)
			{
				_List.Clear();
				return;
			}
			// 送信
			JsonList list = new JsonList();
			foreach (var o in _List) list.Add(o.ToHash());
			string json = Json.Serialize(list);
			_WebSocket.SendString(json);
			// 受信
			json = _WebSocket.RecvString();
			if (!string.IsNullOrEmpty(json))
			{
				var syncs = Synchronized.Parse(Json.Deserialize(json) as JsonList);
				if (syncs != null) Debug.Log(syncs.Count);
			}
			// 掃除
			_List.Clear();
		}

		public void AddPlayer(GameObject o)
		{
			_List.Add(new SyncPlayer() { Position = o.transform.position, Rotation = o.transform.rotation });
		}
		public void AddBall(GameObject o)
		{
			Rigidbody r = o.GetComponent<Rigidbody>();
			_List.Add(new SyncBall() { Type = o.name, Position = o.transform.position, Rotation = o.transform.rotation, Velocity = r.velocity });
		}


	}
}