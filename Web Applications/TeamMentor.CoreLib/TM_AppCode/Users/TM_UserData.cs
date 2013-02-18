using System;
using System.Collections.Generic;
using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
    public class TM_UserData
    {
        public static TM_UserData       Current             { get; set; }
        public string 	                Path_UserData 	    { get; set; }	
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }
        
        public Dictionary<Guid, TMUser>	ActiveSessions	    { get; set; }
        public bool                     UsingFileStorage    { get; set; }
        public bool                     AutoGitCommit       { get; set; }
        public API_NGit                 NGit                { get; set; }
        
        
        public TM_UserData() : this (false)
        {
        }

        public TM_UserData(bool useFileStorage)
        {
            Current = this;
            UsingFileStorage = useFileStorage;
            ResetData();
        }

        public TM_UserData ResetData()
        {
            TMUsers             = new List<TMUser>();            
            ActiveSessions      = new Dictionary<Guid, TMUser>();
            SecretData          = this.secretData_Load();
            AutoGitCommit       = TMConfig.Current.Git.AutoCommit_UserData;
            return this;
        }

        public TM_UserData SetUp()
        {
            return SetUp(true);
        }
        public TM_UserData SetUp(bool createDefaultAdminUser)
        {   
            this.setupGitSupport();
            if (createDefaultAdminUser)
                this.createDefaultAdminUser();  // make sure the admin user exists and is configured            
            return this;
        }
    }
}
