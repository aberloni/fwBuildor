using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor.version
{
    [System.Serializable]
    public enum PublishLevel
    {
        intern = 0,
        festival,
        demo,
        release,
    }

    [System.Serializable]
    public enum BuildPhase
    {
        none,
        φ, // proto
        α, // alpha
        β, // beta
        Ω  // gold
    }

}
