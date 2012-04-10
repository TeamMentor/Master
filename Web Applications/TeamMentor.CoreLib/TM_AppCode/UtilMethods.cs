using System;
using System.Web;
using System.Xml;
using System.Data;
using System.Text;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Xml.Linq;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
//O2Ref:System.Web.Abstractions.dll
//O2File:TmConfig.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public class UtilMethods
    {    	
        public static string syncWithGitHub_Pull_Origin()
		{
            var gitCommand = "pull origin";
            return executeGitCommand(gitCommand);
        }

        public static string syncWithGitHub_Push_Origin()
		{
            var gitCommand = "push origin";
            return executeGitCommand(gitCommand);
        }

        public static string syncWithGitHub_Commit()
        {
            return syncWithGitHub_Commit("TeamMentor Commit at: {0}".format(DateTime.Now));
        }

        public static string syncWithGitHub_Commit(string message)
		{
            executeGitCommand("add -A");       
            var commit = "commit -m '{0}'".format(message);
            return executeGitCommand(commit);
        }

        public static string executeGitCommand(string gitCommand)
        {                   
			var gitExe = @"C:\Program Files\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				gitExe = @"C:\Program Files (x86)\Git\bin\git.exe";
			if (gitExe.fileExists().isFalse())
				return "error: could not find git.exe: {0}".format(gitExe);

			var gitLocalProjectFolder = AppDomain.CurrentDomain.BaseDirectory.pathCombine("..//..").fullPath(); // go up to the main git folder
	 
			var cmdOutput = Processes.startAsCmdExe(gitExe, gitCommand, gitLocalProjectFolder) 
									 .fixCRLF()
                                     .replace("\t", "    ");
			return cmdOutput;
		}
    }
	
	public static class SoapRequestUtils
    {
        public static bool RequestHasSoapAction(this HttpContextBase httpContext)
        {
            return httpContext.Request.ServerVariables["HTTP_SOAPACTION"] != null;
        }

        public static XmlDocument GetPostDataAsXmlDocument(this HttpContextBase httpContext)
        {
            try
            {
                var httpStream = httpContext.Request.InputStream;
                var posStream = httpStream.Position;
                var xmlDocument = new XmlDocument();                
                xmlDocument.Load(httpStream);
                httpStream.Position = posStream;
                return xmlDocument;
            }
            catch
            {
                return null;
            }            
        }
		
		public static String GetPostDataAsString(this HttpContextBase httpContext)
        {	
			try
            {
                var httpStream = httpContext.Request.InputStream;
                var posStream = httpStream.Position;
				httpStream.Position = 0;
                var reader = new System.IO.StreamReader(httpStream);
				var postData = reader.ReadToEnd();
                httpStream.Position = posStream;
                return postData;
            }
            catch
            {
                return null;
            }         
        }
	} 

    public static class Misc_ExtensionMethods
    {
    	//move this to a generic helper classes
    	public static string createPasswordHash(this string username, string password)
		{			
			var stringToHash = username+password;
			var sha256 = System.Security.Cryptography.SHA256.Create();
			var hashBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(stringToHash));
			var hashString = new StringBuilder();	
			foreach (byte b in hashBytes)		
				hashString.Append(b.ToString("x2"));					
			return hashString.ToString();
		}
		
		public static List<string> toStringList(this List<Guid> guids)
		{
			return (from guid in guids
					select guid.str()).toList();
		}		
    }
	
    public class HttpContextFactory
    {
        public static HttpContextBase Context { get; set;}

        public static HttpContextBase Current
        {
            get
            {
                if (Context != null)
                    return Context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
        }
        
    }

}

