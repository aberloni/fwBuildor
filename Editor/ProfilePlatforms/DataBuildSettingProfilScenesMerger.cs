using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 
/// https://docs.unity3d.com/ScriptReference/EditorBuildSettingsScene.html
/// 
/// </summary>

namespace fwp.buildor
{
    [CreateAssetMenu(menuName = "buildor/merger/create DataBuildSettingProfilScenesMerger", order = 100)]
    public class DataBuildSettingProfilScenesMerger : ScriptableObject
    {
        [Tooltip("engine scenes")]
        public DataBuildSettingProfilScenes[] cores;

        [Tooltip("game content")]
        public DataBuildSettingProfilScenes[] levels;

        [Tooltip("debug content")]
        public DataBuildSettingProfilScenes[] debugs;

        public DataBuildSettingProfilScenes[] menus;

#if UNITY_EDITOR

        List<EditorBuildSettingsScene> tmp = new List<EditorBuildSettingsScene>();

        [ContextMenu("apply")]
        public void apply()
        {
            tmp.Clear();

            inject(cores);
            inject(levels);
            inject(debugs);
            inject(menus);

            EditorBuildSettings.scenes = tmp.ToArray();
        }

        void inject(DataBuildSettingProfilScenes[] profils)
        {
            if (profils.Length <= 0)
                return;

            foreach (DataBuildSettingProfilScenes item in profils)
            {
                for (int i = 0; i < item.paths.Length; i++)
                {
                    tmp.Add(new EditorBuildSettingsScene(item.paths[i], true));
                }
            }

        }

        static public void addSceneToBuildSettings(string sceneName)
        {
            //keep existing
            List<EditorBuildSettingsScene> buildsettingScenes = new List<EditorBuildSettingsScene>();
            buildsettingScenes.AddRange(EditorBuildSettings.scenes);

            bool found = false;
            for (int j = 0; j < buildsettingScenes.Count; j++)
            {
                if (buildsettingScenes[j].path.Contains(sceneName))
                {
                    found = true;
                }
            }

            if (!found)
            {
                string completePath = getSceneCompletePath(sceneName);
                buildsettingScenes.Add(new EditorBuildSettingsScene(completePath, true));

                Debug.Log("ADDED " + completePath);
            }

            EditorBuildSettings.scenes = buildsettingScenes.ToArray();
        }

        static public string getSceneCompletePath(string sceneName)
        {
            //AssetDatabase.FindAssets<Scene>()

            string[] all = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].Contains(sceneName + ".unity"))
                {
                    return all[i];
                }
            }

            return string.Empty;
        }

        public static void injectLevel(string level)
        {
            DataBuildSettingProfilScenesMerger merger = getScriptableObjectInEditor<DataBuildSettingProfilScenesMerger>("everything");

            if (merger == null)
            {
                Debug.LogError("no merger for level " + level);
                return;
            }

            if (merger.levels == null)
            {
                Debug.LogError("no levels on merger " + merger.name);
                return;
            }

            for (int i = 0; i < merger.levels.Length; i++)
            {
                if (merger.levels[i].name.Contains(level))
                {
                    merger.levels[i].add();
                    return;
                }
            }

            Debug.LogWarning("didn't find to inject : " + level);
        }

        public static void injectAll(string filter = "everything")
        {
            //DataBuildSettingProfilScenes scenes = HalperScriptables.getScriptableObjectInEditor<DataBuildSettingProfilScenes>("game_release");
            DataBuildSettingProfilScenesMerger merger = getScriptableObjectInEditor<DataBuildSettingProfilScenesMerger>(filter);

            merger.apply();

            Debug.Log("re-applied all scenes from scriptable " + merger.name, merger);
        }
#endif



#if UNITY_EDITOR

        static public T[] getScriptableObjectsInEditor<T>() where T : ScriptableObject
        {
            string[] all = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            List<T> output = new List<T>();
            for (int i = 0; i < all.Length; i++)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
                T data = obj as T;
                if (data == null) continue;
                output.Add(data);
            }
            return output.ToArray();
        }

        static public T getScriptableObjectInEditor<T>(string nameContains = "") where T : ScriptableObject
        {
            string[] all = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            for (int i = 0; i < all.Length; i++)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
                T data = obj as T;

                if (data == null) continue;
                if (nameContains.Length > 0)
                {
                    if (!data.name.Contains(nameContains)) continue;
                }

                return data;
            }
            Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameContains + ")");
            return null;
        }
#endif


    }
}