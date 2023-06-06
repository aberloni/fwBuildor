using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.buildor
{

    public struct BuildHelperFlags
    {
        public bool autorun;
        public bool incVersion;
        public bool isPublishingBuild;

        public bool openFolderOnSuccess;
        public bool zipOnSuccess;
    }

    public struct BuildPathFlags
    {
        public bool pathIncludePrefix;
        public bool pathIncludePlatform;
        public bool pathIncludeDate;
        public bool pathIncludeVersion;
    }

}
