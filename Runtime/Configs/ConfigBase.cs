using UnityEngine;

namespace fwp.configs
{
    public class ConfigBase : ScriptableObject
    {
        const string path_configs = "Assets/Resources/buildor";

        protected static T GetOrCreate<T>(string name) where T : ConfigBase
        {
            string path = $"Buildor/{name}";
            T asset = Resources.Load<T>(path);

#if UNITY_EDITOR
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                string fullPath = $"{path_configs}/{name}.asset";
                System.IO.Directory.CreateDirectory(path_configs);
                UnityEditor.AssetDatabase.CreateAsset(asset, fullPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif

            return asset;
        }
    }
}