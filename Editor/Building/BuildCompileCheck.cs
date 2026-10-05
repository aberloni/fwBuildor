using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;
using System.Linq;

namespace fwp.buildor.editor
{
    /// <summary>
    /// compile player scripts only (no scenes, no assets) for a target & profile symbols
    /// fast way to know if scripts will compile in build
    /// errors are logged in console, editor assemblies are not touched
    /// </summary>
    static public class BuildCompileCheck
    {
        const string path_output = "Temp/buildor_compile_check";

        const string mi_check = BuildorVerbosity._buildor_menuitem_path + "check compile";

        /// <summary>
        /// active profil, same as buildor window button
        /// </summary>
        [MenuItem(mi_check, false, 50)]
        static void miCheck() => check(BuildorVars.Profile);

        [MenuItem(mi_check, true)]
        static bool miCheckValidate() => !BuildExecutor.IsBusy && !EditorApplication.isPlayingOrWillChangePlaymode;

        /// <summary>
        /// active build target, profile symbols & debug options
        /// </summary>
        static public bool check(DataBuildSettingProfile profil)
        {
            if (profil == null)
            {
                Debug.LogError("compile check : no profil");
                return false;
            }

            return check(EditorUserBuildSettings.activeBuildTarget, profil.Symbols,
                BuildorVars.IsDebug && profil.debug != null && profil.debug.developement_build);
        }

        static public bool check(BuildTarget target, string symbols, bool development)
        {
            string[] defines = string.IsNullOrEmpty(symbols)
                ? new string[0]
                : symbols.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var settings = new ScriptCompilationSettings
            {
                target = target,
                group = BuildPipeline.GetBuildTargetGroup(target),
                options = development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None,
                extraScriptingDefines = defines,
            };

            Debug.Log("compile check : " + target + (development ? " (dev)" : "") + " symbols : " + string.Join(";", defines));

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ScriptCompilationResult result = PlayerBuildInterface.CompilePlayerScripts(settings, path_output);
            watch.Stop();

            bool ok = result.assemblies != null && result.assemblies.Count > 0;
            string duration = (watch.ElapsedMilliseconds / 1000f).ToString("F1") + "s";

            if (ok) Debug.Log("<color=green>compile check OK</color> : " + target + " x" + result.assemblies.Count + " assemblies (" + duration + ")");
            else Debug.LogError("compile check FAILED : " + target + " (" + duration + "), see errors above");

            return ok;
        }
    }
}
