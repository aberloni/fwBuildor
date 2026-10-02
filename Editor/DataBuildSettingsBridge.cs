using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor;

namespace fwp.buildor.editor
{
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "+bridge", order = BuildorHelpers.menu_order)]
    public class DataBuildSettingsBridge : ScriptableObject
    {

        [Header("desktop")]
        public DataBuildSettingProfile[] windows;
        public DataBuildSettingProfile[] linux;
        public DataBuildSettingProfile[] osx;

        [Header("mobile")]
        public DataBuildSettingProfile[] android;
        public DataBuildSettingProfile[] ios;

        [Header("console")]
        public DataBuildSettingProfile[] ninSwitch;

        public DataBuildSettingProfile getPlatformProfil(TargetPublish tarState, TargetSdks sdk)
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            var profils = getPlatformProfils(target);
            if (profils == null)
            {
                Debug.LogWarning(" ? no profil list for active build target : " + target);
                return null;
            }

            var ret = profils.FirstOrDefault(x => x != null && x.Is(tarState, sdk));
            if (ret == null) Debug.LogWarning("no profil possible for " + tarState + " & " + sdk);
            return ret;
        }

        /// <summary>
        /// profil list of a platform
        /// null : platform not supported
        /// </summary>
        public DataBuildSettingProfile[] getPlatformProfils(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows: return windows;
                case BuildTarget.StandaloneOSX: return osx;
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.EmbeddedLinux: return linux;
                case BuildTarget.iOS: return ios;
                case BuildTarget.Android: return android;
                case BuildTarget.Switch: return ninSwitch;
            }
            return null;
        }

        /// <summary>
        /// profil scriptable type of a platform
        /// null : platform not supported
        /// </summary>
        static public System.Type getProfilType(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows: return typeof(DataBuildSettingProfileWindows);
                case BuildTarget.StandaloneOSX: return typeof(DataBuildSettingProfileOsx);
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.EmbeddedLinux: return typeof(DataBuildSettingProfileLinux);
                case BuildTarget.iOS: return typeof(DataBuildSettingProfileIos);
                case BuildTarget.Android: return typeof(DataBuildSettingProfileAndroid);
                case BuildTarget.Switch: return typeof(DataBuildSettingProfileSwitch);
            }
            return null;
        }

        /// <summary>
        /// create a new profil asset for target platform, matching publish & sdk
        /// saved next to other profils of the same platform (or next to bridge if none)
        /// and added to bridge
        /// </summary>
        public DataBuildSettingProfile createPlatformProfil(BuildTarget target, TargetPublish publish, TargetSdks sdk)
        {
            System.Type type = getProfilType(target);
            if (type == null)
            {
                Debug.LogError("no profil type for platform : " + target);
                return null;
            }

            var profil = (DataBuildSettingProfile)CreateInstance(type);
            profil.publish = publish;
            profil.sdk = sdk;

            // folder of an existing profil of this platform, or bridge folder
            var sibling = getPlatformProfils(target)?.FirstOrDefault(x => x != null);
            string folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sibling != null ? sibling : this)).Replace('\\', '/');

            // [platform]_[publish](_[sdk])
            string fileName = profil.getPlatformUid() + "_" + publish;
            if (sdk != TargetSdks.none) fileName += "_" + sdk;

            string path = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + fileName + ".asset");
            AssetDatabase.CreateAsset(profil, path);

            addPlatformProfil(target, profil);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            Debug.Log("+profil @" + path, profil);
            return profil;
        }

        void addPlatformProfil(BuildTarget target, DataBuildSettingProfile profil)
        {
            DataBuildSettingProfile[] append(DataBuildSettingProfile[] arr)
                => (arr ?? new DataBuildSettingProfile[0]).Append(profil).ToArray();

            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows: windows = append(windows); break;
                case BuildTarget.StandaloneOSX: osx = append(osx); break;
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.EmbeddedLinux: linux = append(linux); break;
                case BuildTarget.iOS: ios = append(ios); break;
                case BuildTarget.Android: android = append(android); break;
                case BuildTarget.Switch: ninSwitch = append(ninSwitch); break;
            }
        }

    }
}
