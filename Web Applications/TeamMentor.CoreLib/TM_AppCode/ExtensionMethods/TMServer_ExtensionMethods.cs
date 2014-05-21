using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TMServer_ExtensionMethods
    {
        public static TM_Server setDefaultValues(this TM_Server tmServer)
        {
            tmServer.Users_Create_Default_Admin     = true;
            tmServer.TM_Database_Use_AppData_Folder = false;

            tmServer.UserData_Repos = new List<TM_Server.GitRepo>();
            tmServer.SiteData_Repos = new List<TM_Server.GitRepo>();

            tmServer.UserData = new TM_Server.Config
                                        {
                                            Active_Repo_Name    = TMConsts.TM_SERVER_DEFAULT_NAME_USERDATA,
                                            Use_FileSystem      = false,
                                            Enable_Git_Support  = false
                                        };
            tmServer.SiteData = new TM_Server.Config
                                        {
                                            Active_Repo_Name    = TMConsts.TM_SERVER_DEFAULT_NAME_SITEDATA,
                                            Use_FileSystem      = false,
                                            Enable_Git_Support  = false
                                        };

            return tmServer;
            //Active_Repo_Name = ;
        }

        //TM_Server
        public static TM_Server add_UserData_Repo(this TM_Server tmServer, TM_Server.GitRepo userData_GitRepo)
        {
            if (tmServer.notNull() && userData_GitRepo.notNull())
            {
                var existingGitRepo = tmServer.find_UserData_Repo(userData_GitRepo.Name);
                if (existingGitRepo.notNull())                                    // if it already exists, remove it (before adding)
                    tmServer.UserData_Repos.remove(existingGitRepo);
                tmServer.UserData_Repos.add(userData_GitRepo);
            }
            return tmServer;
        }
        
        public static string getActive_UserData_Remote_Repo_GitPath(this TM_Server tmServer)
        {
            if (tmServer.isNull())
                return null;
            var activeRepo = tmServer.getActive_UserData_Repo();
            if (activeRepo.notNull())
                return activeRepo.Remote_GitPath;
            return null;
        }

        public static TM_Server.GitRepo getActive_UserData_Repo(this TM_Server tmServer)
        {
            return tmServer.find_UserData_Repo(tmServer.UserData.Active_Repo_Name);
        }
        public static TM_Server.GitRepo find_UserData_Repo(this TM_Server tmServer, string name)
        {
            return tmServer.UserData_Repos.where(repo => repo.Name == name).first();
        }

        public static TM_Server setActive_UserData_Rep(this TM_Server tmServer, TM_Server.GitRepo gitRepo)
        {
            if (tmServer.UserData_Repos.contains(gitRepo).isFalse())
                tmServer.UserData_Repos.add(gitRepo);
            tmServer.UserData.Active_Repo_Name = gitRepo.Name;
            return tmServer;
        }

   
        // for TM_Xml_Database
        public static TM_Server tmServer(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.isNull())
                return null;
            return tmDatabase.Server.notNull()
                    ? tmDatabase.Server
                    : tmDatabase.load_TMServer_Config();
        }
       
        public static TM_Server load_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            tmDatabase.Server = new TM_Server();
            if (tmDatabase.UsingFileStorage)
            {
                var tmServerFile = tmDatabase.get_Path_TMServer_Config();
                if (tmServerFile.valid())
                {
                    if (tmServerFile.fileExists().isFalse())
                    {
                        "[TM_Xml_Database][load_TMServer_Config] expected TM_Server file didn't exist, so creating it: {0}".info(tmServerFile);
                        new TM_Server().saveAs(tmServerFile);
                    }
                    var tmServer = tmServerFile.load<TM_Server>();
                    if (tmServer.isNull())
                        "[TM_Xml_Database][load_TMServer_Config] Failed to load tmServer file: {0}   Default values will be used".error(tmServerFile);
                    else                    
                        tmDatabase.Server = tmServer;
                }
            }
                
            return tmDatabase.Server;
        }
        public static bool save_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            try
            {
                if (tmDatabase.UsingFileStorage)
                {
                    var tmServerFile = tmDatabase.get_Path_TMServer_Config();
                    if (tmServerFile.valid())
                        tmDatabase.Server.saveAs(tmServerFile);
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.log("in save_TMServer_Config");
                return false;
            }            
        }
    }
}