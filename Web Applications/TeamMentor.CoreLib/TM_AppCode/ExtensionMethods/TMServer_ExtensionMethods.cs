using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TMServer_ExtensionMethods
    {
        //TM_Server
        public static string getActive_UserData_Repo_GitPath(this TM_Server tmServer)
        {
            if (tmServer.isNull())
                return null;
            var activeRepo = tmServer.getActive_UserData_Rep();
            if (activeRepo.notNull())
                return activeRepo.GitPath;
            return null;
        }

        public static TM_Server.UserDataRepo getActive_UserData_Rep(this TM_Server tmServer)
        {
            return tmServer.UserDataRepos.where(repo => repo.Name == tmServer.ActiveRepo).first();
        }

        public static TM_Server setActive_UserData_Rep(this TM_Server tmServer, TM_Server.UserDataRepo userDataRepo)
        {
            if (tmServer.UserDataRepos.contains(userDataRepo).isFalse())
                tmServer.UserDataRepos.add(userDataRepo);
            tmServer.ActiveRepo = userDataRepo.Name;
            return tmServer;
        }

   
        // for TM_Xml_Database
        public static TM_Server tmServer(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.isNull())
                return null;
            return tmDatabase.TM_Server_Config.notNull()
                    ? tmDatabase.TM_Server_Config
                    : tmDatabase.load_TMServer_Config();
        }

        public static string get_Path_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            return (tmDatabase.notNull() && tmDatabase.UsingFileStorage)
                        ? tmDatabase.Path_XmlDatabase.pathCombine(TMConsts.TM_SERVER_FILENAME)
                        : null;            
        }
        public static TM_Server load_TMServer_Config(this TM_Xml_Database tmDatabase)
        {
            if (tmDatabase.UsingFileStorage)
            {
                var tmServerFile = tmDatabase.get_Path_TMServer_Config();
                if (tmServerFile.valid())
                {
                    if (tmServerFile.fileExists().isFalse())
                        new TM_Server().saveAs(tmServerFile);
                    
                    tmDatabase.TM_Server_Config = tmServerFile.load<TM_Server>();
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