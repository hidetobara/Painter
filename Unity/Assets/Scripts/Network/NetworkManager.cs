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
		const int DELTA_TIME = 33;
		const int QUEUE_LIMIT = 30;

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

		List<Synchronized> _SendingQueue = new List<Synchronized>();
		Queue<Synchronized> _SynchronizedQueue = new Queue<Synchronized>();
		WebSocket _WebSocket;
		int _StartTime = 0;

		public void Initialize() { }

		void Start()
		{
			StartCoroutine(Starting());
		}
		IEnumerator Starting()
		{
			var socket = new WebSocket(new Uri(ConstantEnviroment.Instance.Network.Address));
			yield return StartCoroutine(socket.Connect());

			JsonHash hash = new SyncStatus() { Status = NetworkStatus.Joining }.ToHash();
			socket.SendString(Json.Serialize(new JsonList() { hash }));

			while (true)
			{
				yield return null;
				var recv = socket.RecvString();
				Retrieve(recv);
				while(_SynchronizedQueue.Count > 0)
				{
					Synchronized s = _SynchronizedQueue.Dequeue();
					SyncStatus status = s as SyncStatus;
					if (status == null) continue;

					if (status.Status == NetworkStatus.Accept)
					{
						// サーバーと接続できた時
						Synchronized.MyId = status.Id;	// 自分IDを登録
						MyPlayerController.Instance.SetID(status.Group, status.Id);	// 自分IDを登録
						_StartTime = status.Time;	// 始まった時間
						_WebSocket = socket;	// ソケット登録
						Debug.Log("MyId=" + Synchronized.MyId + " Time=" + _StartTime);
						yield break;
					}
				}
			}
		}

		void LateUpdate()
		{
			if (_WebSocket == null)
			{
				_SendingQueue.Clear();
				return;
			}
			// 送信
			JsonList list = new JsonList();
			foreach (var o in _SendingQueue)
			{
				list.Add(o.ToHash());
			}
			string json = Json.Serialize(list);
			_WebSocket.SendString(json);
			// 掃除
			_SendingQueue.Clear();

			// 受信
			var receiving = _WebSocket.RecvString();
			Retrieve(receiving);
			// 同期
			Synchronize();
		}

		public void AddNotify(PlayerController controller)
		{
			_SendingQueue.Add(controller.Send());
		}
		public void AddNotify(InkBallController controller)
		{
			_SendingQueue.Add(controller.Send());
		}

		private void Retrieve(string str)
		{
			if (string.IsNullOrEmpty(str)) return;

			JsonList list = new JsonList();
			var obj = Json.Deserialize(str);
			JsonHash objHash = obj as JsonHash;
			if (objHash != null)
			{
				list.Add(objHash);
			}
			else
			{
				list = obj as JsonList;
				if (list == null) return;
			}

			foreach(var o in list)
			{
				JsonHash hash = o as JsonHash;
				if (hash == null) continue;
				if (!hash.ContainsKey("TIME") || !hash.ContainsKey("DATA")) continue;

				int time = 0;
				if (!int.TryParse(hash["TIME"].ToString(), out time)) continue;

				Synchronized s = Synchronized.ParseHash(hash["DATA"] as JsonHash);
				if (s == null) continue;

				s.Time = time;	// サーバー時間を入れる
				_SynchronizedQueue.Enqueue(s);
			}

			while (_SynchronizedQueue.Count > QUEUE_LIMIT) _SynchronizedQueue.Dequeue();	// 量を
		}

		private void Synchronize()
		{
			float first = 0;
			while(_SynchronizedQueue.Count > 0)
			{
				Synchronized s = _SynchronizedQueue.Peek();
				if (first == 0) first = s.Time;
				if (first > s.Time + DELTA_TIME) break;	// 用調整

				_SynchronizedQueue.Dequeue();	// 使ったので捨てる
				if (SynchronizeBall(s)) continue;
				if (SynchronizePlayer(s)) continue;
			}
		}
		private bool SynchronizeBall(Synchronized s)
		{
			SyncBall ball = s as SyncBall;
			if (ball == null) return false;
			if(ball.IsMine())
			{
//				Debug.Log("[Ball]" + ball.Time + ball.Position);
				return false;
			}

			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			o.GetComponent<InkBallController>().Initialize(ball);
			return true;
		}
		private bool SynchronizePlayer(Synchronized s)
		{
			SyncPlayer player = s as SyncPlayer;
			if (player == null || player.IsMine()) return false;

			PlayerController controller = PlayerController.Get(player.Id);
			if(controller == null)
			{
				GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabPlayer) as GameObject;
				controller = o.GetComponent<PlayerController>();
				controller.Register(player);
				Debug.Log("Player " + player.Id + " created");
			}
			controller.Recieve(player);
			return true;
		}
	}
}