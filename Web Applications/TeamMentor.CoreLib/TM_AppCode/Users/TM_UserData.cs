using System;
using System.Collections.Generic;
using System.Threading;
using FluentSharp.CoreLib;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public class TM_UserData
    {
        public static TM_UserData       Current             { get; set; }
        public static Thread            GitPushThread       { get; set; }

        public string 	                Path_UserData 	    { get; set; }	
        public string 	                Path_UserData_Base 	{ get; set; }
        public string 	                Path_WebRootFiles   { get; set; }
        public string                   FirstScriptToInvoke { get; set; }        
        public List<TMUser>	            TMUsers			    { get; set; }
        public TM_SecretData            SecretData          { get; set; }
        
        //public Dictionary<Guid, TMUser>	ActiveSessions	    { get; set; }
        public bool                     UsingFileStorage    { get; set; }                
        public API_NGit                 NGit                { get; set; }
        public string                   NGit_Author_Name    { get; set; } 
        public string                   NGit_Author_Email   { get; set; }
        
        
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
            NGit_Author_Name    = "tm-bot";
            NGit_Author_Email   = "tm-bot@teammentor.net";
            FirstScriptToInvoke = "H2Scripts//FirstScriptToInvoke.h2";
            Path_WebRootFiles   = "WebRoot_Files";
            TMUsers             = new List<TMUser>();                        
            SecretData          = new TM_SecretData();            
            return this;
        }

        public TM_UserData SetUp()
        {
            try
            {                
                this.setupGitSupportAndLoadTMConfigFile();
                this.firstScript_Invoke();                
                SecretData = this.secretData_Load();
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
