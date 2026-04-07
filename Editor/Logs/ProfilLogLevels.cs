using UnityEngine;

namespace fwp.logs
{

	[CreateAssetMenu(menuName = "buildor/profil/+logs", order = 100)]
	public class ProfilLogLevels : ScriptableObject
	{
		[System.Serializable]
		public struct LogLevel
		{
			public LogType type;
			public StackTraceLogType stackTrace;

			public string stringify() => type + ":" + stackTrace;
		}

		public LogLevel[] levels = new LogLevel[]
		{
			new LogLevel(){ type = LogType.Exception, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Assert, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Error, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Warning, stackTrace = StackTraceLogType.None},
			new LogLevel(){ type = LogType.Log, stackTrace = StackTraceLogType.None},
		};

		/// <summary>
		/// scriptable symbols linked to verbosity
		/// </summary>
		[SerializeField]
		string[] symbolsVerbose = new string[]
		{
			"verbosity"
		};

		public string stringifySymbols()
		{
			string ret = string.Empty;
			foreach (var s in symbolsVerbose)
			{
				ret += s + ",";
			}
			return ret;
		}

		public void applyLogs()
		{
			foreach (var lvl in levels)
			{
				Application.SetStackTraceLogType(lvl.type, lvl.stackTrace);
				Debug.Log(lvl.stringify());
			}
		}
	}

}