using UnityEngine;
using System.Collections;


namespace Painter
{
	public class InkBallController : MonoBehaviour
	{
		public int Group { get; private set; }
		public bool IsGroup1 { get { return Group == GroupProperty.GROUP1; } }
		public bool IsGroup2 { get { return Group == GroupProperty.GROUP2; } }
		private WeaponProperty _Weapon;

		public float WeaponDamage { get { return _Weapon.Damage; } }

		public void Initialize(int group, WeaponProperty w)
		{
			Group = group;
			_Weapon = w;

			Renderer renderer = GetComponent<Renderer>();
			if (renderer == null) return;
			renderer.material.color = ConstantEnviroment.Instance.Group.GetColor(Group);
			StartCoroutine(StartingStall());
		}

		public void Initialize(SyncBall ball)
		{
			Initialize(ball.Group, ConstantEnviroment.Instance.WeaponTable.Get(ball.Type));

			transform.position = ball.Position;
			transform.rotation = ball.Rotation;
			GetComponent<Rigidbody>().velocity = ball.Velocity;
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
				controller.SetColor(ConstantEnviroment.Instance.Group.GetColor(Group));
			}
			Destroy(this.gameObject, 0.1f);
		}

		public SyncBall Send()
		{
			Rigidbody r = this.GetComponent<Rigidbody>();
			return new SyncBall() { Group = this.Group, Type = _Weapon.Name, Position = this.transform.position, Rotation = this.transform.rotation, Velocity = r.velocity };
		}
	}
}