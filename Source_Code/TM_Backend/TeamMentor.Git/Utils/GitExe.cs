using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;

namespace TeamMentor.CoreLib
{
	public class GitExe
	{		
        public static bool CloneUsingGitExe(string gitLocation, string targetFolder)
        {
            if ("git.exe".startProcess_getConsoleOut().valid())     // we found Git
            {
                "[GitUtil] Using Git.Exe for the clone".info();
                var parameters = "clone \"{0}\" \"{1}\"".format(gitLocation.remove("\""), targetFolder.remove("\""));
                var cloneLog   = "git.exe".startProcess_getConsoleOut(parameters);
                "[GitUtil {0}".info(cloneLog);
                if (targetFolder.isGitRepository())
                    return true;
                "[GitUtil Target folder was not a Git REPO".error(targetFolder);
            }
            "[GitUtil] could not find git.exe on the current server".info();
            return false;
        }
    }
}