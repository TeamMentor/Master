//based on code from http://atashbahar.com/post/Combine-minify-compress-JavaScript-files-to-load-ASPNET-pages-faster.aspx
using System;
using System.IO;

using System.Text;
using System.Web;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using XssEncoder = Microsoft.Security.Application.Encoder;

namespace TeamMentor.CoreLib
{
    
	public class ScriptCombiner : IHttpHandler
	{    
			
		public HttpContextBase context;
	    public API_ScriptCombiner apiScriptCombiner;

        public ScriptCombiner()
        { 
           apiScriptCombiner = new API_ScriptCombiner();            
        }
		public void ProcessRequest(HttpContext httpContext)
		{				
			context = HttpContextFactory.Current;		
            apiScriptCombiner.baseFolder = context.Server.MapPath("/");
			var request = context.Request;        
			var response = context.Response;		
			response.Clear();
            
            if (TMConfig.Current.enable304Redirects() && context.sent304Redirect())			
                return;
			try
			{
                

				apiScriptCombiner.minifyCode = true;
				apiScriptCombiner.ignoreCache = true;
				if (request.QueryString["Hello"]=="TM")
				{
					response.Write("Good Morning");
					return;
				}
                
				// Read setName, version from query string
				apiScriptCombiner.setName = XssEncoder.UrlEncode(request.QueryString["s"]) ?? string.Empty;
				apiScriptCombiner.version = XssEncoder.UrlEncode(request.QueryString["v"]) ?? string.Empty;
						
				if (apiScriptCombiner.setName ==string.Empty)
				{
					response.Write("//nothing to do");
					return;
				}			
			
				if (request.QueryString["dontMinify"] == "true")
					apiScriptCombiner.minifyCode = false;

				switch(request.QueryString["ct"])
				{
					case "css": 
						apiScriptCombiner.contentType = "text/css";
						apiScriptCombiner.minifyCode = false;
						break;
					default:
						apiScriptCombiner.contentType = "application/x-javascript";
						break;
				}
				// Decide if browser supports compressed response
				apiScriptCombiner.isCompressed = CanGZip(context.Request);
                
                apiScriptCombiner.CombineFiles();
				var responseBytes = apiScriptCombiner.CombinedBytes;
                var isCompressed  = apiScriptCombiner.isCompressed;
                var contentType   = apiScriptCombiner.contentType;
			    WriteBytes(responseBytes, isCompressed,contentType);                
			}
			catch(Exception ex)
			{
				ex.log();
				response.Write("//Error processing request"+  ex.Message);
				response.End();
			}		
		}	

		public void WriteBytes(byte[] bytes, bool isCompressed, string contentType)
		{
			var response = context.Response;

			response.AppendHeader("Content-Length", bytes.Length.str());
		
			response.ContentType = contentType;

		    response.AppendHeader("Content-Encoding", isCompressed ? "gzip" : "utf-8");

		    response.ContentEncoding = Encoding.Unicode;
			response.OutputStream.Write(bytes, 0, bytes.Length);
			response.Flush();
		}

		public bool CanGZip(HttpRequestBase request)
		{
			string acceptEncoding = request.Headers["Accept-Encoding"];
			if (!string.IsNullOrEmpty(acceptEncoding) &&
				 (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate")))
				return true;
			return false;
		}

		public bool IsReusable
		{
			get { return true; }
		}

		// private helper method that return an array of file names inside the text file stored in App_Data folder
		
	}
}