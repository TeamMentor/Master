using System.Collections.Generic;
using System.Linq;
using NGit.Diff;
using NGit.Revwalk;
using NGit.Storage.File;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;
using Sharpen;

namespace TeamMentor.UnitTests
{
    public static class NGitAPI_Extra_ExensionMethods
    {
        public static List<string> refLogs(this API_NGit nGit)
        {
            return nGit.refLogs(100);
        }

        public static List<string>  refLogs(this API_NGit nGit, int maxCount)
        {
            return nGit.reflogs_Raw()
                       .take(maxCount)
                       .select(refLog => refLog.str()).toList();
        }   
    }
}
