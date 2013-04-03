using System;
using System.IO;
using System.Security;
using System.Web;
using FluentSharp;
using O2.DotNetWrappers.ExtensionMethods;

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
        public string Admin_InvokeScript(string scriptName)
        {
            var method = typeof (O2_Script_Library).method(scriptName);
            if (method.isNull())
                return "script not found";
            try
            {
                var returnValue = method.invokeStatic().str().compileAndExecuteCodeSnippet();
                return returnValue.str();
            }
            catch (Exception ex)
            {
                ex.log("[Admin_InvokeScript] for script:" + scriptName);
                return "script failed to execute";
            }
            
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
        public Stream TBot_Show()
        {
            try
            {
                this.response_ContentType_Html();
                return new TBot_Brain(this).RenderPage();
            }
            catch (SecurityException)
            {
                Redirect_Login("/tbot");
            }	        
            return null;
        }
        public Stream TBot_Run(string what)
        {
            try
            {
                this.response_ContentType_Html();
                if (what.lower().contains("git"))
                    Admin_InvokeScript("load_NGit_Dlls");         // to solve prob with NGit dlls not being avaialble for compilation )
                return new TBot_Brain(this).Run(what);
            }
            catch (SecurityException)
            {
                Redirect_Login("/tbot");
            }	        
            return null;
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
    