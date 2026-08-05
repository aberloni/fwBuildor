using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    public enum TargetPublish
    {
        release = 0,
        demo, // specific version with #demo symbol
        festival, // specific version with #festival symbol
        custom, // custom launch #custom symbol
    }

    public enum TargetDebug
    {
        release = 0,
        debug,
    }

    /// <summary>
    /// must be uppercase
    /// used as symbols #if
    /// </summary>
    public enum TargetSdks
    {
        none = 0,
        STEAM = 1,
    }

    [Flags]
    public enum TargetFeatures
    {
        none = 0,
        debugTools = 1 << 1,
        watermark = 1 << 2,
        achievements = 1 << 3,
        lang_en = 1 << 4,
        metrics = 1 << 5,
    }
}
