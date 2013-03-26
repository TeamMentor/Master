//based on code from http://atashbahar.com/post/Combine-minify-compress-JavaScript-files-to-load-ASPNET-pages-faster.aspx
using System;
using System.IO;

using System.Text;
using System.Web;
using XssEncoder = Microsoft.Security.Application.Encoder;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
	public class ScriptCombiner : IHttpHandler
	{    
		public static string   MappingsLocation			 {get;set;}
		public static DateTime LastModified_HeaderDate	{ get; set; }
	
		public string setName 				{ get; set;}
		public string version 				{ get; set;}
		public string contentType 			{ get; set;}
	
		public string[] 	 filesProcessed	{ get; set;}
		public StringBuilder allScripts		{ get; set;}
		public string 		 minifiedCode	{ get; set;}
		public bool 		 minifyCode		{ get; set;}
		public bool 		 ignoreCache	{ get; set; }
	
		public HttpContextBase context;

		static ScriptCombiner()
		{
			LastModified_HeaderDate = DateTime.Now;			//used to calculate if we will send a '302 Not Modified' to the user
		}
		public ScriptCombiner()
		{
			MappingsLocation = "../javascript/_mappings/{0}.txt";				
		}
	
		public void ProcessRequest(HttpContext httpContext)
		{				
			context = HttpContextFactory.Current;		
			var request = context.Request;        
			var response = context.Response;		
			response.Clear();

			if (TMConfig.Current.TMSetup.Enable304Redirects && send304Redirect())
			{
				context.Response.StatusCode = 304;
				context.Response.StatusDescription = "Not Modified";
				return;
			}
			setCacheHeaders();		

			try
			{
				minifyCode = true;
				ignoreCache = true;
				if (request.QueryString["Hello"]=="TM")
				{
					response.Write("Good Morning");
					return;
				}

				// Read setName, version from query string
				setName = XssEncoder.UrlEncode(request.QueryString["s"]) ?? string.Empty;
				version = XssEncoder.UrlEncode(request.QueryString["v"]) ?? string.Empty;
						
				if (setName ==string.Empty)
				{
					response.Write("//nothing to do");
					return;
				}			
			
				if (request.QueryString["dontMinify"] == "true")
					minifyCode = false;

				switch(request.QueryString["ct"])
				{
					case "css": 
						contentType = "text/css";
						minifyCode = false;
						break;
					default:
						contentType = "application/x-javascript";
						break;
				}
				// Decide if browser supports compressed response
				bool isCompressed = CanGZip(context.Request);

				using (var memoryStream = new MemoryStream(8092))
				{
					// Decide regular stream or gzip stream based on whether the response can be compressed or not
					//using (Stream writer = isCompressed ?  (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) : memoryStream)
					using (Stream writer = isCompressed ? (Stream)(new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(memoryStream)) : memoryStream)
					{
						// Read the files into one big string
						allScripts = new StringBuilder();
						filesProcessed = GetScriptFileNames(setName);
						foreach (string fileName in filesProcessed)
						{
							var fullPath = context.Server.MapPath(fileName.trim());

							if (fullPath.fileExists())
							{
								allScripts.AppendLine("\n\n/********************************** ");
								allScripts.AppendLine(" *****    " + fileName);
								allScripts.AppendLine(" **********************************/\n\n");
								allScripts.AppendLine(File.ReadAllText(fullPath));
							}
						}

						var codeToSend = allScripts.ToString();

						if (minifyCode)
						{
							// Minify the combined script files and remove comments and white spaces
							var minifier = new JavaScriptMinifier();
							minifiedCode = minifier.Minify(codeToSend);
							codeToSend = minifiedCode;
						}

						// Send minfied string to output stream
						byte[] bts = Encoding.UTF8.GetBytes(codeToSend);
						writer.Write(bts, 0, bts.Length);
					}				

					// Generate the response
					var responseBytes = memoryStream.ToArray();
					WriteBytes(responseBytes, isCompressed);

				}
			}
			catch(Exception ex)
			{
				ex.log();
				response.Write("//Error processing request"+  ex.Message);
				response.End();
			}		
		}	

		public void WriteBytes(byte[] bytes, bool isCompressed)
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
		private static string[] GetScriptFileNames(string setName)
		{	
			var httpContext = HttpContextFactory.Current;	//HttpContext.Current
		
			var scripts = new System.Collections.Generic.List<string>();		
			var resolvedFile = MappingsLocation.format(setName);		
		
			string setPath = httpContext.Server.MapPath(resolvedFile);		
		
			if (setPath.fileExists())		
				using (var setDefinition = File.OpenText(setPath))
				{					
					while (setDefinition.Peek() >= 0)
					{
						var fileName = setDefinition.ReadLine();
						if (fileName.valid() && fileName.starts("#").isFalse())
							scripts.Add(fileName);					
					}
				}	
			return scripts.ToArray();
		}

		//Cache code

		public bool send304Redirect()
		{
			var ifModifiedSinceHeader = context.Request.Headers["If-Modified-Since"];
			if (ifModifiedSinceHeader.valid() && ifModifiedSinceHeader.isDate())
			{
				var ifModifiedSinceDate = DateTime.Parse(ifModifiedSinceHeader);
				if (LastModified_HeaderDate.str() == ifModifiedSinceDate.str())
					return true;
			}
			return false;
		}

		public void setCacheHeaders()
		{	
			context.Response.Cache.SetLastModified(LastModified_HeaderDate);
		}
	}
}