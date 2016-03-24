using UnityEngine;
using System.Collections;


namespace Painter
{
	public class NetworkProperty
	{
		public virtual string GetAddress(int port)
		{
			return "ws://127.0.0.1:" + port + "/";
		}
	}
}