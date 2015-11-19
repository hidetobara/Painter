using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour {

//	string URI = "ws://49.212.141.20:8080/";
	string URI = "ws://127.0.0.1:12345/";

	// Use this for initialization
	IEnumerator Start () {
		WebSocket w = new WebSocket(new Uri(URI));
		yield return StartCoroutine(w.Connect());
		w.SendString("Hi there");
		int i=0;
		while (true)
		{
			string reply = w.RecvString();
			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				w.SendString("Hi there"+i++);
				if (i > 3) break;
			}
			if (w.Error != null)
			{
				Debug.LogError ("Error: "+w.Error);
				break;
			}
			yield return 0;
		}
		w.Close();
	}
}
