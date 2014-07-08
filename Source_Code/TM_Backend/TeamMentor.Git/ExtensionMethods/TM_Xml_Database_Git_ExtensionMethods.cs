using System;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Git;
using FluentSharp.Git.APIs;
using FluentSharp.Web;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Git_ExtensionMethods
    {
        public static TM_Xml_Database   setupGitSupport(this TM_Xml_Database_Git tmDatabaseGit)
        {
            var tmFileStorage = TM_FileStorage.Current;
            var tmDatabase = tmFileStorage.TMXmlDatabase;

            var gitConfig = tmFileStorage.tmServer().Git;

            if (gitConfig.LibraryData_Git_Enabled)
            {
                var online = WebUtils.online();
                if (online)
                    "[TM_Xml_Database] [setupGitSupport] we are online, so git Pull and Pull will be attempted".info();
                else
                    "[TM_Xml_Database] [setupGitSupport] we are offline, so no git Pull and Pulls".info();
                foreach (var library in tmDatabase.tmLibraries())
                {
                    var libraryPath = tmDatabase.xmlDB_Path_Library_XmlFile(library).parentFolder();
                    if (libraryPath.isGitRepository())
                    {
                        var nGit = libraryPath.git_Open();
                        if (online)   
                        {
                            nGit.git_Pull_Library();
                            nGit.git_Push_Library();
                        }                                                
                        tmDatabaseGit.NGits.Add(nGit);
                    }
                    else
                        "[TM_Xml_Database] [setupGitSupport]  library {0} is currently not a git repo".info(libraryPath.folderName());
                }
                tmDatabaseGit.triggerGitCommit();                
            }
            return tmDatabase;        
        }
        public static TM_Xml_Database_Git   triggerGitCommit (this TM_Xml_Database_Git tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;
            if (tmFileStorage.tmServer().Git.LibraryData_Git_Enabled)
            {
                foreach(var nGit in tmDatabase.NGits)
                    if (nGit.status().valid())
                        nGit.gitCommit_SeparateThread();
            }
            return tmDatabase;
        }
        public static API_NGit          git_Push_Library(this API_NGit nGit)
        {
            var tmFileStorage = TM_FileStorage.Current;
            var tmServer      = tmFileStorage.tmServer();
            if(tmServer.notNull())
                if (tmServer.Git.LibraryData_Auto_Push)
                    try
                    {
                        nGit.push();
                    }
                    catch (Exception ex)
                    {
                        ex.log("git_Push_Library");
                    }
            return nGit;
        }
        public static API_NGit          git_Pull_Library(this API_NGit nGit)
        {
            var tmFileStorage = TM_FileStorage.Current;
            var tmServer = tmFileStorage.tmServer();
            if (tmServer.notNull())
                if (tmServer.Git.LibraryData_Auto_Pull)
                    try
                    {
                        nGit.pull();
                    }
                    catch (Exception ex)
                    {
                        ex.log("git_Pull_Library");
                    }
            return nGit;
        }
        public static API_NGit          gitCommit_SeparateThread(this API_NGit nGit)
        {
            O2Thread.mtaThread(
                ()=>{                        
                        lock (nGit)
                        {
                            nGit.setDefaultAuthor();
                            nGit.add_and_Commit_using_Status();
                            nGit.git_Push_Library();
                        }
                });
            return nGit;
        }

        public static TM_Xml_Database   handle_UserData_GitLibraries(this TM_Xml_Database tmDatabase)
        {
            try
            {
                var tmFileStorage = TM_FileStorage.Current;
                if (WebUtils.online())
                    "[TM_Xml_Database] [handle_UserData_GitLibraries] online, so checking for TM UserData repos to clone".info();
                else
                    "[TM_Xml_Database] [handle_UserData_GitLibraries] online".info();
                foreach (var gitLibrary in tmFileStorage.UserData.SecretData.Libraries_Git_Repositories)
                {
                    if (gitLibrary.regEx("Lib_.*.git"))
                    {
                        var libraryName = gitLibrary.split("Lib_").last().remove(".git").replace("_" , " ");
                        var targetFolder = tmFileStorage.Path_XmlLibraries.pathCombine(libraryName);
                        if (targetFolder.dirExists().isFalse())
                        {
                            "[TM_Xml_Database] [handle_UserData_GitLibraries] cloning {0}".info(libraryName);
                            tmDatabase.clone_Library(gitLibrary,targetFolder);
                            //gitLibrary.git_Clone(targetFolder);
                        }
                        else 
                            "[TM_Xml_Database] [handle_UserData_GitLibraries] skipping git clone since there was already a library called: {0}".info(libraryName);
                    }
                    else
                        "[handle_UserData_GitLibraries] provided git library didn't fit expected format (it should be called Lib_{LibName}.git, and it was: {0}".error(gitLibrary);
                }
            }
            catch (Exception ex)
            {
                ex.log("handle_UserData_GitLibraries");
                
            }            
            return tmDatabase;
        }
        public static TM_Xml_Database   clone_Library      (this TM_Xml_Database tmDatabase, string gitLibrary, string targetFolder)
        {
            var start = DateTime.Now;
            "[TM_Xml_Database] [GitClone] Start".info();
            if (GitExe.CloneUsingGitExe(gitLibrary,targetFolder).isFalse())
            {
                "[TM_Xml_Database] [GitClone] Using NGit for the clone".info();    
                gitLibrary.git_Clone(targetFolder);
            }
            
            "\n\n[TM_UserData] [GitClone] in: {0}\n\n".debug(start.duration_To_Now());
            return tmDatabase;
        }
    }
}