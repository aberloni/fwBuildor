using UnityEngine;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// open build folder in explorer/finder after build
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post open folder", fileName = "post_openFolder")]
    public class BodulePostOpenFolder : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        protected override void doApply(BuildContext ctx)
        {
            // manual apply : no summary, use profile path
            string path = ctx.summary.HasValue ? ctx.summary.Value.outputPath : ctx.profile?.FullPath;

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("open folder : no path");
                return;
            }

            BuildProcess.ulog("+OPEN FOLDER of build : " + path, this);
            BuildPostprocess.openBuildFolder(path);
        }
    }
}
