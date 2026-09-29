using UnityEngine;
using fwp.buildor.editor;

namespace fwp.buildor
{
    /// <summary>
    /// steam specific post build
    /// remove steam_appid.txt from build folder (dev only file, must not be shipped)
    /// place before zip
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+post steam", fileName = "post_steam")]
    public class BoduleSteam : BuildModule
    {
        public override BodulePhase Phase => BodulePhase.post;

        const string file_appid = "steam_appid.txt";

        protected override void doApply(BuildContext ctx)
        {
            if (ctx.profile == null)
            {
                Debug.LogWarning("steam : no profile");
                return;
            }

            BodulePostClearFiles.removeRootFile(ctx.profile.BuildPath, file_appid, this);
        }
    }
}
