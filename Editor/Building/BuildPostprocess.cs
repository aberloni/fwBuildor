using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        static public void openBuildFolder(string path)
        {
            // remove file name from path
            if (path.Contains(BuildHelperBase.Profile.getExtension()))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            WinEdBuildor.os_openFolder(path);
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
                WinEdBuildor.winExecute(command);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string command = $"zip -r {pathZip} {folderToZip}";
                log("osx.zip : " + command);
                WinEdBuildor.osxExecute(command);
            }
        }

    }

}