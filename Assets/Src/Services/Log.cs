using System.Diagnostics;

namespace Src.Services
{
	public class GLog
	{
		[Conditional("DEBUG")]
		public static void Debug(string s)
		{
			UnityEngine.Debug.Log(s);
		}
	}
}