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

	    public Stream TBot_Show()
	    {
	        try
	        {
                this.response_ContentType_Html();
	            return new TBot_Brain().RenderPage();
	        }
	        catch(SecurityException)
	        {              
	            Redirect_Login("/tbot");	            	            
	        }	       
            return null;
	    }

	    public Stream TBot_Run(string what)
	    {
            this.response_ContentType_Html();
            return new TBot_Brain().Run(what);
	    }

        public Stream TBot_List()
	    {
            try
            {
                this.response_ContentType_Html();
                return new TBot_Brain().List();
            }
	        catch (SecurityException)
	        {
                Redirect_Login("/tbot");           
	        }
            return null;
	    }

	}


}
	