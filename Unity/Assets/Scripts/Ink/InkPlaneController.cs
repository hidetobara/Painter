using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Painter
{
	public class InkPlaneController : MonoBehaviour
	{
		Mesh _Mesh;
		List<Vector4> _Inks = new List<Vector4>();
		Vector3[] _Worlds;
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

			Renderer render = GetComponent<Renderer>();
			render.material.SetColor("_Color1", ConstantEnviroment.Instance.Color1);
			render.material.SetColor("_Color2", ConstantEnviroment.Instance.Color2);
		}

		void OnCollisionStay(Collision collision)
		{
			List<int> group1 = new List<int>();
			List<int> group2 = new List<int>();
			foreach (var contact in collision.contacts)
			{
				var ball = contact.otherCollider.GetComponent<InkBallController>();
				if (ball == null) continue;

				for (int v = 0; v < _Length; v++)
				{
					if (IsNear(_Worlds[v], contact.point))
					{
						if (ball.IsGroup1) group1.Add(v);
						if (ball.IsGroup2) group2.Add(v);
					}
				}
			}

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
					z += 0.2f;
					if (z > 2) z = 2f;
					if (w > 1) w = 1f;
				}
				if(group == GroupProperty.GROUP2)
				{
					w += 0.2f;
					if (w > 2f) w = 2f;
					if (z > 1f) z = 1f;
				}
				_Inks[index] = new Vector4(v.x, v.y, z, w);
			}
		}

		bool IsNear(Vector3 a, Vector3 b, float distance = 1.1f)
		{
			return Vector3.Distance(a, b) < distance;
		}

		public int CalclateGroup(Vector3 position, float distance = 3.0f)
		{
			int group = 0;
			for(int i = 0; i < _Length; i++)
			{
				float d = Vector3.Distance(position, _Worlds[i]);
				if(distance > d)
				{
					distance = d;
					float g1 = _Inks[i].z;
					float g2 = _Inks[i].w;
					if (g1 < 0.3f && g2 < 0.3f) continue;
					if (g1 > g2) group = 1; else group = 2;
				}
			}
			return group;
		}

		System.Random _Random = new System.Random();
		float RandomFloat(float low, float high)
		{
			return (float)(_Random.NextDouble() * (high - low) + low);
		}
	}
}