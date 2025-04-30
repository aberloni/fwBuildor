using UnityEngine;
using System;
using System.Diagnostics;

/// <summary>
/// tools platform specific related
/// </summary>

namespace fwp.buildor
{

    static public class SystemDetection
	{

#if UNITY_EDITOR
        /// <summary>
        /// internal, editor
        /// </summary>
        static public void os_browseFolder(string folderPath)
        {

            UnityEditor.EditorUtility.RevealInFinder(folderPath);
        }
#endif

        static public string generateUniqId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// open explorer at path
        /// </summary>
        /// <param name="folderPath"></param>
        static public void os_openFolder(string folderPath, bool selectFolder = false)
        {
            folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

            string argument = string.Empty;

            if (selectFolder)
            {
                //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                argument = "/select, ";
            }

            argument += "\"" + folderPath + "\"";

            UnityEngine.Debug.Log("cmd:opening : " + argument);

            //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        static public void os_cmd(string command)
        {
            // https://stackoverflow.com/questions/181719/how-do-i-start-a-process-from-c   
            //System.Diagnostics.Process.Start("cmd.exe", command);
            //Process process = Process.Start($"cmd.exe");

            // https://stackoverflow.com/questions/1255909/execute-cmd-command-from-code/1255928#1255928
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.WorkingDirectory = "c:\temp";
            process.StartInfo.Arguments = "/c " + command;

            process.Start();
            //lock unity until exe is done
            //process.WaitForExit();

            //process.Close();
        }


        //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
        /// <summary>
        /// Windows Store Apps: Application.persistentDataPath points to %userprofile%\AppData\Local\Packages\<productname>\LocalState.
        /// iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
        /// Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices
        /// </summary>
        /// <returns></returns>
        static public string getDataPath()
        {
            return Application.persistentDataPath;
        }

        static public bool isLinux()
        {
            return Application.platform == RuntimePlatform.LinuxPlayer;
        }

        static public bool isDesktop()
        {
            if (isWindows()) return true;
            if (isOsx()) return true;
            if (isLinux()) return true;

            return false;
        }

        /// <summary>
        /// https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
        /// </summary>
        /// <returns></returns>
        static public bool isWindows()
        {
#if UNITY_STANDALONE
            return true;
#else
    //return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
    return false;
#endif
        }

        /// <summary>
        /// mac
        /// </summary>
        static public bool isOsx()
        {
#if UNITY_STANDALONE_OSX
    return true;
#elif UNITY_MACOS
    return true;
#else
            //return Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor;
            return false;
#endif
        }

        static public bool isAndroid()
        {
#if UNITY_ANDROID
    return true;
#else
            //return Application.platform == RuntimePlatform.Android;
            return false;
#endif
        }

        static public bool isIos()
        {
#if UNITY_IOS
    return true;
#else
            //return Application.platform == RuntimePlatform.IPhonePlayer;
            return false;
#endif
        }

        static public bool isSwitch()
        {
#if UNITY_SWITCH
    return true;
#else
            //return Application.platform == RuntimePlatform.Switch;
            return false;
#endif
        }

        static public bool isTouchSupported()
        {
            return Input.touchSupported;
        }


        /// <summary>
        /// Application.platform check for android&ios
        /// </summary>
        static public bool isMobile()
        {
            if (isIos()) return true;
            if (isAndroid()) return true;
            return false;
        }



        /// <summary>
        /// meant to call cmd on windows
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="args"></param>
        static public void startCmd(string fullPath, string args = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            if (args.Length > 0) startInfo.Arguments = args;

            //Debug.Log(Environment.CurrentDirectory);

            Process.Start(startInfo);

        }

    }

}