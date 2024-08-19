using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{
    [System.Serializable]
    public class BuildHelperFlags
    {
        public bool autorun;
        public bool incVersion;
        public bool isPublishingBuild;

        public bool openFolderOnSuccess;
        public bool zipOnSuccess;
    }

    [System.Serializable]
    public class BuildPathFlags
    {
        public bool pathIncludePrefix;
        public bool pathIncludePlatform;
        public bool pathIncludeDate;
        public bool pathIncludeVersion;
    }

}
