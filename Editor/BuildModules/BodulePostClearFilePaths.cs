using UnityEngine;
using System.IO;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// remove specific files within build folder after build
    /// paths are relative to build folder (export path)
    /// file name can use * and ? to target multiple files of the same folder
    /// ie: "MyGame_Data/StreamingAssets/config.json", "MyGame_Data/StreamingAssets/logs/*.log"
    /// place before zip
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post clear file paths", fileName = "post_clearFilePaths")]
    public class BodulePostClearFilePaths : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        [Tooltip("file paths relative to build folder, wildcards allowed in file name only, ie: MyGame_Data/StreamingAssets/*.log")]
        public string[] paths = new string[0];

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("clear file paths : no profile");
                return;
            }

            string root = Path.GetFullPath(ctx.profile.BuildPath).TrimEnd('\\', '/');
            if (!Directory.Exists(root))
            {
                Debug.LogWarning("clear file paths : no build folder @" + root);
                return;
            }

            int count = 0;
            foreach (string path in paths)
            {
                count += removeFiles(root, path);
            }

            BuildProcess.ulog("clear file paths : removed x" + count + " @" + root, this);
        }

        /// <summary>
        /// remove file(s) matching relative path
        /// returns count of removed files
        /// </summary>
        int removeFiles(string root, string relative)
        {
            if (string.IsNullOrWhiteSpace(relative)) return 0;

            relative = relative.Trim().Replace('\\', '/').TrimStart('/');

            if (relative.EndsWith("/"))
            {
                Debug.LogWarning("clear file paths : skipped '" + relative + "', folder given (use BodulePostClearFolders)");
                return 0;
            }

            int sep = relative.LastIndexOf('/');
            string folder = sep < 0 ? string.Empty : relative.Substring(0, sep);
            string pattern = relative.Substring(sep + 1);

            // wildcards only in file name
            if (folder.IndexOfAny(new char[] { '*', '?' }) >= 0)
            {
                Debug.LogWarning("clear file paths : skipped '" + relative + "', wildcards not allowed in folder part");
                return 0;
            }

            // catch-all at root would remove app itself
            if (folder.Length <= 0 && pattern.Trim('*', '?', '.').Length <= 0)
            {
                Debug.LogWarning("clear file paths : skipped '" + relative + "', catch-all pattern at root");
                return 0;
            }

            string dir = folder.Length <= 0 ? root : BodulePostClearFolders.solvePath(root, folder);
            if (dir == null) return 0;

            if (!Directory.Exists(dir))
            {
                BuildProcess.ulog("clear file paths : folder not found @" + dir, this);
                return 0;
            }

            string[] files = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
            if (files.Length <= 0)
            {
                BuildProcess.ulog("clear file paths : no match '" + pattern + "' @" + dir, this);
                return 0;
            }

            foreach (string f in files)
            {
                BuildProcess.ulog("-file " + f, this);
                File.Delete(f);
            }

            return files.Length;
        }

        public override string strOneLine()
        {
            return base.strOneLine() + " paths x" + paths.Length;
        }
    }
}
