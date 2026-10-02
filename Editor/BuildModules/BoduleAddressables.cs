using UnityEngine;
using UnityEditor;
using fwp.buildor.editor;

#if BUILDOR_ADDRESSABLES
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace fwp.buildor
{
    /// <summary>
    /// addressables content : clean and/or build, before player build
    /// needs com.unity.addressables (BUILDOR_ADDRESSABLES, editor asmdef version define), does nothing without it
    ///
    /// during a build, content build is skipped when unity already builds it with the player
    /// (addressables settings : Build Addressables on Player Build), to avoid building it twice
    /// a failed content build cancels the player build
    /// </summary>
    [CreateAssetMenu(menuName = BuildorHelpers._menuItem_basepath + "modules/+addressables", fileName = "addressables")]
    public class BoduleAddressables : BuildModule
    {
        [Header("clean")]
        [Tooltip("delete scriptable build pipeline cache (Library/BuildCache), next content build is a full rebuild (slow)")]
        public bool purgeBuildCache = false;

        [Tooltip("delete content built by all addressables builders")]
        public bool cleanContent = false;

        [Header("rebuild")]
        [Tooltip("save assets & build content of active build target, incremental (unchanged assets are not rebuilt)\nskipped during a build if unity builds content with player\noff : clean steps only")]
        public bool rebuildAddressable = true;

        [Tooltip("marked dirty & saved before building content (ie: assets modified by code without SetDirty)")]
        public Object[] saveBeforeBuild = new Object[0];

        protected override void doApply(BuildContext ctx)
        {
#if BUILDOR_ADDRESSABLES
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("addressables : no addressables settings in project");
                return;
            }

            if (purgeBuildCache)
            {
                BuildProcess.ulog("-build cache", this);
                BuildCache.PurgeCache(false);
            }

            if (cleanContent)
            {
                BuildProcess.ulog("-content", this);
                AddressableAssetSettings.CleanPlayerContent();
            }

            if (!rebuildAddressable) return;

            saveAssets();

            // building : unity will build content during BuildPlayer
            // manual apply : no player build follows, always build
            if (BuildExecutor.IsBusy && isBuiltWithPlayer(settings))
            {
                BuildProcess.ulog("content built with player (addressables settings)", this);
                return;
            }

            BuildProcess.ulog("+content " + ctx.Target, this);
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

            // thrown during preprocess : build is cancelled
            if (!string.IsNullOrEmpty(result.Error))
                throw new BuildFailedException("addressables : content build failed : " + result.Error);

            BuildProcess.ulog("content built in " + result.Duration.ToString("F1") + "s @" + result.OutputPath, this);
#else
            Debug.LogWarning("addressables : package com.unity.addressables not installed");
#endif
        }

#if BUILDOR_ADDRESSABLES
        /// <summary>
        /// AddressablesPreferences.kBuildAddressablesWithPlayerBuildKey (internal)
        /// </summary>
        const string pref_buildWithPlayer = "Addressables.BuildAddressablesWithPlayerBuild";

        /// <summary>
        /// same as AddressablesPlayerBuildProcessor.ShouldBuildAddressablesForPlayerBuild (internal)
        /// </summary>
        static bool isBuiltWithPlayer(AddressableAssetSettings settings)
        {
            switch (settings.BuildAddressablesWithPlayerBuild)
            {
                case AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer: return true;
                case AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer: return false;
                default: return EditorPrefs.GetBool(pref_buildWithPlayer, true); // PreferencesValue
            }
        }

        void saveAssets()
        {
            foreach (var o in saveBeforeBuild)
            {
                if (o != null) EditorUtility.SetDirty(o);
            }

            AssetDatabase.SaveAssets();
        }
#endif

        public override string strOneLine()
        {
            string ret = base.strOneLine();
            if (purgeBuildCache) ret += " -cache";
            if (cleanContent) ret += " -content";
            if (rebuildAddressable) ret += " +content";
            return ret;
        }
    }
}
