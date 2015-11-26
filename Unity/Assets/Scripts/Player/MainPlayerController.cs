﻿using UnityEngine;
using System.Collections;


namespace Painter
{
	public class MainPlayerController : MonoBehaviour
	{
		public Camera MainCamera;
		private Vector3 _CameraBias = new Vector3(0, 3, -5);
		private Vector3 _Target;
		// 定数
		private PlayerProperty _Player;
		private WeaponProperty _Weapon;

		void Start()
		{
			ConstantEnviroment.Instance.Initialize();
			_Player = ConstantEnviroment.Instance.FriendPlayer;
			_Weapon = ConstantEnviroment.Instance.MyWeapon;
		}

		void Update()
		{
			// Around
			Vector3 angle = gameObject.transform.rotation.eulerAngles;
			angle += new Vector3(0, _Around, 0);
			gameObject.transform.rotation = Quaternion.Euler(angle);
			_Around *= 0.75f;

			// Move
			Vector3 move = gameObject.transform.rotation * new Vector3(0, 0, _Velocity);
			gameObject.transform.position += move;
			_Velocity *= 0.9f;

			// Camera
			_Target = gameObject.transform.forward * 3 + gameObject.transform.position;
			Vector3 bias = gameObject.transform.rotation * _CameraBias;
			MainCamera.transform.position = transform.position + bias;
			MainCamera.transform.LookAt(_Target);
		}

		void OnGUI()
		{
			float size = 80;
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
			_Velocity = 0.5f;
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
			_Velocity = -0.25f;
		}
		void ActAttack()
		{
			GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabInkBall) as GameObject;
			Rigidbody r = o.GetComponent<Rigidbody>();
			o.transform.position = gameObject.transform.position + gameObject.transform.forward * 2;
			o.transform.rotation = gameObject.transform.rotation;
			r.velocity = gameObject.transform.forward * _Weapon.Velocity;

			InkBallController controller = o.GetComponent<InkBallController>();
			controller.Initialize(_Player, _Weapon);
		}
	}
}