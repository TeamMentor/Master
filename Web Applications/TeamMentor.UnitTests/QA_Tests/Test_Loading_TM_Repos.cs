using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FluentSharp.CoreLib;
using FluentSharp.Git;
using FluentSharp.Git.APIs;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.QA_Tests
{
    public class Git_Clone_Data
    {
        [XmlAttribute] public string   When          { get; set; }
        [XmlAttribute] public double   Clone_Seconds { get; set; }
        [XmlAttribute] public string   Clone_Type    { get; set; }
        [XmlAttribute] public string   Repo_Name     { get; set; }
        [XmlAttribute] public string   Repo_Source   { get; set; }
        [XmlAttribute] public string   Repo_Path     { get; set; }
        [XmlAttribute] public int      Repo_Files    { get; set; }
    }

    [TestFixture][Ignore]
    public class Test_Loading_TM_Repos
    {
        public String GitHub_Repo_Path      { get; set; }
        public string TM_Libraries_Folder   { get; set; }
        public string Temp_Cloned_Repos     { get; set; }
        public string GitExe_Path           { get; set; }        
        public bool   Skip_Online_Tests     { get; set; }
        public bool   Delete_Temp_Repos     { get; set; }
        public bool   Open_Temp_Repo_Folder { get; set; }
        public bool   Save_Git_Clone_Data   { get; set; }                        

        public Test_Loading_TM_Repos()
        {
            TM_Libraries_Folder = @"E:\TeamMentor\TM_Releases\TM_Libraries\3.4.1_Libs";
            Assert.IsNotNull(TM_Libraries_Folder);        
            Assert.IsTrue(TM_Libraries_Folder.dirExists());

            Temp_Cloned_Repos = "_Temp_Clones".tempDir(false);
            Assert.IsNotNull(Temp_Cloned_Repos);
            
            GitHub_Repo_Path = "git@github.com:TMContent/Lib_{0}.git";
            Assert.IsNotNull(GitHub_Repo_Path);
                        
            GitExe_Path= "git.exe";
            Assert.IsNotNull(GitExe_Path);
            var gitExecutionResult = gitExe_Execute("","");
            Assert.IsTrue (gitExecutionResult.starts("usage: git"));
            
            Skip_Online_Tests     = false;
            Delete_Temp_Repos     = true;
            Save_Git_Clone_Data   = true;
            Open_Temp_Repo_Folder = true;
            if (Open_Temp_Repo_Folder)
                Temp_Cloned_Repos.startProcess();
        }       
        
        [Test] public void Test_Clone_Net_2_0()
        {           
            runCloneForNGitAndGitExe(".NET_2.0"          , 536, 20);            
        }   
        [Test] public void Test_Clone_Net_3_5()
        {
            runCloneForNGitAndGitExe(".NET_3.5"          , 839, 20);           
        }
        [Test] public void Test_Clone_Net_4_0()
        {
            runCloneForNGitAndGitExe(".NET_4.0"          , 452, 20);            
        }
        [Test] public void Test_Clone_Android()
        {
            runCloneForNGitAndGitExe("Android"           , 058, 10);            
        }
        [Test] public void Test_Clone_CPP()
        {
            runCloneForNGitAndGitExe("CPP"               , 491, 15);            
        }
        [Test] public void Test_Clone_CWE()
        {
            runCloneForNGitAndGitExe("CWE"               , 110, 10);            
        }
        [Test] public void Test_Clone_HTML5()
        {
            runCloneForNGitAndGitExe("HTML5"             , 166, 12);            
        }
        [Test] public void Test_Clone_iOS()
        {
            runCloneForNGitAndGitExe("iOS"               , 064, 10);            
        }
        [Test] public void Test_Clone_Java()
        {
            runCloneForNGitAndGitExe("Java"              , 661, 20);            
        }        
        [Test] public void Test_Clone_PHP()
        {
            runCloneForNGitAndGitExe("PHP"               , 594, 120000);         
        }
        [Test] public void Test_Clone_Scala()
        {
            runCloneForNGitAndGitExe("Scala"             , 181, 10);            
        }
        [Test] public void Test_Clone_Vulnerabilities()
        {
            runCloneForNGitAndGitExe("Vulnerabilities"   , 188, 20);    
            "continue".alert();
        }
        [Test] public void Test_Clone_PCI_DSS_Compliance()
        {
            runCloneForNGitAndGitExe("PCI_DSS_Compliance", 333, 12);            
        }
        
        
        //helper function

        public void runCloneForNGitAndGitExe(string repoName, int numberOfFiles, int maxSeconds)
        {
            cloneAndCheck(gitExe_Local_Clone , "Git.exe__Local", repoName, numberOfFiles, maxSeconds);            
            cloneAndCheck(NGit_Local_Clone   , "Ngit_____Local", repoName, numberOfFiles, maxSeconds);                          
            if (Skip_Online_Tests)
            {
                "*** Skip_Online_Tests is set to true so Skipping GitHub clones".debug();                
                return;
            }
            cloneAndCheck(gitExe_GitHub_Clone, "Git.exe_GitHub", repoName, numberOfFiles, maxSeconds);
            cloneAndCheck(NGit_GitHub_Clone  , "NGit____GitHub", repoName, numberOfFiles, maxSeconds);                
        }
        public void cloneAndCheck(Func<string,API_NGit> cloneFunction, String cloneType, string repoName, int numberOfFiles, int maxSeconds)
        {            
            var start         = DateTime.Now;
            var nGit          = cloneFunction (repoName);       
            if (nGit.isNull() && Skip_Online_Tests)
                return;
            var nGit_Files = nGit.files().size();
            var cloneSeconds = (DateTime.Now - start).Seconds;
            "***** [cloneAndCheck] Cloned {0} in {1} sec and got {2} files".debug(repoName, cloneSeconds, nGit_Files);
            Assert.AreEqual(numberOfFiles, nGit_Files);            
            Assert.Greater (maxSeconds   , cloneSeconds);
            
            var cloneData = new Git_Clone_Data
                {
                    When          = DateTime.Now.str(),
                    Clone_Seconds = Math.Round((double)cloneSeconds,2),
                    Repo_Files    = nGit_Files,
                    Clone_Type    = cloneType,
                    Repo_Name     = repoName,
                    Repo_Path     = nGit.Path_Local_Repository,
                    Repo_Source   = nGit.remote_Url("origin")
                };
            var path = nGit.Path_Local_Repository;
            var source = nGit.remote_Url("origin");
            saveCloneData(cloneData);

            if (Delete_Temp_Repos)
                Assert.IsTrue  (nGit.delete_Repository_And_Files());
        }

        public void saveCloneData(Git_Clone_Data cloneData)
        {            
            if (Save_Git_Clone_Data)
            {
                var results_XmlFile = Temp_Cloned_Repos.pathCombine("Git_Clone_Data.xml");
                var gitCloneData = (results_XmlFile.fileExists())
                                        ? results_XmlFile.load<List<Git_Clone_Data>>()
                                        : new List<Git_Clone_Data>();
                gitCloneData.add(cloneData);
                gitCloneData.saveAs(results_XmlFile);
            }
        }
        public API_NGit checkRepo(string repo_Source, string repo_Clone)
        {            
            Assert.IsTrue   (repo_Clone.dirExists());

            var nGit       = repo_Clone.git_Open();            

            Assert.IsNotNull(nGit);
            Assert.IsTrue   (repo_Clone.isGitRepository());
            Assert.IsTrue   (nGit.commits_SHA1().notEmpty());
            Assert.IsTrue   (nGit.files().notEmpty());
            Assert.AreEqual (nGit.remote_Url("origin"),repo_Source);            
            return nGit;
        }

        //NGIT
        
        public API_NGit NGit_Local_Clone(string repoName)
        {
            "\n\n    *** NGit_Local_Clone  ***\n\n".info();            
            var repo_Source     = TM_Libraries_Folder.pathCombine(repoName);
            var repo_Clone_Name = repoName.append("_NGit_Local_").add_RandomLetters(3);
            Assert.IsTrue    (repo_Source.dirExists());  
            Assert.IsTrue    (repo_Source.isGitRepository());   
            return NGit_Clone(repo_Source, repo_Clone_Name);
        }
        public API_NGit NGit_GitHub_Clone(string repoName)
        {
            "\n\n    *** NGit_GitHub_Clone  ***\n\n".info();                        
            
            var repo_Source     = GitHub_Repo_Path.format(repoName);
            var repo_Clone_Name = repoName.append("_NGit_GitHub_").add_RandomLetters(3);
            
            return NGit_Clone(repo_Source, repo_Clone_Name);
        }       
        public API_NGit NGit_Clone(string repo_Source, string repo_Clone_Name)
        {
            var repo_Clone  = Temp_Cloned_Repos.pathCombine(repo_Clone_Name);
            
            Assert.IsFalse  (repo_Clone.dirExists());

            var nGit        = repo_Source.git_Clone(repo_Clone);
            Assert.IsNotNull(nGit);
            nGit.close();
            return checkRepo(repo_Source,repo_Clone);
        }

        //Git.exe
        public API_NGit gitExe_Local_Clone(string repoName)
        {
            "\n\n    *** gitExe_Local_Clone  ***\n\n".info();            
            var repo_Clone_Name = repoName.append("_GitExe_Local").add_RandomLetters(3);
            var repo_Source     = TM_Libraries_Folder.pathCombine(repoName);
            return gitExe_Clone(repo_Source, repo_Clone_Name);
        }
        public API_NGit gitExe_GitHub_Clone(string repoName)
        {
             "\n\n    *** gitExe_GitHub_Clone  ***\n\n".info();                        
            
            var repo_Source     = GitHub_Repo_Path.format(repoName);
            var repo_Clone_Name = repoName.append("_GitExe_GitHub_").add_RandomLetters(3);
            
            return gitExe_Clone(repo_Source, repo_Clone_Name);
        }
        

        public API_NGit gitExe_Clone(string repo_Source, string repo_Clone_Name)
        {                        
            var repo_Clone  = Temp_Cloned_Repos.pathCombine(repo_Clone_Name);
            var gitCommand = "clone \"{0}\" \"{1}\"".format(repo_Source, repo_Clone_Name);

            Assert.IsFalse(repo_Clone.dirExists());
            
            gitExe_Execute(gitCommand, Temp_Cloned_Repos);
            
            Assert.IsTrue(repo_Clone.dirExists());
            
            return checkRepo(repo_Source,repo_Clone);
        }

        public string gitExe_Execute(string command, string workDirectory)
        {   
            //"[gitExe_Execute] executing command: {0}".debug(command);
            return GitExe_Path.startProcess_getConsoleOut(command,workDirectory).info();
        }

      


        // to delete


        public API_NGit local_Clone(string repoName)
        {
            var repo_Source = TM_Libraries_Folder.pathCombine(repoName);
            Assert.IsTrue   (repo_Source.dirExists());  
            Assert.IsTrue   (repo_Source.isGitRepository());   
           
            var repo_Clone  = Temp_Cloned_Repos.pathCombine(repoName.add_RandomLetters(3));
            
            Assert.IsFalse  (repo_Clone.dirExists());

            var nGit        = repo_Source.git_Clone(repo_Clone);

            Assert.IsTrue   (repo_Clone.dirExists());
            Assert.IsNotNull(nGit);
            Assert.IsTrue   (repo_Clone.isGitRepository());
            Assert.IsTrue   (nGit.commits_SHA1().notEmpty());
            Assert.IsTrue   (nGit.files().notEmpty());
            return nGit;
        }
                
            
        public API_NGit gitHub_Clone(string repoName)
        {
            if (Skip_Online_Tests)
            {
                "*** Skip_Online_Tests is set to true so Skipping GitHub clone".debug();
                return null;
            }
            var gitCloneUrl = GitHub_Repo_Path.format(repoName);
            var repo_Clone  = Temp_Cloned_Repos.pathCombine(repoName.append("_GitHub").add_RandomLetters(3));
            
            Assert.IsFalse  (repo_Clone.dirExists());

            var nGit        = gitCloneUrl.git_Clone(repo_Clone);
            Assert.IsTrue   (repo_Clone.dirExists());
            Assert.IsNotNull(nGit);
            return nGit;
        }


        
        
    }
}