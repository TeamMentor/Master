using System;
using System.IO;
using System.Security;
using System.Web;
using FluentSharp;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{	
    public partial class TM_REST
    {	
        public string Version()
        {			
            return this.type().Assembly.version();
        }		
        public string Admin_ReloadCache()
        {
            UserGroup.Admin.setThreadPrincipalWithRoles();
            var response = TmWebServices.XmlDatabase_ReloadData();
            UserGroup.Anonymous.setThreadPrincipalWithRoles();
            return response;
        }
        public string Admin_Restart()
        {
            "[TM_REST]: Received request to restart Application".debug();
            typeof(HttpRuntime).invokeStatic("ShutdownAppDomain", "");
            return "done";
        }        
        public string Admin_Logs()
        {
            return TmWebServices.GetLogs();	        
        }
        public string Admin_Logs_Reset()
        {
            return TmWebServices.ResetLogs();	        
        }
        
        public bool   SendEmail(EmailMessage_Post emailMessagePost)
        {
            return TmWebServices.SendEmail(emailMessagePost);            
        }
        
        public Stream TBot_Run(string what)
        {
            try
            {
                this.response_ContentType_Html();
                //if (what.lower().contains("git"))
                //    Admin_InvokeScript("load_NGit_Dlls");         // to solve prob with NGit dlls not being avaialble for compilation )
                TmWebServices.logUserActivity("Open TBot Page", what);
                return new TBot_Brain(this).Run(what);
            }
            catch (SecurityException)
            {
                TmWebServices.logUserActivity("Access Denied","TBot Page: {0}".format(what));
                Redirect_Login("/tbot");
            }	        
            return null;
        }

        public Stream TBot_Render(string what)
        {
            try
            {
                this.response_ContentType_Html();                
                return new TBot_Brain(this).Render(what);
            }
            catch (SecurityException)
            {
                return null;
            }	                    
        }
        public Stream TBot_Json(string what)
        {
            try
            {
                this.response_ContentType_Json();                
                return new TBot_Brain(this).Json(what);
            }
            catch (SecurityException)
            {
                return "{ 'error': 'SecurityException' }".stream_UFT8();    
            }	                    
        }

        
        public Stream TBot_List()
        {
            try
            {
                this.response_ContentType_Html();
                return new TBot_Brain(this).List();
            }
            catch (SecurityException)
            {
                Redirect_Login("/tbot");           
            }
            return null;
        }

        [Admin] public TM_SecretData Get_TM_SecretData()
        {
            return TM_UserData.Current.SecretData;
        }
        [Admin] public bool          Set_TM_SecretData(TM_SecretData tmSecretData)
        {
            try
            {
                if (tmSecretData.Rijndael_IV != TM_UserData.Current.SecretData.Rijndael_IV && 
                    tmSecretData.Rijndael_Key !=TM_UserData.Current.SecretData.Rijndael_Key)
                {
                    "[Set_TM_SecretData] both Rijndael_IV and Rijndael_Key are different from current value (not supported scenario and possible attack)".error();
                    return false;
                }
                TM_UserData.Current.SecretData = tmSecretData;
                TM_UserData.Current.secretData_Save();                
                return true;
            }
            catch (Exception ex)
            {
                ex.log("[Set_TM_SecretData]");
                return false;
            }
        }
        [Admin] public bool          Reload_UserData()
        {
            TM_UserData.Current.ReloadData();
            return true;
        }
        [Admin] public bool          Reload_TMConfig()
        {
            TMConfig.loadConfig();                                  // load default one
            TM_UserData.Current.handle_UserData_ConfigActions();    // load (if available) from current UserData location
            return true;
        }
        [Admin] public string        Reload_Cache()
        {
            return TmWebServices.XmlDatabase_ReloadData();	        
        }
        [Admin] public string        Get_GitUserConfig()
        {
            return TMConfig.Current.getGitUserConfigFile().fileContents();
        }
        [Admin] public bool          Set_GitUserConfig(string gitUserConfig_Data)
        {            
            return TMConfig.Current.setGitUserConfigFile(gitUserConfig_Data);   
        }
        [Admin] public string        FirstScript_FileContents()
        {
            return TM_UserData.Current.firstScript_FileLocation().fileContents();
        }
        [Admin] public string        FirstScript_Invoke()
        {
            return TM_UserData.Current.firstScript_Invoke();
        }

    }


}
    