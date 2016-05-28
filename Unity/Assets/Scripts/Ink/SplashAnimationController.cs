using UnityEngine;
using System.Collections;

namespace Painter
{
	public class SplashAnimationController : MonoBehaviour
	{
		private int _Group;
		private Renderer[] _Renders;

		public Vector3 Velocity = new Vector3(0, 4, 0);
		public float AliveTime = 3;
		public float ScatterRange = 3;

		void Start()
		{
			Rigidbody rigid = GetComponent<Rigidbody>();
			rigid.velocity = Velocity;
			StartCoroutine(Splashing());
		}

		public void Initialize(int group, int count = 0)
		{
			_Group = group;

			_Renders = GetComponentsInChildren<Renderer>();
			SetColor(ConstantEnviroment.Instance.Group.GetColor(group));
			SetScatter(count);
		}

		void SetColor(Color color)
		{
			print(_Renders[0].name + " " + color);
			foreach (Renderer r in _Renders) r.material.color = color;
		}

		private void SetScatter(int count)
		{
			if (count <= 1) return;

			const float Limit = 0.1f;
			for (int i = 0; i < count; i++)
			{
				Vector3 random = new Vector3(Random.Range(-Limit, Limit), 0, Random.Range(-Limit, Limit));
				GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabSubInkBall) as GameObject;
				o.transform.position = this.transform.position + random;
				InkBallController controller = o.GetComponent<InkBallController>();
				controller.Initialize(_Group, new WeaponSplashProperty());
			}
		}

		IEnumerator Splashing()
		{
			for(float time = 0; time < AliveTime; time += Time.deltaTime)
			{
				float theta = time / AliveTime;
				float value = theta / (0.1f + theta) * ScatterRange;
				transform.localScale = new Vector3(value, 1, value);
				yield return null;
			}
			Destroy(this.gameObject);
		}
	}
}