using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace fwp.buildor.editor
{
    [System.Serializable]
    public class SpecificPathOwner
    {
        static string GetSessionUserName()
        {
            // Cross-platform: works on Windows, Linux, macOS
            string user = Environment.UserName;

            if (string.IsNullOrEmpty(user))
                user = Environment.GetEnvironmentVariable("USER")      // Linux/macOS
                    ?? Environment.GetEnvironmentVariable("USERNAME"); // Windows

            user = user ?? "unknown";

            string machine = Environment.MachineName;

            return machine + "/" + user;
        }

        [SerializeField] string owner;
        [SerializeField] SpecificPaths paths;

        public SpecificPathOwner()
        {
            owner = GetSessionUserName();
            paths = new();
        }

        public bool Is() => owner == GetSessionUserName();

        public SpecificPaths Get() => paths;

        public override string ToString()
        {
            return "[specpath]" + owner;
        }
    }

    /// <summary>
    /// per profil, can specify fixed path
    /// </summary>
    [System.Serializable]
    public class SpecificPaths
    {
        [SerializeField] string[] paths;

        public string ActivePath => Get(BuildorVars.TargetDebug);

        public SpecificPaths()
        {
            /*
            paths = new string[System.Enum.GetValues(typeof(TargetDebug)).Length];
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = string.Empty;
            }*/

            paths = Enumerable.Repeat(string.Empty, System.Enum.GetValues(typeof(TargetDebug)).Length).ToArray();
        }

        public bool match(TargetDebug t, string p)
        {
            return Get(t) == p;
        }

        public void Set(TargetDebug t, string p)
        {
            paths[(int)t] = p;
        }

        public string Get(TargetDebug t)
        {
            return paths[(int)t];
        }

        public void clear(TargetDebug t)
        {
            paths[(int)t] = string.Empty;
        }
    }

}