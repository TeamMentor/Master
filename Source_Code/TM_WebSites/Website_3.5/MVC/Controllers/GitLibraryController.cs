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
using TeamMentor.FileStorage;
using TeamMentor.FileStorage.XmlDatabase;

namespace TeamMentor.Website.App_Code.TempControllers
{
    

    public class GitLibraryController : Controller
    {
        API_NGit nGit;

        public GitLibraryController()
        {
            if (TM_Xml_Database_Git.Current_Git.isNull())
                new TM_Xml_Database_Git().setupGitSupport();

            nGit = TM_Xml_Database_Git.Current_Git.NGits.first();
        }

        public ActionResult Index()
        {
            return Redirect("GitLibrary/Repository");
        }

        public ActionResult Repository()
        {            
            
            var viewGitRepo = new View_GitRepo()
                {
                    Name    = "UserData",
                    Path    = nGit.Path_Local_Repository,
                    Status  = nGit.status()                    
                };


            viewGitRepo.GitData  = getGitData(true);
            
            return View("~/Views/GitUserData/Repository.cshtml",viewGitRepo);
        }
        public ActionResult File(string path)
        {            
            if (path.isGuid())
            {
                path = TM_FileStorage.Current.xmlDB_guidanceItemPath(path.guid()).replace("\\","/");
            }
                
            var gitData = nGit.gitData_Repository();
            if(gitData.isNull())
                return View("~/Views/GitUserData/File.cshtml",new GitData_File());
            //var gitData = getGitData(nGit, true);
            var gitFile = (from file in gitData.Files where path.contains(file.FilePath) select file).first();
            if(gitFile.isNull())
                return View("~/Views/GitUserData/File.cshtml",new GitData_File());

            gitData.Files = gitFile.wrapOnList();
            gitData.map_File_Commits();
            return View("~/Views/GitUserData/File.cshtml",gitFile);
        }


        private GitData_Repository getGitData(bool clearCache)
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
            var viewFile = new View_GitFileContents()
                {
                    FilePath = path,
                    Sha1 = sha1,
                    Contents = nGit.open_Object(sha1).bytes().ascii()
                };
            return View("~/Views/GitUserData/FileContents.cshtml",viewFile);
        }
        public ActionResult FileDiff(string path, string fromSha1,string toSha1)
        {            

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
            return View("~/Views/GitUserData/FileDiff.cshtml",viewFile);
        }
    }
}