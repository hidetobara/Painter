using UnityEngine;
using System.Collections;

public class MainPlayerController : MonoBehaviour
{
	public Camera MainCamera;
	private Vector3 _CameraBias = new Vector3(0, 3, -5);
	private Vector3 _Target;

	private Object _InkBallPrefab;

	void Start()
	{
		_InkBallPrefab = Resources.Load("InkBall");
	}

	void Update()
	{
		// Around
		Vector3 angle = gameObject.transform.rotation.eulerAngles;
		angle += new Vector3(0, _Around, 0);
		gameObject.transform.rotation = Quaternion.Euler(angle);
		_Around *= 0.5f;

		// Move
		Vector3 move = gameObject.transform.rotation * new Vector3(0, 0, _Velocity);
		gameObject.transform.position += move;
		_Velocity *= 0.75f;

		// Camera
		_Target = gameObject.transform.forward * 3 + gameObject.transform.position;
		Vector3 bias = gameObject.transform.rotation * _CameraBias;
		MainCamera.transform.position = transform.position + bias;
		MainCamera.transform.LookAt(_Target);
	}

	void OnGUI()
	{
		float size = 60;
		if (GUI.Button(new Rect(size, 0, size, size), "FRWRD")) ActForward();
		if (GUI.Button(new Rect(0, size, size, size), "LEFT")) ActLeft();
		if (GUI.Button(new Rect(size * 2, size, size, size), "RIGHT")) ActRight();
		if (GUI.Button(new Rect(size, size * 2, size, size), "BCK")) ActBack();
		if (GUI.Button(new Rect(size, size, size, size), "ATK")) ActAttack();
	}

	float _Velocity = 0;
	float _Around = 0;
	void ActForward()
	{
		_Velocity = 1;
	}
	void ActLeft()
	{
		_Around = -5;
	}
	void ActRight()
	{
		_Around = 5;
	}
	void ActBack()
	{
		_Velocity = -0.5f;
	}
	void ActAttack()
	{
		GameObject o = Instantiate(_InkBallPrefab) as GameObject;
		Rigidbody r = o.GetComponent<Rigidbody>();
		o.transform.position = gameObject.transform.position + gameObject.transform.forward;
		r.velocity = gameObject.transform.forward * 5;
	}
}
