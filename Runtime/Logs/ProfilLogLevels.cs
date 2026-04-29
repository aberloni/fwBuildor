using UnityEngine;

namespace fwp.logs
{
	[CreateAssetMenu(menuName = "buildor/profil/+logs", order = 100)]
	public class ProfilLogLevels : ScriptableObject
	{
		[UnityEditor.MenuItem("Window/Logs/all script only")]
		static void menuScriptOnly()
		{
			Debug.Log("[LOGS] apply full script only");
			applyToEditor(levels_scripts_only);
		}

		static public LogLevel[] levels_scripts_only = new LogLevel[]
		{
			new LogLevel(){ type = LogType.Exception, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Assert, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Error, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Warning, stackTrace = StackTraceLogType.ScriptOnly},
			new LogLevel(){ type = LogType.Log, stackTrace = StackTraceLogType.ScriptOnly},
		};

		[System.Serializable]
		public struct LogLevel
		{
			public LogType type;
			public StackTraceLogType stackTrace;

			public string stringify() => type + ":" + stackTrace;
		}

		public LogLevel[] levels = levels_scripts_only;
		
		/// <summary>
		/// scriptable symbols linked to verbosity
		/// </summary>
		public string[] symbolsVerbose = new string[]
		{
			"verbosity"
		};

		public void apply()
		{
			applyToEditor(levels);
		}

		static void applyToEditor(LogLevel[] levels)
		{
			string output = "";
			foreach (var lvl in levels)
			{
				Application.SetStackTraceLogType(lvl.type, lvl.stackTrace);
				output += "\n" + lvl.stringify();
			}

			Debug.Log("->| logs.applied: " + output);
		}

		/// <summary>
		/// default, all stacks
		/// </summary>
		static public void resetEditor()
		{
			applyToEditor(new LogLevel[]
			{
				new LogLevel(){ type = LogType.Exception, stackTrace = StackTraceLogType.ScriptOnly},
				new LogLevel(){ type = LogType.Assert, stackTrace = StackTraceLogType.ScriptOnly},
				new LogLevel(){ type = LogType.Error, stackTrace = StackTraceLogType.ScriptOnly},
				new LogLevel(){ type = LogType.Warning, stackTrace = StackTraceLogType.ScriptOnly},
				new LogLevel(){ type = LogType.Log, stackTrace = StackTraceLogType.ScriptOnly},
			});
		}
	}

}