using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fwp.version
{
    using fwp.buildor.editor;

    public class VersionManagerProfil
    {
#if UNITY_EDITOR
        [MenuItem("Version/log profil internal version")]
        static public void miLogCurrProfilWork()
        {
            var profil = BuildHelperBase.getActiveProfile(buildor.version.PublishLevel.intern);
            VersionManager.logXYZVersion(profil.versionInternal);
        }

        [MenuItem("Version/log profil publish version")]
        static public void miLogCurrProfilPublish()
        {
            var profil = BuildHelperBase.getActiveProfile(buildor.version.PublishLevel.release);
            VersionManager.logXYZVersion(profil.versionPublish);
        }
    }
#endif
}

