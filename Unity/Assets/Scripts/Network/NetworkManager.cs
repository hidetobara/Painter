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

		float _ConnectedTime = 0;
		float _BiasTime = 0;
		float GetTime() { return Time.unscaledTime - _ConnectedTime + _BiasTime; }

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
				var recv = socket.RecvString();
				var list = Synchronized.Parse(Json.Deserialize(recv) as JsonList);
				if (list == null) continue;

				foreach (var sync in list)
				{
					SyncStatus status = sync as SyncStatus;
					if(status.Status == NetworkStatus.Start)
					{
						// サーバーと接続できた時
						Synchronized.MyId = status.Id;	// 自分IDを登録
						MyPlayerController.Instance.SetID(status.Id);	// 自分IDを登録
						_BiasTime = status.Time;	// 時間のずれを調整
						_ConnectedTime = Time.unscaledTime;	// 通信始まった時間を登録
						_WebSocket = socket;	// ソケット登録
						Debug.Log("MyId=" + Synchronized.MyId + " Time=" + _BiasTime);
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
			foreach (var o in _List)
			{
				o.Time = GetTime();
				list.Add(o.ToHash());
			}
			string json = Json.Serialize(list);
			_WebSocket.SendString(json);
			// 受信
			json = _WebSocket.RecvString();
			if (!string.IsNullOrEmpty(json))
			{
				var syncs = Synchronized.Parse(Json.Deserialize(json) as JsonList);
				if (syncs != null && syncs.Count > 0) Retrieve(syncs);
			}
			// 掃除
			_List.Clear();
		}

		public void AddNotify(PlayerController controller)
		{
			_List.Add(controller.Send());
		}
		public void AddNotify(InkBallController controller)
		{
			_List.Add(controller.Send());
		}

		private void Retrieve(List<Synchronized> list)
		{
			foreach(var s in list)
			{
				if (RetrieveBall(s)) continue;
				if (RetrievePlayer(s)) continue;
			}
		}
		private bool RetrieveBall(Synchronized s)
		{
			SyncBall ball = s as SyncBall;
			if (ball == null) return false;
			if(ball.IsMine())
			{
				Debug.Log("[Ball]" + ball.Time);
				return false;
			}

			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			o.transform.position = ball.Position;
			o.transform.rotation = ball.Rotation;
			o.GetComponent<Rigidbody>().velocity = ball.Velocity;
			return true;
		}
		private bool RetrievePlayer(Synchronized s)
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