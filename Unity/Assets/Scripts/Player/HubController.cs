using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Painter
{
	public class HubController : MonoBehaviour
	{
		private static Dictionary<int, HubController> _Instances = new Dictionary<int, HubController>();
		public static HubController Get(int group)
		{
			if (!_Instances.ContainsKey(group)) return null;
			return _Instances[group];
		}

		const float RANGE = 2.5f;
		public int Group;

		void Awake()
		{
			if (Get(Group) != null) Debug.LogWarning("Duplicated HubController:" + Group);
			_Instances[Group] = this;
		}

		void Start()
		{
			Renderer render = GetComponent<Renderer>();
			render.material.color = ConstantEnviroment.Instance.Group.GetColor(Group);
		}

		public void Restart(PlayerController player)
		{
			// 位置をリセット
			player.transform.position = transform.position + GetRandomVector3();
			player.transform.rotation = transform.rotation;
		}

		private Vector3 GetRandomVector3()
		{
			return new Vector3(Random.Range(-RANGE, RANGE), Random.Range(-RANGE, RANGE), 0);
		}
	}
}