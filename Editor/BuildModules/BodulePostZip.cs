using UnityEngine;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// zip build folder next to it after build
    /// place after any bodule that write into build folder
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post zip", fileName = "post_zip")]
    public class BodulePostZip : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("zip : no profile");
                return;
            }

            BuildProcess.ulog("+ZIP " + ctx.profile.ZipFullPath, this);
            BuildPostprocess.zipBuildFolder(ctx.profile.BuildPath, ctx.profile.ZipFullPath);
        }
    }
}
