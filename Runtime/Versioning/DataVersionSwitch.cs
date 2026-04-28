using UnityEngine;
using System.IO;

namespace fwp.version
{
    /// <summary>
    /// MAJOR.MINOR.PATCH
    /// 
    /// MAJOR : PlayerSettings.Switch.releaseVersion
    /// 
    /// can't set in editor, must edit NMETA after building rom
    /// MINOR : 
    /// PATCH : 4 bits, max value = 15
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "buildor/version/+switch", order = 100)]
    public class DataVersionSwitch : DataBuildSettingVersion
    {

        public int VersionRelease
        {
            get
            {
                return major;
            }
        }

        public string VersionMinorPatch
        {
            get
            {
                return (minor + separator + patch).ToString();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// how to apply version number on switch
        /// </summary>
        public override void applyVersionToEditor()
        {
            // this is ignored by nintendo
            // only set for editor feedback
            UnityEditor.PlayerSettings.bundleVersion = Version;

#if UNITY_SWITCH && !UNITY_6000_0_OR_NEWER
            // before u6000
            // switch section only exist with installed package

            // num inc rom release inc number
            // must be numeric string
            UnityEditor.PlayerSettings.Switch.releaseVersion = buildNumber.ToString();

            // user visible version
            UnityEditor.PlayerSettings.Switch.displayVersion = version;
#endif
        }
#endif

        public static void InjectVersionToRom(string romFolderPath, string version)
        {
            // Parse X.Y.Z
            string[] parts = version.Split('.');
            int major = int.Parse(parts[0]);
            int minor = int.Parse(parts[1]);
            int micro = int.Parse(parts[2]);

            // Validate
            if (major > 255) throw new System.ArgumentException("Major max is 255");
            if (minor > 255) throw new System.ArgumentException("Minor max is 255");
            if (micro > 15) throw new System.ArgumentException("Micro max is 15");

            // Find .nmeta
            string[] nmetaFiles = Directory.GetFiles(romFolderPath, "*.nmeta", SearchOption.AllDirectories);
            if (nmetaFiles.Length == 0) throw new FileNotFoundException("No .nmeta found in: " + romFolderPath);

            string nmetaPath = nmetaFiles[0];

            // Inject
            System.Xml.XmlDocument doc = new();
            doc.Load(nmetaPath);

            doc.SelectSingleNode("//Version/Major").InnerText = major.ToString();
            doc.SelectSingleNode("//Version/Minor").InnerText = minor.ToString();
            doc.SelectSingleNode("//Version/Micro").InnerText = micro.ToString();

            doc.Save(nmetaPath);
        }

    }

}
