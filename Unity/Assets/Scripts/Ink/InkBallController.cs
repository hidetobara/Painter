using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InkBallController : MonoBehaviour
	{
		private PlayerProperty _Player;
		private WeaponProperty _Weapon;

		public void Initialize(PlayerProperty p, WeaponProperty w)
		{
			_Player = p;
			_Weapon = w;

			Renderer renderer = GetComponent<Renderer>();
			if (renderer == null) return;
			renderer.material.color = p.MainColor;

			StartCoroutine(StartingStall());
		}

		private IEnumerator StartingStall()
		{
			float passed = 0;
			while (true)
			{
				if(passed > _Weapon.StallTime)
				{
					Rigidbody r = GetComponent<Rigidbody>();
					r.useGravity = true;
				}
				if(passed > _Weapon.BrokenTime)
				{
					Destroy(this.gameObject);
					break;
				}

				yield return null;
				passed += Time.deltaTime;
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			foreach (var contact in collision.contacts)
			{
				GameObject go = Instantiate(ConstantEnviroment.Instance.PrefabInkScatter) as GameObject;
				go.transform.position = contact.point;
				var controller = go.GetComponent<InkScatterController>();
				controller.SetColor(_Player.MainColor);
			}
			Destroy(this.gameObject, 0.1f);
		}

		public SyncBall Send()
		{
			Rigidbody r = this.GetComponent<Rigidbody>();
			return new SyncBall() { Type = this.name, Position = this.transform.position, Rotation = this.transform.rotation, Velocity = r.velocity };
		}
	}
}