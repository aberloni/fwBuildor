using UnityEngine;

namespace fwp.logs
{

	[CreateAssetMenu(menuName = "buildor/profil/+logs", order = 100)]
	public class ProfilLogLevels : BuildModule
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
		public string[] symbolsVerbose = new string[]
		{
			"verbosity"
		};

		override public void Apply()
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

		public override string strOneLine()
		{
			return base.strOneLine() + " x" + levels.Length;
		}
	}

}