using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    public enum TargetPublish
    {
        release = 0,
        demo,
        festival,
    }

    public enum TargetDebug
    {
        release = 0,
        debug,
    }

    public enum TargetSdks
    {
        none = 0,
        steam,
        ninSwitch,
    }

    [Flags]
    public enum TargetFeatures
    {
        none = 0,
        debugTools = 1 << 1,
        watermark  = 1 << 2,
        achievements  = 1 << 3,
        lang_en = 1 << 4,
        metrics = 1 << 5,
    }
}
