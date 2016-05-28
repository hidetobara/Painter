using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InkScatterController : MonoBehaviour
	{
		private int _Group;

		public void Initialize(int group, int count = 0)
		{
			_Group = group;

			SetColor(ConstantEnviroment.Instance.Group.GetColor(group));
			SetScatter(count);
		}

		private void SetColor(Color color)
		{
			var particle = GetComponent<ParticleSystem>();
			if (particle == null) return;
			particle.startColor = color;
		}

		private void SetScatter(int count)
		{
			if (count <= 1) return;

			const float Limit = 0.1f;
			for(int i = 0; i < count; i++)
			{
				Vector3 random = new Vector3(Random.Range(-Limit, Limit), 0, Random.Range(-Limit, Limit));
				GameObject o = Instantiate(ConstantEnviroment.Instance.PrefabSubInkBall) as GameObject;
				o.transform.position = this.transform.position + random;
				InkBallController controller = o.GetComponent<InkBallController>();
				controller.Initialize(_Group, new WeaponSplashProperty());
			}
		}

		void Start()
		{
			Destroy(this.gameObject, 1.1f);
		}
	}
}