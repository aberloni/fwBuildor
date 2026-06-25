using UnityEngine;

namespace fwp.configs
{

    [CreateAssetMenu(menuName = "buildor/configs/+demo", order = 100)]
    public class ConfigDemo : ConfigBase
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Window/Buildor/(?) demo")]
#endif
        static public void logDemoState()
        {
            Debug.Log("demo ? " + IsDemo);
        }

        [SerializeField]
        bool _usage = false;

        public bool Usage
        {
            get => _usage;
            set
            {
                _usage = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        static ConfigDemo instance;
        static public ConfigDemo Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = GetOrCreate<ConfigDemo>("demo");
                return instance;
            }
        }

        static public bool IsDemo
        {
            get
            {
                if (Instance == null) return false;
                return Instance.Usage;
            }
        }
    }

}