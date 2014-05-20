using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TM_Server
    {
        public bool          Create_Default_Admin_Account    { get; set; }
        public Config        UserData { get; set; }
        public Config        SiteData { get; set; }
        public List<GitRepo> UserData_Repos { get; set; }
        public List<GitRepo> SiteData_Repos { get; set; }

        public TM_Server()
        {
            this.resetData();
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