using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// write a text file with version number(s) in build folder after build
    /// + "dev build" line if dev build, "#debug" line if debug symbol is set
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post drop version", fileName = "post_dropVersion")]
    public class BodulePostDropVersion : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        public string fileName = "version.txt";

        const string symbolDebug = "debug";

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("drop version : no profile");
                return;
            }

            string content = ctx.profile.VersionFull;
            if (isDevBuild(ctx)) content = addLine(content, "dev build");
            if (hasDebugSymbol(ctx)) content = addLine(content, "#" + symbolDebug);

            string p = Path.Combine(ctx.profile.BuildPath, fileName);
            BuildProcess.ulog("+VERSION DROP @" + p, this);
            File.WriteAllText(p, content);
        }

        static string addLine(string content, string line) => string.IsNullOrEmpty(content) ? line : content + "\n" + line;

        /// <summary>
        /// post : options of build summary
        /// manual : editor build settings (set by profile, debug level)
        /// </summary>
        static bool isDevBuild(BuildContext ctx)
        {
            if (ctx.summary.HasValue) return (ctx.summary.Value.options & BuildOptions.Development) != 0;
            return EditorUserBuildSettings.development;
        }

        /// <summary>
        /// player settings symbols of built target contain debug (#if debug)
        /// </summary>
        static bool hasDebugSymbol(BuildContext ctx)
        {
            var group = BuildPipeline.GetBuildTargetGroup(ctx.Target);
            string symbols = fwp.symbols.ScriptSymbolsView.getPlayerSetSymbols(group);
            return symbols.Split(';').Any(s => s.Trim() == symbolDebug);
        }
    }
}
