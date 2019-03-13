using System.Diagnostics;

namespace UnofficialMultiplayerAPI_UTILS
{
	static internal class Log
	{
	#if HOST
		const string prefix = "UMPAPI[Host]";
	#else
		const string prefix = "UMPAPI[Client]";
	#endif


		[Conditional("DEBUG")]
		public static void Debug(string msg)
		{
			Verse.Log.Message($"{prefix}: {msg}", true);
		}

		public static void Message(string msg, bool ignoreStopLoggingLimit = false)
		{
			Verse.Log.Message($"{prefix}: {msg}", ignoreStopLoggingLimit);
		}

		public static void Warning(string msg, bool ignoreStopLoggingLimit = false)
		{
			Verse.Log.Warning($"{prefix}: {msg}", ignoreStopLoggingLimit);
		}

		public static void Error(string msg, bool ignoreStopLoggingLimit = false)
		{
			Verse.Log.Error($"{prefix}: {msg}", ignoreStopLoggingLimit);
		}
	}
}
