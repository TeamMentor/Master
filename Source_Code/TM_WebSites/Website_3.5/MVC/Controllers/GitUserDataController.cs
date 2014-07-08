using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentSharp.CoreLib;
using FluentSharp.Git.Utils;
using NGit;
using NGit.Diff;
using NGit.Revwalk;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;
using TeamMentor.CoreLib;
using FluentSharp.Git.APIs;
using FluentSharp.Git;
namespace TeamMentor.Website.App_Code.TempControllers
{
    /*public class View_GitFile
    {        
        public string               FilePath    { get; set; }           
        public GitData_File         File 
        View_GitFile
    }*/
    public class View_GitFileContents
    {        
        public string               FilePath    { get; set; }           
        public string               Sha1        { get; set; } 
        public string               Contents    { get ;set; }
    }
    public class View_GitFileDiff
    {        
        public string               FilePath    { get; set; }           
        public string               FromSha1    { get; set; } 
        public string               ToSha1      { get; set; } 
        public string               Diff        { get ;set; }
    }
    public class View_GitRepo
    {
        public string               Name        { get; set; }   
        public string               Path        { get; set; }   
        public string               Status      { get; set; }   
        public GitData_Repository   GitData     { get; set; }   
    }

    public class GitUserDataController : Controller
    {        
        public ActionResult Index()
        {
            return Redirect("GitUserData/Repository");
        }

        public ActionResult Repository()
        {
            var nGit = TM_UserData_Git.Current.NGit;
            
            var viewGitRepo = new View_GitRepo()
                {
                    Name    = "UserData",
                    Path    = nGit.Path_Local_Repository,
                    Status  = nGit.status()                    
                };


            viewGitRepo.GitData  = getGitData(nGit,true);
            
            return View(viewGitRepo);
        }
        public ActionResult File(string path)
        {   
            var nGit = TM_UserData_Git.Current.NGit;
            var gitData = nGit.gitData_Repository();

            //var gitData = getGitData(nGit, true);
            var gitFile = (from file in gitData.Files where file.FilePath == path select file).first();
            gitData.Files = gitFile.wrapOnList();
            gitData.map_File_Commits();
            return View(gitFile);
        }


        private GitData_Repository getGitData(API_NGit nGit, bool clearCache)
        {
            if (clearCache)
                "gitData".o2Cache(null);
             return "gitData".o2Cache(()=>
                        {
                                var gitData = nGit.gitData_Repository();
                                gitData.map_File_Commits();
                            return gitData;
                        });
        }
        public ActionResult FileContents(string path, string sha1)
        {
            var nGit = TM_UserData_Git.Current.NGit;
            var viewFile = new View_GitFileContents()
                {
                    FilePath = path,
                    Sha1 = sha1,
                    Contents = nGit.open_Object(sha1).bytes().ascii()
                };
            return View(viewFile);
        }
        public ActionResult FileDiff(string path, string fromSha1,string toSha1)
        {
            var nGit = TM_UserData_Git.Current.NGit;

            Func<Repository, string, string, string, string> getDiff =
                (gitRepo, repoPath, fromCommitId, toCommitId) =>
                    {

                        var fromCommit = gitRepo.Resolve(fromCommitId);
                        var toCommit = gitRepo.Resolve(toCommitId);

                        var outputStream = "Sharpen.dll".assembly().type("ByteArrayOutputStream").ctor(new object[0]).cast<OutputStream>();
                        //return "diffing from {0} to  {1}".format(fromCommit, toCommit);
                        var diffFormater = new DiffFormatter(outputStream);
                        var pathFilter = PathFilter.Create(repoPath);
                        diffFormater.SetRepository(gitRepo);
                        diffFormater.SetPathFilter(pathFilter);
                        //diffFormater.Format(refLog.GetNewId(), refLog.GetOldId());
                        diffFormater.Format(fromCommit, toCommit);
                        return "result: " + outputStream.str();
                    };

            Func<Repository, string, string, string> getFistValue =
                (gitRepo, commitSha1, repoPath) =>
                    {
                        var revCommit = nGit.commit(commitSha1);
                        var outputStream = "Sharpen.dll".assembly().type("ByteArrayOutputStream").ctor(new object[0]).cast<OutputStream>();
                        var diffFormater = new DiffFormatter(outputStream);                
                        var pathFilter = PathFilter.Create(repoPath);
                        diffFormater.SetRepository(gitRepo);
                        diffFormater.SetPathFilter(pathFilter);
                
                        var revWalk = new RevWalk(gitRepo);
                        var canonicalTreeParser = new CanonicalTreeParser(null, revWalk.GetObjectReader(), revCommit.Tree);
                        diffFormater.Format(new EmptyTreeIterator(), canonicalTreeParser);
                        return outputStream.str().fix_CRLF();                
                    };
            
            var rawDiff = fromSha1 == NGit_Consts.EMPTY_SHA1 
                            ? getFistValue(nGit.repository(), fromSha1, path) 
                            :  getDiff(nGit.repository(), path, fromSha1, toSha1);


            var viewFile = new View_GitFileDiff()
                {
                    FilePath = path,
                    FromSha1 = fromSha1,
                    ToSha1 = toSha1,
                    Diff = rawDiff
                };
            return View(viewFile);
        }
    }
}