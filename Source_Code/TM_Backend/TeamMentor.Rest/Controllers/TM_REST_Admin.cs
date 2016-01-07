using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using FluentSharp.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{	
    public partial class TM_REST
    {	
        public string Version()
        {			
            return this.type().Assembly.version();
        }		
        [Assert_Admin]
        public string Admin_ReloadCache()
        {
            UserGroup.Admin.assert();                               // temp elevate privileges
            try
            {
                var response = TmWebServices.XmlDatabase_ReloadData();    
                return response;
            }
            finally
            {
                UserGroup.None.assert();
            }
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
                
                TmWebServices.logUserActivity("Open TBot Page", what);
                return new TBot_Brain(this).Run(what);
            }
            catch (SecurityException)
            {
                TmWebServices.logUserActivity("Access Denied","TBot Page (Run): {0}".format(what));                
                return Redirect_Login("/tbot");
            }	        
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
                TmWebServices.logUserActivity("Access Denied","TBot Page (Render): {0}".format(what));
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
                TmWebServices.logUserActivity("Access Denied","TBot Page (Json): {0}".format(what));
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
                TmWebServices.logUserActivity("Access Denied","TBot List Command");
                return Redirect_Login("/tbot");           
            }            
        }

        [Admin] public TM_SecretData Get_TM_SecretData()
        {
            UserRole.Admin.demand();
            return TM_UserData.Current.SecretData;
        }
        [Admin] public bool          Set_TM_SecretData(TM_SecretData tmSecretData)
        {
            UserRole.Admin.demand();
            try
            {
                if (tmSecretData.Rijndael_IV != TM_UserData.Current.SecretData.Rijndael_IV && 
                    tmSecretData.Rijndael_Key !=TM_UserData.Current.SecretData.Rijndael_Key)
                {
                    "[Set_TM_SecretData] both Rijndael_IV and Rijndael_Key are different from current value (not supported scenario and possible attack)".error();
                    return false;
                }
                TM_UserData.Current.SecretData = tmSecretData;
                return TM_FileStorage.Current.secretData_Save();       //TM_UserData.Current.secretData_Save();                                
            }
            catch (Exception ex)
            {
                ex.log("[Set_TM_SecretData]");
                return false;
            }
        }
        [Admin] public bool          Reload_UserData()
        {
            UserRole.Admin.demand();
            TM_FileStorage.Current.load_UserData();            
            return true;
        }
        [Admin] public bool          Reload_TMConfig()
        {
            UserRole.Admin.demand();
            TM_FileStorage.Current.tmConfig_Reload();
            return true;
        }
        [Admin] public string        Reload_Cache()
        {
            UserRole.Admin.demand();
            return TmWebServices.XmlDatabase_ReloadData();	        
        }

        [Admin]
        public bool Publish_Data()
        {
            UserRole.Admin.demand();
            if (TMConfig.Current.notNull())
            {
                var scriptPath = TMConfig.Current.TMSetup.TMReloadDataScriptPath;
                if (!scriptPath.fileExists())
                {
                    "[Publishing Data]You need to set the location of the bat file".log();
                    return false;
                }
                var workingDirectory = Path.GetDirectoryName(scriptPath);
                var arguments        = String.Empty;
                
                 //FluentSharp method.
                 var outputLog = scriptPath.startProcess_getConsoleOut(arguments, workingDirectory);
                 //Login output
                 outputLog.log();

                 return outputLog.contains("... Data has been published");
            }
            return false;
        }

        /*[Admin] public string        Get_GitUserConfig()
        {            
            return TM_Xml_Database.Current.getGitUserConfigFile().fileContents();
        }
        [Admin] public bool          Set_GitUserConfig(string gitUserConfig_Data)
        {            
            return  TM_Xml_Database.Current.setGitUserConfigFile(gitUserConfig_Data);               
        }*/
            /*[Admin] public string        FirstScript_FileContents()
        {
            return TM_UserData.Current.firstScript_FileLocation().fileContents();
        }
        [Admin] public string        FirstScript_Invoke()
        {
            return TM_UserData.Current.firstScript_Invoke();
        }*/

        }


}
    