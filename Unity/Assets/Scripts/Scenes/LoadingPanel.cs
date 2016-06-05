using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Painter
{
	public class LoadingPanel : MonoBehaviour
	{
		private static LoadingPanel _Instance;
		public static LoadingPanel Instance
		{
			get { if (_Instance == null) _Instance = FindObjectOfType<LoadingPanel>(); return _Instance; }
		}

		public GameObject LoadingImage;
		private int _Degree = 0;

		void Start()
		{
			Instance.Off();
		}

		public void SetProgress(float value)
		{
			var image = GetComponent<Image>();
			if (image == null) return;
			Debug.Log(value);
			image.color = new Color(0, 0, 0, value / 2);
		}

		public void On()
		{
			gameObject.SetActive(true);
		}

		public void Off()
		{
			gameObject.SetActive(false);
		}

		private void Update()
		{
			_Degree += 2;
			if (_Degree >= 360) _Degree = 0;

			int index = _Degree / 30;
			LoadingImage.transform.localEulerAngles = new Vector3(0, 0, index * 30);
		}
	}
}