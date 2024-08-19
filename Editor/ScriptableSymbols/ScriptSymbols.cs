using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.symbols
{
    using UnityEditor;
    using System;

    /// <summary>
    /// //https://steamworks.github.io/installation/
    /// </summary>
    enum ScriptSymbolsGenerics
    {
        debug, // all debug tooling

        watermark,

        achievements, // achievement tracking

        noSave, // will remove any save present on EVERY startup

        noSplash, noVideo,
    }

    enum ScriptSymbolsProfiling
    {
        profiling,
        benchmark,
    }

    enum ScriptSymbolsExternals
    {
        festival,
    }

    enum ScriptSymbolsVerbosity
    {

        logs, verbose,

    }

    enum ScriptSymbolsLocalization
    {
        // force localization default value
        loca_en, loca_fr, loca_cn, loca_jp,
    }

    enum ScriptSymbolsSteam
    {
        STEAMWORKS_WIN, STEAMWORKS_LIN_OSX, // plugin dll
    }

    enum ScriptSymbolsNinSwitch
    {
        NN_PLUGIN_ENABLE, // tells editor that plugin is present
    }

    enum ScriptSymbolsSimulator
    {
        build,
        ninSwitch,
    }

    public static class ScriptSymbols
    {

        static Type[] supported_enums = {
            typeof(ScriptSymbolsGenerics),
            typeof(ScriptSymbolsLocalization),
            typeof(ScriptSymbolsVerbosity),
            typeof(ScriptSymbolsSimulator),
            typeof(ScriptSymbolsExternals),
            typeof(ScriptSymbolsProfiling),

            // 3rd
            typeof(ScriptSymbolsSteam),
            typeof(ScriptSymbolsNinSwitch),
        };

        static List<Enum> _enums = new List<Enum>();

        static public List<Enum> extractEnums()
        {
            if (_enums != null && _enums.Count > 0) return _enums;

            List<Enum> ret = new List<Enum>();
            foreach (var e in supported_enums)
            {
                var enumValue = (Enum)System.Activator.CreateInstance(e);
                ret.Add(enumValue);
            }

            _enums = ret;

            return ret;
        }

        /// <summary>
        /// solve from active hardware platform
        /// </summary>
        static public BuildTargetGroup evaluateCurrentTargetPlatform()
        {
            BuildTargetGroup ret = BuildTargetGroup.Unknown;

            if (isAndroid()) ret = BuildTargetGroup.Android;
            else if (isIos()) ret = BuildTargetGroup.iOS;
            else if (isSwitch()) ret = BuildTargetGroup.Switch;
            else if (isDesktop()) ret = BuildTargetGroup.Standalone;

            return ret;
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

    }
}
