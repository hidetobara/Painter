﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Painter
{
	public class InkPlaneController : MonoBehaviour
	{
		const float NEAR_DISTANCE = 1.1f;

		public int AreaLimist;	

		Mesh _Mesh;
		List<Vector4> _Inks = new List<Vector4>();
		Vector3[] _Worlds;
		MeshTable _MeshTable;
		private int _Length { get { return _Worlds.Length; } }

		// Use this for initialization
		void Start()
		{
			_Mesh = GetComponent<MeshFilter>().mesh;
			int length = _Mesh.uv.Length;
			for (int l = 0; l < length; l++) _Inks.Add(new Vector4(RandomFloat(0, 1), RandomFloat(0, 1), 0, 0));
			_Mesh.SetUVs(1, _Inks);

			_Worlds = new Vector3[_Mesh.vertices.Length];
			for (int v = 0; v < _Mesh.vertices.Length; v++) _Worlds[v] = transform.TransformPoint(_Mesh.vertices[v]);
			_MeshTable = new MeshTable(AreaLimist, _Worlds);

			Renderer render = GetComponent<Renderer>();
			foreach (Material m in render.materials)
			{
				m.SetColor("_Color1", ConstantEnviroment.Instance.Color1);
				m.SetColor("_Color2", ConstantEnviroment.Instance.Color2);
			}
			Debug.Log("name=" + name + " count=" + length);
		}

		void OnCollisionStay(Collision collision)
		{
			List<int> group1 = new List<int>();
			List<int> group2 = new List<int>();
			foreach (var contact in collision.contacts)
			{
				var ball = contact.otherCollider.GetComponent<InkBallController>();
				if (ball != null)
				{
					List<int> contacts = _MeshTable.Contacts(contact.point, NEAR_DISTANCE);
					if (contacts.Count > 0)
					{
						if (ball.IsGroup1) group1.AddRange(contacts);
						if (ball.IsGroup2) group2.AddRange(contacts);
					}
				}
			}
			if (group1.Count == 0 && group2.Count == 0) return;

			UpdateInk(group1.Distinct().ToList(), GroupProperty.GROUP1);
			UpdateInk(group2.Distinct().ToList(), GroupProperty.GROUP2);
			_Mesh.SetUVs(1, _Inks);
		}
		private void UpdateInk(List<int> indexes, int group)
		{
			foreach (int index in indexes)
			{
				Vector4 v = _Inks[index];
				float z = v.z;
				float w = v.w;
				if (group == GroupProperty.GROUP1)
				{
					z += 0.3f;
					if (z > 1.1) z = 1.1f;
					if (w > 1) w = 1f;
				}
				if(group == GroupProperty.GROUP2)
				{
					w += 0.3f;
					if (w > 1.1f) w = 1.1f;
					if (z > 1f) z = 1f;
				}
				_Inks[index] = new Vector4(v.x, v.y, z, w);
			}
		}

		IEnumerator _Calclating = null;
		public void CalclateGroup(Vector3 position, Action<int> onGrouped)
		{
			if(_Calclating != null) return;
			_Calclating = _MeshTable.NearestGroupAsync(position, Group, onGrouped);
			StartCoroutine(_Calclating);
		}
		private int Group(int index)
		{
			_Calclating = null;
			if (index < 0) return 0;

			float g1 = _Inks[index].z;
			float g2 = _Inks[index].w;
			if (g1 < 0.3f && g2 < 0.3f) return 0;
			if (g1 > g2) return 1; return 2;
		}

		System.Random _Random = new System.Random();
		float RandomFloat(float low, float high)
		{
			return (float)(_Random.NextDouble() * (high - low) + low);
		}
	}
}