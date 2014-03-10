using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

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
			var gitExe = @"C:\Program Files\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				gitExe = @"C:\Program Files (x86)\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				return "error: could not find git.exe: {0}".format(gitExe);

			var gitLocalProjectFolder = AppDomain.CurrentDomain.BaseDirectory.pathCombine("..//..").fullPath();
			// go up to the main git folder

			var cmdOutput = Processes.startAsCmdExe(gitExe, gitCommand, gitLocalProjectFolder)
			                         .fix_CRLF()
			                         .replace("\t", "    ");
			return cmdOutput;
		}

	}
}