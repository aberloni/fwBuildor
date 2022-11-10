using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fwp.buildor.version
{
    using fwp.buildor.editor;

    public class VersionManagerProfil
    {
#if UNITY_EDITOR
        static public string getProfilWorkVersion() => BuildHelperBase.getActiveProfile().internalVersion.version;
        static public string getProfilWorkBuildNumber() => BuildHelperBase.getActiveProfile().internalVersion.buildNumber.ToString();

        [MenuItem("Build/Version/log profil internal version")]
        static public void miLogCurrProfilWork()
        {
            var profil = BuildHelperBase.getActiveProfile();
            VersionManager.logXYZVersion(profil.internalVersion);
        }

        [MenuItem("Build/Version/log profil publish version")]
        static public void miLogCurrProfilPublish()
        {
            var profil = BuildHelperBase.getActiveProfile();
            VersionManager.logXYZVersion(profil.publishVersion);
        }
    }
#endif
}

