using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace fwp.buildor.editor
{
    /// <summary>
    /// git calls, executed in a given folder (git resolves the repo containing it)
    /// git must be in PATH
    /// </summary>
    static public class HelperGit
    {
        /// <summary>
        /// git add -A . (folder content only), then git commit -m [message]
        /// no commit if add failed
        /// </summary>
        static public bool commitAll(string folder, string message)
        {
            if (!run(folder, "add -A .")) return false;
            return run(folder, "commit -m \"" + message.Replace("\"", "\\\"") + "\"");
        }

        /// <summary>
        /// output logged, error log on failure
        /// false : git not found or non zero exit code
        /// </summary>
        static public bool run(string folder, string args)
        {
            var psi = new ProcessStartInfo("git", args)
            {
                WorkingDirectory = folder,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };

            try
            {
                using (var p = Process.Start(psi))
                {
                    // async stderr : avoid deadlock when both streams fill up
                    var err = p.StandardError.ReadToEndAsync();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    // git writes warnings (ie: CRLF) to stderr, not errors
                    string log = "git " + args + " @" + folder + "\n" + output + err.Result;

                    if (p.ExitCode != 0)
                    {
                        Debug.LogError(log + "\nexit code " + p.ExitCode);
                        return false;
                    }

                    Debug.Log(log);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("git " + args + " @" + folder + " : " + e.Message);
                return false;
            }
        }
    }
}
