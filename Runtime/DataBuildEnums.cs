using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    [System.Serializable]
    public enum PublishLevel
    {
        release = 0,
        demo,
        festival,
    }

    [System.Serializable]
    public enum DebugLevel
    {
        release = 0,
        debug,
    }
    
    [System.Serializable]
    public enum Sdks
    {
        none = 0,
        steam,
        ninSwitch,
    }

}
