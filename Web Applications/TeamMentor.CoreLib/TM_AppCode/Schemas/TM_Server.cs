using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TM_Server
    {
        public string                      ActiveRepo    { get; set; }
        public List<UserDataRepo>          UserDataRepos { get; set; }

        public TM_Server()
        {
            UserDataRepos = new List<UserDataRepo>();
        }

        public class UserDataRepo
        {
            public string Name { get; set; }
            public string GitPath { get; set; }
            public string Local_SHA1 { get; set; }
            public string Remote_SHA1 { get; set; }
        }
    }

}