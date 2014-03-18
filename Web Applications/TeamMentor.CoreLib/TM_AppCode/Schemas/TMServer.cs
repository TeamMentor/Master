using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TMServer
    {
        public string                      ActiveRepo    { get; set; }
        public List<TMServer_UserDataRepo> UserDataRepos { get; set; }

        public TMServer()
        {
            UserDataRepos = new List<TMServer_UserDataRepo>();
        }
    }

    public class TMServer_UserDataRepo
    {
        public string Name { get; set; }
        public string GitPath { get; set; }
        public string Local_SHA1 { get; set; }
        public string Remote_SHA1 { get; set; }
    }
}