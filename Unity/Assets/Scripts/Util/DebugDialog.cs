using UnityEngine;
using System.Collections;


namespace Painter
{
	public class DebugDialog : MonoBehaviour
	{
		private static DebugDialog _Instance;
		public static DebugDialog Instance
		{
			get
			{
				if (_Instance == null)
				{
					GameObject o = new GameObject("DebugDialog");
					_Instance = o.AddComponent<DebugDialog>();
				}
				return _Instance;
			}
		}

		MainPlayerController _MainPlayer;
		GUIText _Log;

		void Start()
		{
			_MainPlayer = MainPlayerController.Instance;
			_Log = gameObject.AddComponent<GUIText>();
			_Log.transform.localPosition = new Vector3(0.1f, 0.9f, 0);
			_Log.text = CalculateVertexes();
			_Log.fontSize = 16;

			Input.gyro.enabled = true;
		}

		public void Initialize() { }

		string CalculateVertexes()
		{
			var list = FindObjectsOfType<MeshFilter>();
			int count = 0;
			foreach (var filter in list) count += filter.mesh.vertexCount;
			return "VertecCount=" + count;
		}

		void Update()
		{
#if !UNITY_EDITOR
			Print("Gyro=" + Input.acceleration + System.Environment.NewLine + Log.Instance.GetInfo());
#endif
		}

		void OnGUI()
		{
			float size = 80;
			if (GUI.Button(new Rect(size, 0, size, size), "FRWRD")) _MainPlayer.ActForward();
			if (GUI.Button(new Rect(0, size, size, size), "LEFT")) _MainPlayer.ActLeft();
			if (GUI.Button(new Rect(size * 2, size, size, size), "RIGHT")) _MainPlayer.ActRight();
			if (GUI.Button(new Rect(size, size * 2, size, size), "BCK")) _MainPlayer.ActBack();
			if (GUI.Button(new Rect(size, size, size, size), "ATK")) _MainPlayer.ActAttack();
		}

		void Print(string s) { _Log.text = s; }
		float EulerAsin(float v) { return Mathf.Asin(Input.acceleration.y) * 180.0f / Mathf.PI; }
	}
}