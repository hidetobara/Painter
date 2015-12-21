using System;
using System.Collections;
using UnityEngine;

namespace Painter
{
	public class GroupProperty
	{
		public const int GROUP1 = 1;
		public const int GROUP2 = 2;

		public Color GetColor(int group)
		{
			switch(group)
			{
				case 0:
					return Color.white;	// どこに属さない
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