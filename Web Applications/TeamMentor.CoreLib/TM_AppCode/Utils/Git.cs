using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;

namespace TeamMentor.CoreLib
{
	public class Git
	{
		//GIT
		public static string syncWithGitHub_Pull_Origin()
		{
			var gitCommand = "pull origin";
			return executeGitCommand(gitCommand);
		}

		public static string syncWithGitHub_Push_Origin()
		{
			var gitCommand = "push origin";
			return executeGitCommand(gitCommand);
		}

		public static string syncWithGitHub_Commit()
		{
			return syncWithGitHub_Commit("TeamMentor Commit at: {0}".format(DateTime.Now));
		}

		public static string syncWithGitHub_Commit(string message)
		{
			executeGitCommand("add -A");
			var commit = "commit -m '{0}'".format(message);
			return executeGitCommand(commit);
		}

		public static string executeGitCommand(string gitCommand)
		{
            return null;     // disabling this since it is not used anymore
			/*var gitExe = @"C:\Program Files\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				gitExe = @"C:\Program Files (x86)\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				return "error: could not find git.exe: {0}".format(gitExe);

			var gitLocalProjectFolder = AppDomain.CurrentDomain.BaseDirectory.pathCombine("..//..").fullPath();
			// go up to the main git folder

			var cmdOutput = Processes.startAsCmdExe(gitExe, gitCommand, gitLocalProjectFolder)
			                         .fix_CRLF()
			                         .replace("\t", "    ");
			return cmdOutput;*/
		}

        // this is the one used now
        public static bool CloneUsingGit(string gitLocation, string targetFolder)
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