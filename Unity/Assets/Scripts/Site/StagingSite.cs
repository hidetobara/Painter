using UnityEngine;
using System.Collections;


namespace Painter.Site
{
	public class StagingNetworkProperty : NetworkProperty
	{
		public override string GetAddress(int port)
		{
			return "ws://49.212.141.20:" + port + "/";
		}
	}
}