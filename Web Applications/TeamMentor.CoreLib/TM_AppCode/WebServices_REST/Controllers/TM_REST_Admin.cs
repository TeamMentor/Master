using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
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
                if (what.contains("git"))
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

	}


}
	