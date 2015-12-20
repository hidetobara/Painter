using UnityEngine;
using System.Collections;


namespace Painter.Site
{
	public class StagingNetworkProperty : NetworkProperty
	{
		public override string Address { get { return "ws://49.212.141.20:55555/"; } }
	}
}