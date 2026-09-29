using UnityEngine;
using System.IO;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// remove specific files at root of build folder after build
    /// ie: "steam_appid.txt"
    /// place before zip
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post clear files", fileName = "post_clearFiles")]
    public class BodulePostClearFiles : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        [Tooltip("file names (with extension) at root of build folder, ie: steam_appid.txt")]
        public string[] files = new string[0];

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("clear files : no profile");
                return;
            }

            int count = 0;
            foreach (string file in files)
            {
                if (removeRootFile(ctx.profile.BuildPath, file, this)) count++;
            }

            BuildProcess.ulog("clear files : removed x" + count + " @" + ctx.profile.BuildPath, this);
        }

        /// <summary>
        /// remove a file at root of build folder, if it exists
        /// true if removed
        /// </summary>
        static public bool removeRootFile(string root, string fileName, Object context = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            fileName = fileName.Trim();

            // root level only : no sub path
            if (fileName.IndexOfAny(new char[] { '/', '\\' }) >= 0 || Path.IsPathRooted(fileName) || fileName == ".." || fileName == ".")
            {
                Debug.LogWarning("clear files : skipped invalid file name '" + fileName + "' (root level only)");
                return false;
            }

            string path = Path.Combine(root, fileName);
            if (!File.Exists(path))
            {
                BuildProcess.ulog("clear files : not found @" + path, context);
                return false;
            }

            BuildProcess.ulog("-file " + path, context);
            File.Delete(path);
            return true;
        }

        public override string strOneLine()
        {
            return base.strOneLine() + " files x" + files.Length;
        }
    }
}
