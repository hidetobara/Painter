using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class InkPlaneController : MonoBehaviour
{
	Mesh _Mesh;
	List<Vector4> _Inks = new List<Vector4>();
	Vector3[] _Worlds;

	// Use this for initialization
	void Start()
	{
		_Mesh = GetComponent<MeshFilter>().mesh;
		int length = _Mesh.uv.Length;
		for (int l = 0; l < length; l++) _Inks.Add(new Vector4(RandomFloat(0, 1), RandomFloat(0, 1), 0, 0));
		_Mesh.SetUVs(1, _Inks);

		_Worlds = new Vector3[_Mesh.vertices.Length];
		for (int v = 0; v < _Mesh.vertices.Length; v++) _Worlds[v] = transform.TransformPoint(_Mesh.vertices[v]);
	}

	void OnCollisionStay(Collision collision)
	{
		List<int> indexes = new List<int>();
		foreach(var contact in collision.contacts)
		{
			var ball = contact.otherCollider.GetComponent<InkBallController>();
			if (ball == null) continue;

			for(int v = 0; v < _Worlds.Length; v++)
			{
				if (IsNear(_Worlds[v], contact.point)) indexes.Add(v);
			}
		}
		indexes = indexes.Distinct().ToList();
		foreach (int index in indexes)
		{
			Vector4 v = _Inks[index];
			float z = v.z + 0.25f;
			if (z > 1) z = 1;
			_Inks[index] = new Vector4(v.x, v.y, z, v.w);
		}
		_Mesh.SetUVs(1, _Inks);
	}

	bool IsNear(Vector3 a, Vector3 b, float distance = 1.1f)
	{
		return Vector3.Distance(a, b) < distance;
	}

	System.Random _Random = new System.Random();
	float RandomFloat(float low, float high)
	{
		return (float)(_Random.NextDouble() * (high - low) + low);
	}
}
