using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

namespace fwp.buildor.editor
{
    using System;
    using UnityEditor.Build.Reporting;

    public class BuildPostprocess : BuildProcess
    {
        BuildSummary summary;

        public Action onPostEnded;

        public BuildProcess doLaunch(DataBuildSettingProfile profil, BuildSummary summary)
        {
            this.summary = summary;

            launch(profil);

            return this;
        }

        protected override IEnumerator exec()
        {
            yield return null;
            yield return null;
            yield return null;

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    
                    onSuccess();

                    break;

                case BuildResult.Failed:
                case BuildResult.Cancelled:
                case BuildResult.Unknown:
                default:

                    onFailure();

                    break;
            }

            onPostEnded?.Invoke();

        }

        void onFailure()
        {
            log("<color=red>build.Failure</color>");
            Debug.LogError("Build failed: " + summary.result);

            Debug.LogError($"BuildResult : Helper Build : {summary.result}");
            Debug.LogError("options : " + summary.options);
            Debug.LogError("output path : " + summary.outputPath);

        }

        void onSuccess()
        {
            
            // autorun will inject a flag, no need to do it by hand
            /*
            if (_parameters.buildFlags.autorun)
            {
                Debug.Log("<color=orange>AUTORUN</color>");
                WinEdBuildor.winExecute(summary.outputPath); // autorun flag
            }
            */

            log("<color=green>build.Success</color>");

            log("<b>Build finished</b>");
            log("  L version : <b>" + profil.Version + "</b>");
            log("  L result : warnings : " + summary.totalWarnings + " | errors " + summary.totalErrors);
            log("  L symbols : " + ScriptableSymbolHelper.getGroupSymbols(summary.platformGroup));
            log("  L platform : <b>" + summary.platform + "</b>");
            log("  L build time : " + summary.totalTime);

            ulong bytes = summary.totalSize;
            ulong byteToMo = 1048576;

            int size = (int)(bytes / byteToMo);

            log("  L size : " + summary.totalSize + " bytes ; " + size + " Mo");
            log("  L path : " + summary.outputPath);

            if (BuildorVars.PostOpenFolder)
            {
                log($"+OPEN FOLDER of build : {summary.outputPath}");
                openBuildFolder(summary.outputPath); // success.open
            }

            if (BuildorVars.PostZip)
            {
                log($"+ZIP");

                zipBuildFolder(profil.BuildPath, profil.ZipFullPath); // autozip after build
            }

            if (BuildorVars.PostAutorun)
            {
                log($"+AUTORUN  @{profil.FullPath}");
                shellOpenFile(profil.FullPath);
            }

        }

        /// <summary>
        /// open FOLDER of current build
        /// </summary>
        static public void openBuildFolder(string path, bool parent = false)
        {
            // remove file name from path
            Debug.Log(path);

            if (path.Contains(BuildExecutor.Profile.getExtension()))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            if (parent)
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            os_openFolder(path);
        }

        /// <summary>
        /// zipBuildFolder("D:/builds/myGame", "D:/builds/myGame.zip");
        /// </summary>
        static public void zipBuildFolder(string folderPath, string zipPath)
        {
            EditorUtility.DisplayProgressBar("Zipping", "Compressing...", 0f);

            string parent = System.IO.Path.GetDirectoryName(folderPath);
            string folderName = System.IO.Path.GetFileName(folderPath);

            ulog("folder.drop: " + parent);
            ulog("folder.name: " + folderName);

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                shellExecute("cmd.exe",
                    $"/c tar -a -c -f \"{zipPath}\" \"{folderName}\"",
                    parent);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string args = $"-c \"zip -r '{zipPath}' '{folderPath}'\"";
                shellExecute("/bin/bash", args);
            }
            else
            {
                Debug.LogWarning("platform not supported");
            }

            EditorUtility.ClearProgressBar();
        }


        static public void shellExecute(string processPath, string args = "", string workingDir = "")
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = processPath;

            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false; // better control
            process.StartInfo.CreateNoWindow = false;

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += (s, e) => { if (e.Data != null) Debug.Log(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) Debug.LogError(e.Data); };

            if (!string.IsNullOrEmpty(workingDir)) process.StartInfo.WorkingDirectory = workingDir;

            try
            {
                Debug.Log($"shell @ {processPath} {args}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit(); // wait for process to finish
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// call of generic windows process
        /// </summary>
        static public void shellOpenFile(string processPath)
        {
            if (!System.IO.File.Exists(processPath))
            {
                Debug.LogWarning("missing file @ " + processPath);
                return;
            }
            shellExecute(processPath);
        }

        static public void osxExecute(string args)
        {
            Debug.Log("osx.bash : " + args);
            var pStart = new System.Diagnostics.ProcessStartInfo("/bin/bash");
            //pStart.FileName = "/System/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal";
            pStart.WorkingDirectory = "/";

            pStart.UseShellExecute = false;
            pStart.RedirectStandardInput = true;
            pStart.RedirectStandardOutput = true;

            var proc = new System.Diagnostics.Process();
            proc.StartInfo = pStart;
            proc.Start();

            proc.StandardInput.WriteLine(args);
            proc.StandardInput.Flush();
            //proc.WaitForExit();
        }


        /// <summary>
        /// open explorer at path
        /// </summary>
        /// <param name="folderPath"></param>
        static public void os_openFolder(string folderPath, bool selectFolder = false)
        {
            folderPath = folderPath.Replace(@"\", @"/"); // uniform
            if (!folderPath.EndsWith("/")) folderPath += "/"; // last /

            if (!System.IO.Directory.Exists(folderPath))
            {
                Debug.LogWarning("no folder " + folderPath);
                return;
            }

            string argument = string.Empty;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes

                if (selectFolder)
                {
                    //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
                    argument = "/select, ";
                }

                argument += "\"" + folderPath + "\"";

                UnityEngine.Debug.Log("explorer:opening : " + argument);

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {


                UnityEngine.Debug.Log("finder:opening : " + folderPath);
                EditorUtility.RevealInFinder(folderPath);
            }
            else
            {
                throw new System.NotImplementedException("platform not implem");
            }


        }

        [MenuItem("Window/Buildor/Logs/(folder) player logs")]
        static public void openPlayerLogsFolder()
        {
            //https://stackoverflow.com/questions/4494290/detect-the-location-of-appdata-locallow
            // Environment.SpecialFolder.LocalApplicationData)

            //startCmd("C:/Users/lego/AppData/LocalLow/com.redcorner.king/King");
            //startCmd("cmd", "start %APPDATA%"); // roaming
            //startCmd("cmd", "/K \"cd /D %LOCALAPPDATA%\""); // local
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "Low"; // c:/[USERAPPDATA]/LocalLow
            path = System.IO.Path.Combine(path, Application.companyName, Application.productName);
            // path = System.IO.Path.Combine(path, "Player.log");

            // shellOpenFile(path); // local
            os_openFolder(path);
        }

        [MenuItem("Window/Buildor/Logs/(folder) editor logs")]
        static public void openEditorLogsFolder()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData); // c:/[USERAPPDATA]/Local
            path = System.IO.Path.Combine(path, "Unity/Editor");
            Debug.Log(path);

            //startCmd("C:/Users/lego/AppData/LocalLow/com.redcorner.king/King");
            // os_openFolder("C:/Users/lego/AppData/Local/Unity/Editor");
            os_openFolder(path);
        }

    }

}