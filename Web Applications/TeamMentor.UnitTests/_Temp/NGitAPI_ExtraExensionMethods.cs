using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.Git;
using FluentSharp.Git.APIs;

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
            return nGit.refLogs_Raw()
                       .take(maxCount)
                       .select(refLog => refLog.str()).toList();
        }   
    }
}
