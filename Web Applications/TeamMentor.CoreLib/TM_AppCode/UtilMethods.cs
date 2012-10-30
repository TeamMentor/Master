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
using System.Globalization;
using System.IO.Compression;
using System.Threading;
using O2.DotNetWrappers.DotNet;
using System.Web.Script.Serialization;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using System.Security.Permissions;
//O2Ref:System.Web.Abstractions.dll
//O2File:TmConfig.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public class UtilMethods
    {    	
		//GIT
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


		//GZIP
		public static void setGZipCompression_forAjaxRequests()
		{
			setGZipCompression_forAjaxRequests(HttpContext.Current.Request, HttpContext.Current.Response);
		}
		public static void setGZipCompression_forAjaxRequests(HttpRequest request, HttpResponse response)  //based on code from http://geekswithblogs.net/rashid/archive/2007/09/15/Compress-Asp.net-Ajax-Web-Service-Response---Save-Bandwidth.aspx
		{
			if (TMConfig.Current.TMDebugAndDev.EnableGZipForWebServices.isFalse())
				return;			
			try
			{
				if (request.ContentType.lower().starts(new List<string>() { "text/xml", "application/json" }))
				{
					string acceptEncoding = request.Headers["Accept-Encoding"];

					if (!string.IsNullOrEmpty(acceptEncoding))
					{
						acceptEncoding = acceptEncoding.ToLower(CultureInfo.InvariantCulture);

						if (acceptEncoding.Contains("gzip"))
						{
							response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
							response.AddHeader("Content-encoding", "gzip");
						}
						else if (acceptEncoding.Contains("deflate"))
						{
							response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
							response.AddHeader("Content-encoding", "deflate");
						}
					}
				}
				
			}
			catch (Exception ex)
			{
				ex.log("in enableGZipCompression_forAjaxRequests");
			}
		}

		//RESPONSE
		public static void addDefaultRequestHeaders()
		{
			//add clickjacking protection
			//HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");   //this broken the openUrl GUI funcionality
			HttpContext.Current.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
			//IE AntiXSS projecttion
			HttpContext.Current.Response.AddHeader("X-XSS-Protection", "1; mode=block");			
			
		}
		
	}

	public class REPL
	{
		//public static List<Thread> ExecutionThreads = new List<Thread>();		
		public static int MAX_EXECUTION_TIME = 20000;
		
		[Admin(SecurityAction.Demand)]
		public static string executeSnippet(string snippet)
		{
			"[REPL] executing snippet with size: {0}".info(snippet.size());
			object executionResult = "";
			var compileError = "";
			Action<string> onCompileOk = (msg) => { };
			Action<string> onCompileFail = (msg) => { compileError = msg; };
			var result = snippet.fixCRLF().compileAndExecuteCodeSnippet(onCompileOk, onCompileFail);
			if (compileError.valid())
				executionResult = compileError;
			else
				executionResult = result.notNull() ? result : "";

			if (executionResult is string)
				return (string)executionResult;
			try
			{
				return new JavaScriptSerializer().Serialize(executionResult);
			}
			catch { }
						
			return executionResult.str();			
		}

		public static string executeSnippet_SeparateThread(string snippet)
		{			
			var executionResult = "";			
			var sync = new AutoResetEvent(false);
			var thread = O2Thread.mtaThread(
							()=>{
								executionResult = executeSnippet(snippet);
									sync.Set();
								});

			if (thread.Join(MAX_EXECUTION_TIME).isFalse())
			{
				"[REPL] Execution timeout reached".error();
				return "Error: Snippet execution timed out";
			}
			return executionResult;
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

	public class KeyValue<TKey, TValue>
	{
		public TKey Key { get; set; }
		public TValue Value { get; set; }

		public KeyValue()
		{
		}
	}
	public static class KeyValue_extensionMethods
	{
		public static List<KeyValue<TKey, TValue>> ConvertDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{

			try
			{
				var keyValueList = new List<KeyValue<TKey, TValue>>();
				foreach (TKey key in dictionary.Keys)
				{
					keyValueList.Add(new KeyValue<TKey, TValue> { Key = key, Value = dictionary[key] });
				}
				return keyValueList;
			}
			catch (Exception ex)
			{
				ex.logWithStackTrace("in ConvertDictionary");
				return new List<KeyValue<TKey, TValue>>(); 
			}
		}
	}
}

