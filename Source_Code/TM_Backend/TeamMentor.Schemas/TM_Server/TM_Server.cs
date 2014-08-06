using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    
    public class TM_Server : MarshalByRefObject
    {
        public static TM_Server Current                        { get; set; }
        //public bool             Users_Create_Default_Admin     { get; set; }
        //public bool             TM_Database_Use_AppData_Folder { get; set; }
        public bool             UserActivities_Disable_Logging { get; set; }
        public bool             RealTime_Logs                  { get; set; }
        public Git_Config    Git              { get; set; }
        public List<Config>  UserData_Configs { get; set; }
        public List<Config>  SiteData_Configs { get; set; }
        
        public TM_Server()
        {          
            Current = this;

            //Users_Create_Default_Admin      = true;
            //TM_Database_Use_AppData_Folder  = false;
            UserActivities_Disable_Logging  = false;
            RealTime_Logs                   = false;

            Git = new Git_Config();
            UserData_Configs = new List<Config>();
            SiteData_Configs = new List<Config>();        
        }
          
        
        public class Config : MarshalByRefObject
        {         
            public string   Name                    { get; set; }
            public bool     Active                  { get; set; }
//            public bool     Use_FileSystem          { get; set; }
//            public bool     Enable_Git_Support      { get; set; }            
//            public string   Local_GitPath           { get; set; }
            public string   Remote_GitPath          { get; set; }
        }
        
        public class Git_Config : MarshalByRefObject
        {
            public bool UserData_Git_Enabled { get; set; }
            public bool UserData_Auto_Pull { get; set; }
            public bool UserData_Auto_Push { get; set; }
            public bool LibraryData_Git_Enabled { get; set; }
            public bool LibraryData_Auto_Pull { get; set; }
            public bool LibraryData_Auto_Push { get; set; }

            public Git_Config()
            {
                LibraryData_Git_Enabled = true;                 // all user and library data should be controled by Git
                UserData_Git_Enabled = true;
                LibraryData_Auto_Pull = true;                 // pull is automatic (or changed on TMConfig file                
                UserData_Auto_Pull = true;
                LibraryData_Auto_Push = false;                // push must be set on the TM config
                UserData_Auto_Push = false;
            }
        }
    }

}