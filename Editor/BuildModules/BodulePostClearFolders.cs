using UnityEngine;
using System.IO;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// remove specific folders within build folder after build
    /// paths are relative to build folder (export path)
    /// ie: "MyGame_Data/StreamingAssets/something/"
    /// place before zip
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post clear folders", fileName = "post_clearFolders")]
    public class BodulePostClearFolders : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        [Tooltip("folder paths relative to build folder, ie: MyGame_Data/StreamingAssets/something/")]
        public string[] paths = new string[0];

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("clear folders : no profile");
                return;
            }

            string root = Path.GetFullPath(ctx.profile.BuildPath).TrimEnd('\\', '/');
            if (!Directory.Exists(root))
            {
                Debug.LogWarning("clear folders : no build folder @" + root);
                return;
            }

            int count = 0;
            foreach (string path in paths)
            {
                string dir = solvePath(root, path);
                if (dir == null) continue;

                if (!Directory.Exists(dir))
                {
                    BuildProcess.ulog("clear folders : not found @" + dir, this);
                    continue;
                }

                BuildProcess.ulog("-folder " + dir, this);
                Directory.Delete(dir, true);
                count++;
            }

            BuildProcess.ulog("clear folders : removed x" + count + " @" + root, this);
        }

        /// <summary>
        /// absolute path of relative folder
        /// null if empty or outside of build folder
        /// root : full path, no trailing separator
        /// </summary>
        static public string solvePath(string root, string relative)
        {
            if (string.IsNullOrWhiteSpace(relative)) return null;

            relative = relative.Trim().Replace('\\', '/').Trim('/');

            if (relative.Length <= 0 || Path.IsPathRooted(relative))
            {
                Debug.LogWarning("clear folders : skipped invalid path '" + relative + "'");
                return null;
            }

            string full = Path.GetFullPath(Path.Combine(root, relative));

            // must stay strictly within build folder (no ../, not root itself)
            if (!full.StartsWith(root + Path.DirectorySeparatorChar))
            {
                Debug.LogWarning("clear folders : skipped path outside build folder '" + relative + "'");
                return null;
            }

            return full;
        }

        public override string strOneLine()
        {
            return base.strOneLine() + " paths x" + paths.Length;
        }
    }
}
