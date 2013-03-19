using System;
using System.Collections.Generic;
using System.Threading;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp;

namespace TeamMentor.CoreLib
{
    public class TM_UserData
    {
        public static TM_UserData       Current             { get; set; }
        public static Thread            GitPushThread       { get; set; }

        public string 	                Path_UserData 	    { get; set; }	
        public string 	                Path_UserData_Base 	{ get; set; }
        public string 	                Git_UserData 	    { get; set; }
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }
        
        //public Dictionary<Guid, TMUser>	ActiveSessions	    { get; set; }
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
            //ActiveSessions      = new Dictionary<Guid, TMUser>();
            SecretData          = new TM_SecretData();
            AutoGitCommit       = TMConfig.Current.Git.AutoCommit_UserData;           
            return this;
        }

        public TM_UserData SetUp()
        {
        /*    return SetUp(true);
        }
        public TM_UserData SetUp(bool createDefaultAdminUser)
        {*/
            try
            {
                this.setupGitSupport();
                this.SecretData = this.secretData_Load();
                
                //this.secretDataScript_Invoke();
            
            
                //if (createDefaultAdminUser)
                //   this.createDefaultAdminUser();  // make sure the admin user exists and is configured                            
            }
            catch (Exception ex)
            {
                ex.log("[In TM_UserData SetUp]");
            }            
            return this;
        }

        public TM_UserData ReloadData()
        {
            this.SetUp();
            this.loadTmUserData();
            this.createDefaultAdminUser();  // make sure the admin user exists and is configured
            return this;
        }
    }
}
