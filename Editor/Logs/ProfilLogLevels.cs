using UnityEngine;

namespace fwp.logs
{

	[CreateAssetMenu(menuName = "buildor/+profil logs", order = 100)]
	public class ProfilLogLevels : ScriptableObject
	{
		[System.Serializable]
		public struct LogLevel
		{
			public LogType type;
			public StackTraceLogType stackTrace;

			public string stringify() => type + ":" + stackTrace;
		}

		public LogLevel[] levels;

		public void apply()
		{
			foreach (var lvl in levels)
			{
				Application.SetStackTraceLogType(lvl.type, lvl.stackTrace);
				Debug.Log(lvl.stringify());
			}
		}
	}

}