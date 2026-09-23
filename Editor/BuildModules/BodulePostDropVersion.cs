using UnityEngine;
using System.IO;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// write a text file with version number(s) in build folder after build
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post drop version", fileName = "post_dropVersion")]
    public class BodulePostDropVersion : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        public string fileName = "version.txt";

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("drop version : no profile");
                return;
            }

            string p = Path.Combine(ctx.profile.BuildPath, fileName);
            BuildProcess.ulog("+VERSION DROP @" + p, this);
            File.WriteAllText(p, ctx.profile.VersionFull);
        }
    }
}
