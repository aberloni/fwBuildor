using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Player;

namespace fwp.buildor.editor
{
    /// <summary>
    /// compile player scripts only (no scenes, no assets) for a target
    /// with symbols already set in player settings (same as build)
    /// fast way to know if scripts will compile in build
    /// errors are logged in console, editor assemblies are not touched
    /// </summary>
    static public class BuildCompileCheck
    {
        const string path_output = "Temp/buildor_compile_check";

        const string mi_check = BuildorVerbosity._buildor_menuitem_path + "check compile";

        /// <summary>
        /// same as buildor window button
        /// </summary>
        [MenuItem(mi_check, false, 50)]
        static void miCheck() => check();

        [MenuItem(mi_check, true)]
        static bool miCheckValidate() => !BuildExecutor.IsBusy && !EditorApplication.isPlayingOrWillChangePlaymode;

        /// <summary>
        /// active build target & development build setting
        /// </summary>
        static public bool check() => check(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.development);

        static public bool check(BuildTarget target, bool development)
        {
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

            var settings = new ScriptCompilationSettings
            {
                target = target,
                group = group,
                options = development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None,
            };

            Debug.Log("compile check : " + target + (development ? " (dev)" : "")
                + " symbols : " + fwp.symbols.ScriptSymbolsView.getPlayerSetSymbols(group));

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
