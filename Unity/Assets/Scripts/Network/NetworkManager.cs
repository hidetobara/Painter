using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
			StartCoroutine(Connecting());
		}
		IEnumerator Connecting()
		{
			var env = ConstantEnviroment.Instance;
			string uri = env.Network.GetAddress(env.Scene.GetPort(PermanentEnvironment.Instance.GameSceneName));
			Debug.Log("[Connecting] IP=" + uri + " Group=" + MyPlayerController.Instance.Group);
			var socket = new WebSocket(new Uri(uri));
			yield return StartCoroutine(socket.Connect());

			JsonHash hash = new SyncStatus() { Group = MyPlayerController.Instance.Group, Status = NetworkStatus.Join }.ToHash();
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
						Synchronized.MyGroup = status.Group;	// 自分のGroupを登録
						Synchronized.MyId = status.ShortId;	// 自分のIDを登録
						MyPlayerController.Instance.SetID(status.Group, status.ShortId);
						_StartTime = status.Time;	// 始まった時間
						_WebSocket = socket;	// ソケット登録
						Debug.Log("[Connected] Group=" + status.Group + " ID=" + Synchronized.MyId + " Time=" + _StartTime);
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
				if (o is SyncStatus) Debug.Log(o.ToString());
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

		public void AddStatus(NetworkStatus status)
		{
			_SendingQueue.Add(new SyncStatus() { Status = status });
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
				if (SynchronizeStatus(s)) continue;
			}
		}
		private bool SynchronizeBall(Synchronized s)
		{
			SyncBall ball = s as SyncBall;
			if (ball == null) return false;
			if(ball.IsMine())
			{
				return false;
			}
			PlayerController controller = PlayerController.Get(ball.Id);
			if (controller == null) return false;
			if (!controller.Alive) return false;

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
				// プレイヤーの新規作成
				GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabPlayer) as GameObject;
				controller = o.GetComponent<PlayerController>();
				controller.Register(player);
				o.name = "Player-" + player.Group + "." + player.Id;
				Debug.Log("[created] id=" + player.Id);
			}
			controller.Recieve(player);
			return true;
		}
		private bool SynchronizeStatus(Synchronized s)
		{
			SyncStatus status = s as SyncStatus;
			if (status == null || status.IsMine()) return false;

			if(status.Status == NetworkStatus.Dead)
			{
				Debug.Log("[dead] id=" + status.Id);
				PlayerController controller = PlayerController.Get(status.Id);
				if (controller == null) return false;
				controller.Alive = false;
				return true;
			}
			if(status.Status == NetworkStatus.Born)
			{
				Debug.Log("[born] id=" + status.Id);
				PlayerController controller = PlayerController.Get(status.Id);
				if (controller == null) return false;
				controller.Alive = true;
				return true;
			}
			return true;
		}
	}
}