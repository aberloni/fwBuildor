using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// PlayerSettings.Android.v
/// PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)
///
/// https://mogutan.wordpress.com/2015/03/06/confusing-unity-mobile-player-settings-for-versions/
/// 
/// PlayerSettings.bundleVersion = major + "." + minor + "."+increment;
/// PlayerSettings.Android.bundleVersionCode = build;
/// PlayerSettings.iOS.buildNumber = PlayerSettings.bundleVersion;
///
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release
/// "Version" CFBundleShortVersionString(String - iOS, OS X) specifies the release version number of the bundle, which identifies a released iteration of the app.The release version number is a string comprised of three period-separated integers.
/// "Build" CFBundleVersion (String - iOS, OS X) specifies the build version number of the bundle, which identifies an iteration(released or unreleased) of the bundle.The build version number should be a string comprised of three non-negative, period-separated integers with the first integer being greater than zero.The string should only contain numeric (0-9) and period(.) characters.Leading zeros are truncated from each integer and will be ignored(that is, 1.02.3 is equivalent to 1.2.3). This key is not localizable.``
/// 
/// guide lines
/// https://stackoverflow.com/questions/21125159/which-ios-app-version-build-numbers-must-be-incremented-upon-app-store-release/38009895#38009895
/// 
/// The pair(Version, Build number) must be unique.
///   The sequence is valid: (1.0.1, 12) -> (1.0.1, 13) -> (1.0.2, 13) -> (1.0.2, 14) ...
///   Version(CFBundleShortVersionString) must be in ascending sequential order.
///   Build number(CFBundleVersion) must be in ascending sequential order.
///   
/// </summary>

namespace fwp.version
{
    public class VersionManager : MonoBehaviour
    {
        /// <summary>
        /// must be explicitly called
        /// </summary>
        static public void displayOnStartup()
        {
            Debug.Log(getPlayerSettingsVersion());

#if !noVersion
            new GameObject("--version").AddComponent<VersionManager>();
#endif
        }

        [SerializeField]
        float timer = 0f;

        float timerTarget = 4f;

        private void Awake()
        {

            /*
            //https://docs.unity3d.com/ScriptReference/Debug-isDebugBuild.html
            if (Debug.isDebugBuild)
            {
                timerTarget = -1f;
            }
            */

            timer = timerTarget;

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (timerTarget > 0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = timerTarget;
                    enabled = false;
                    //GameObject.Destroy(gameObject);
                }
            }
        }

        GUIStyle guis;
        float _sWidth;
        Rect rec = new Rect();

        void OnEnable()
        {
            if (guis == null)
            {
                guis = new GUIStyle();

                guis.normal.textColor = Color.gray;
                guis.alignment = TextAnchor.LowerRight;

                //updateFontSize();
            }
            
            if (_sWidth != Screen.width)
            {
                _sWidth = Screen.width;

                updateFontSize();
                updateRect();
            }
        }

        private void OnGUI()
        {
            //string v = Application.version;
            string v = formatVersion();
            GUI.Label(rec, v, guis);
        }

        /// <summary>
        /// this can only access PlayerSettings version
        /// with context #if and debug indicator
        /// </summary>
        virtual public string formatVersion()
        {
            string output = getPlayerSettingsVersion();

            if (Debug.isDebugBuild) output += " (devb)";

#if debug
            output += " (debug)";
#endif

            return output;
        }

        void updateRect()
        {
            rec.x = Screen.width - 30f;
            rec.y = Screen.height - 65f;
            rec.width = 25f;
            rec.height = 25f;
        }

        void updateFontSize()
        {
            float pxSize = 20f;
            float ratio = pxSize / 1920f; // N px on a 1080p screen
            int size = Mathf.FloorToInt(Screen.width * ratio);
            guis.fontSize = size;
        }

        static public void logVersionStamps(DataBuildSettingVersion v)
        {
            Debug.Log("v.timestamps " + v.getTimestamps());
        }

        /// <summary>
        /// major.minor.inc
        /// no build number
        /// </summary>
        static public string getFormatedVersion(int[] version, char separator = DataBuildSettingVersion.separator)
        {
            return version[0].ToString() + DataBuildSettingVersion.separator 
                + version[1] + DataBuildSettingVersion.separator 
                + version[2];
        }

        /// <summary>
        /// x.y.z , no dash
        /// </summary>
        static private int[] getPlayerSettingsVersionInts()
        {
            // https://docs.unity3d.com/ScriptReference/Application-version.html
            return splitVersion(getPlayerSettingsVersion());
        }

        static public int[] splitVersion(string v)
        {
            //Debug.Log(v);

            //default
            if (v.Length < 1 || v.IndexOf(".") < 0)
            {
                Debug.LogWarning("no version given, defaulting to 0.0.1");
                v = "0.0.1";
            }

            //gather numbers
            List<string> split = new List<string>();
            split.AddRange(v.Split('.'));

            //add missing members
            if (split.Count < 1) split.Add("0");
            if (split.Count < 2) split.Add("0");
            if (split.Count < 3) split.Add("0");

            //convert to int[]
            int[] output = new int[split.Count];
            for (int i = 0; i < split.Count; i++)
            {
                output[i] = int.Parse(split[i]);
            }
            return output;
        }

        static public string getPlayerSettingsVersion()
        {
            //return PlayerSettings.bundleVersion;
            return Application.version;
        }

        static public string getPlayerSettingsBuildNumber()
        {
            return string.Empty;
        }

    }

}
