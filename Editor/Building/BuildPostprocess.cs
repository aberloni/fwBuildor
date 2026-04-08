using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

namespace fwp.buildor.editor
{
    using UnityEditor.Build.Reporting;

    public class BuildPostprocess : BuildProcess
    {
        public bool openFolderOnSuccess;
        public bool zipOnSuccess;

        BuildSummary summary;

        public BuildProcess doLaunch(DataBuildSettingProfile profil, BuildSummary summary)
        {
            this.summary = summary;

            launch(profil);

            return this;
        }

        protected override IEnumerator exec()
        {
            yield return null;

            switch (summary.result)
            {
                case BuildResult.Succeeded:

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
                    log("  L version : <b>" + fwp.version.VersionManager.getFormatedVersion() + "</b>");
                    log("  L result : warnings : " + summary.totalWarnings + " | errors " + summary.totalErrors);
                    log("  L symbols : " + ScriptableSymbolHelper.getGroupSymbols(summary.platformGroup));
                    log("  L platform : <b>" + summary.platform + "</b>");
                    log("  L build time : " + summary.totalTime);

                    ulong bytes = summary.totalSize;
                    ulong byteToMo = 1048576;

                    int size = (int)(bytes / byteToMo);

                    log("  L size : " + summary.totalSize + " bytes ; " + size + " Mo");
                    log("  L path : " + summary.outputPath);

                    yield return null;
                    if (openFolderOnSuccess)
                    {
                        log($"+OPEN FOLDER of build : {summary.outputPath}");
                        openBuildFolder(summary.outputPath); // success.open
                    }

                    yield return null;
                    if (zipOnSuccess)
                    {
                        string zipName = profil.getZipName();
                        log($"+ZIP {summary.outputPath}@{zipName}");
                        zipBuildFolder(summary.outputPath, zipName);
                    }

                    break;

                case BuildResult.Failed:
                case BuildResult.Cancelled:
                case BuildResult.Unknown:
                default:

                    log("<color=red>build.Failure</color>");
                    Debug.LogError("Build failed: " + summary.result);

                    Debug.LogError($"BuildResult : Helper Build : {summary.result}");
                    Debug.LogError("options : " + summary.options);
                    Debug.LogError("output path : " + summary.outputPath);


                    break;
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

            if(parent)
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            os_openFolder(path);
        }

        /// <summary>
        /// https://superuser.com/questions/201371/create-zip-folder-from-the-command-line-windows
        /// https://techcommunity.microsoft.com/t5/containers/tar-and-curl-come-to-windows/ba-p/382409
        /// https://ss64.com/nt/tar.html
        /// </summary>
        void zipBuildFolder(string outputPath, string zipName)
        {

            // path/to/builds/project/zipName.exe
            if (outputPath.Contains("exe") || outputPath.Contains("app"))
            {
                outputPath = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            }

            log("zip output path @ " + outputPath);

            // remove '/' just before exe file name
            if (outputPath.EndsWith("/")) outputPath = outputPath.Substring(0, outputPath.Length - 1);

            // parent folder to project/ (builds/)
            string buildsRoot = outputPath.Substring(0, outputPath.LastIndexOf('/'));
            log("output : " + outputPath);
            log("root : " + buildsRoot);

            // get project/
            string projectFolder = outputPath.Substring(outputPath.LastIndexOf('/') + 1);

            //string args = $"-cf {outputZip} {buildFolderPath}";

            // cd /D D:/fwProtoss/fw/builds/ && tar.exe -avcf fwp.zip fwp__win__0-0-11

            string folderToZip = $"{buildsRoot}/{projectFolder}";
            string pathZip = $"{buildsRoot}/{zipName}";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // https://stackoverflow.com/questions/60904/how-can-i-open-a-cmd-window-in-a-specific-location
                string command = $"/K cd /D {buildsRoot} && tar.exe -avcf {zipName} {projectFolder}";
                log("win.zip : " + command);
                shellOpenFile(command);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string command = $"zip -r {pathZip} {folderToZip}";
                log("osx.zip : " + command);
                osxExecute(command);
            }
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
		/// call of generic windows process
		/// </summary>
		static public void shellOpenFile(string processPath, string args = "")
		{
			if (!System.IO.File.Exists(processPath))
			{
				Debug.LogWarning("missing file @ " + processPath);
				return;
			}

			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = processPath;
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

			if (args.Length > 0) process.StartInfo.Arguments = args;

			Debug.Log("shell @ " + processPath);

			//https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
			process.Start();
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