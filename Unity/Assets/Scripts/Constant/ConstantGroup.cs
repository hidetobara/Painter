using System;
using System.Collections;
using UnityEngine;

namespace Painter
{
	public class GroupProperty
	{
		public Color GetColor(int group)
		{
			switch(group)
			{
				case 0:
					return Color.red;
				case 1:
					return new Color(0.9f, 0.5f, 0.9f);
				case 2:
					return new Color(0.5f, 0.9f, 0.9f);
				default:
					return new Color(0.9f, 0.9f, 0.5f);
			}
		}
	}
}