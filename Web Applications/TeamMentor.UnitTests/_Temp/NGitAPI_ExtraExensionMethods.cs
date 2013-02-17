using System;
using NGit;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.UnitTests
{
    public static class NGitAPI_Extra_ExensionMethods
    {
        public static ObjectId  head(this API_NGit nGit)
        {
            try
            {
                return nGit.Repository.Resolve(Constants.HEAD);
            }
            catch (Exception ex)
            {
                ex.log();
                return null;
            }
            
        }
        public static API_NGit  create_File      (this API_NGit nGit, string virtualFileName, string fileContents)
        {
            return nGit.writeFile(virtualFileName, fileContents);
        }
    }
}
