using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex
    {
        public static string webRootFiles(this TM_UserData userData)
        {
            if (userData.notNull() && 
                userData.Path_UserData.valid() && 
                userData.Path_UserData.dirExists() && 
                userData.Path_WebRootFiles.valid() &&
                userData.UsingFileStorage)
            {
                return userData.Path_UserData.pathCombine(userData.Path_WebRootFiles);
            }
            return null;
        }
    }
}
