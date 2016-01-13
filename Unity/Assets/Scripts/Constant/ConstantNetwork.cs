using UnityEngine;
using System.Collections;


namespace Painter
{
	public class NetworkProperty
	{
		public virtual string Address { get { return "ws://127.0.0.1:55555/"; } }
	}
}