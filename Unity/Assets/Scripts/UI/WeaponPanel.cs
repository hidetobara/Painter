using UnityEngine;
using System.Collections;


namespace Painter
{
	public class WeaponPanel : MonoBehaviour
	{
		private UnityEngine.UI.Slider _Slider;
		public float Value
		{
			set { _Slider.value = value; }
			get { return _Slider.value; }
		}

		private static WeaponPanel _Instance;
		public static WeaponPanel Instance
		{
			get
			{
				if (_Instance == null) _Instance = FindObjectOfType<WeaponPanel>();
				return _Instance;
			}
		}

		void Start()
		{
			_Slider = GetComponent<UnityEngine.UI.Slider>();
		}
	}
}