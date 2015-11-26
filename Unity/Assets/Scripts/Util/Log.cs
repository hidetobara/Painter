using System;
using System.Collections.Generic;
using System.Text;

namespace Painter
{
	public class Log
	{
		private static Log _Instance = null;
		public static Log Instance
		{
			get
			{
				if (_Instance == null) _Instance = new Log();
				return _Instance;
			}
		}
		private Log() { }

		private List<string> _Info = new List<string>();
		private List<string> _Error = new List<string>();

		public void Clear() { lock (_Info) { _Info.Clear(); } }
		public void AddInfo(string s) { lock (_Info) { _Info.Add(s); } }
		public void AddError(string s) { lock (_Info) { _Info.Add(s); } lock (_Error) { _Error.Add(s); } }

		public string GetInfo() { lock (_Info) { return string.Join(Environment.NewLine, _Info.ToArray()); } }
		public string GetError() { lock (_Error) { return string.Join(Environment.NewLine, _Error.ToArray()); } }
	}
}
