using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TMServer_ExtMet
    {
        

        public static string getActive_UserData_Repo_GitPath(this TMServer tmServer)
        {
            var activeRepo = tmServer.getActive_UserData_Rep();
            if (activeRepo.notNull())
                return activeRepo.GitPath;
            return null;
        }

        public static TMServer_UserDataRepo getActive_UserData_Rep(this TMServer tmServer)
        {
            return tmServer.UserDataRepos.where(repo => repo.Name == tmServer.ActiveRepo).first();
        }

        public static TMServer setActive_UserData_Rep(this TMServer tmServer, TMServer_UserDataRepo userDataRepo)
        {
            if (tmServer.UserDataRepos.contains(userDataRepo).isFalse())
                tmServer.UserDataRepos.add(userDataRepo);
            tmServer.ActiveRepo = userDataRepo.Name;
            return tmServer;
        }

        /*public static bool setGitUserConfigFile(this TM_Xml_Database tmDatabase, string gitUserConfig_Data)
        {
            try
            {
                var gitUserConfigFile = tmDatabase.getGitUserConfigFile();
                if (gitUserConfig_Data.notValid() && gitUserConfigFile.fileExists())
                {
                    "[setGitUserConfigFile] Deleting current gitUserconfigFile: {0}".info(gitUserConfigFile);
                    gitUserConfigFile.file_Delete();
                }
                else
                    gitUserConfig_Data.saveAs(gitUserConfigFile);
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[setGitUserConfigFile]");
                return false;
            }
        }*/
    }

    public static class TMServer_ExtMet_TM_Xml_Database
    {
        public static string get_Path_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            return (tmDatabase.notNull() && tmDatabase.UsingFileStorage)
                        ? tmDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME)
                        : null;            
        }
        public static TMServer load_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
            {
                var tmServerFile = tmDatabase.get_Path_TMServer_Config();
                if (tmServerFile.valid())
                {
                    if (tmServerFile.fileExists().isFalse())
                        new TMServer().saveAs(tmServerFile);
                    
                    tmDatabase.TM_Server_Config = tmServerFile.load<TMServer>();
                }
            }
            return tmDatabase.TM_Server_Config;
        }
        public static bool save_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            try
            {
                if (tmDatabase.UsingFileStorage)
                {
                    var tmServerFile = tmDatabase.get_Path_TMServer_Config();
                    if (tmServerFile.valid())
                        tmDatabase.TM_Server_Config.saveAs(tmServerFile);
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