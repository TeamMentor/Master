using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_Server
    {
        public static string WebRoot { get; set; }
        public static string AppData_Folder { get; set; }

        
        public bool          Users_Create_Default_Admin     { get; set; }
        public bool          TM_Database_Use_AppData_Folder { get; set; }
        public Config        UserData { get; set; }
        public Config        SiteData { get; set; }
        public List<GitRepo> UserData_Repos { get; set; }
        public List<GitRepo> SiteData_Repos { get; set; }

        public TM_Server()
        {
            WebRoot = AppDomain.CurrentDomain.BaseDirectory;
            AppData_Folder = WebRoot.pathCombine("App_Data");
            this.setDefaultValues();            
        }


        public class Config
        {
            public string   Active_Repo_Name;
            public bool     Use_FileSystem;
            public bool     Enable_Git_Support;
        }         

        public class GitRepo
        {
            public string Name { get; set; }
            public string Local_GitPath { get; set; }
            public string Remote_GitPath { get; set; }
        }
    }

}