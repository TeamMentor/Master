//based on code from http://atashbahar.com/post/Combine-minify-compress-JavaScript-files-to-load-ASPNET-pages-faster.aspx
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using XssEncoder = Microsoft.Security.Application.Encoder;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:ICSharpCode.SharpZipLib.dll
//O2File:..\UtilMethods.cs
//O2File:JavaScriptMinifier.cs

//O2Ref:AntiXSSLibrary.dll

public partial class ScriptCombiner : IHttpHandler
{
    public static TimeSpan CACHE_DURATION = TimeSpan.FromDays(30);
	public static string mappingsLocation {get;set;}
	
	public string setName 				{ get; set;}
    public string version 				{ get; set;}
	public string contentType 			{ get; set;}
	
	public string[] 	 filesProcessed	{ get; set;}
	public StringBuilder allScripts		{ get; set;}
	public string 		 minifiedCode	{ get; set;}
	public bool 		 minifyCode		{ get; set;}
	public bool 		 ignoreCache	{ get; set; }
	
	public HttpContextBase context;
	
	public ScriptCombiner()
	{
		ScriptCombiner.mappingsLocation = "../javascript/_mappings/{0}.txt";				
	}
	
    public void ProcessRequest(HttpContext __context)
    {				
		this.context = HttpContextFactory.Current;		
		var request = context.Request;        
		var response = context.Response;		
		response.Clear();	
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
					this.contentType = "text/css";
					minifyCode = false;
					break;
				default:
					this.contentType = "application/x-javascript";
					break;
			}
			// Decide if browser supports compressed response
			bool isCompressed = this.CanGZip(context.Request);

			// If the set has already been cached, write the response directly from
			// cache. Otherwise generate the response and cache it
			if (ignoreCache || !this.WriteFromCache(setName, version, isCompressed))
			{
				using (MemoryStream memoryStream = new MemoryStream(8092))
				{
					// Decide regular stream or gzip stream based on whether the response can be compressed or not
					//using (Stream writer = isCompressed ?  (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) : memoryStream)
					using (Stream writer = isCompressed ? (Stream)(new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(memoryStream)) : memoryStream)                
					{
						// Read the files into one big string
						this.allScripts = new StringBuilder();
						this.filesProcessed = GetScriptFileNames(setName);
						foreach (string fileName in this.filesProcessed)
						{							
							var fullPath = context.Server.MapPath(fileName.trim());
		
							if (fullPath.fileExists())
							{
								this.allScripts.AppendLine("\n\n/********************************** ");
								this.allScripts.AppendLine(" *****    " + fileName   );
								this.allScripts.AppendLine(" **********************************/\n\n");
								this.allScripts.AppendLine(File.ReadAllText(fullPath));															
							}
						}
						
						var codeToSend = this.allScripts.ToString();
								
						if (minifyCode)
						{							
							// Minify the combined script files and remove comments and white spaces
							var minifier = new JavaScriptMinifier();
							this.minifiedCode = minifier.Minify(codeToSend);
							codeToSend = this.minifiedCode;							
						}			
						
						// Send minfied string to output stream
						byte[] bts = Encoding.UTF8.GetBytes(codeToSend);
						writer.Write(bts, 0, bts.Length);
					}

					// Cache the combined response so that it can be directly written
					// in subsequent calls 
					byte[] responseBytes = memoryStream.ToArray();
					context.Cache.Insert(GetCacheKey(setName, version, isCompressed),
						responseBytes, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
						CACHE_DURATION);
						
					// Generate the response
					this.WriteBytes(responseBytes, isCompressed);
				}
			}
		}
		catch(Exception ex)
		{
			ex.log();
			response.Write("//Error processing request"+  ex.Message);
			response.End();
		}		
    }
	
    public bool WriteFromCache(string setName, string version, bool isCompressed)
    {
        byte[] responseBytes = context.Cache[GetCacheKey(setName, version, isCompressed)] as byte[];

        if (responseBytes == null || responseBytes.Length == 0)
            return false;

        this.WriteBytes(responseBytes, isCompressed);
        return true;
    }

    public void WriteBytes(byte[] bytes, bool isCompressed)
    {
        var response = context.Response;

        response.AppendHeader("Content-Length", bytes.Length.ToString());
		
        response.ContentType = this.contentType;

        if (isCompressed)
            response.AppendHeader("Content-Encoding", "gzip");
        else
            response.AppendHeader("Content-Encoding", "utf-8");

//        context.Response.Cache.SetCacheability(HttpCacheability.Public);
//        context.Response.Cache.SetExpires(DateTime.Now.Add(CACHE_DURATION));
//        context.Response.Cache.SetMaxAge(CACHE_DURATION);
								
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

    public string GetCacheKey(string setName, string version, bool isCompressed)
    {
        return "HttpCombiner." + setName + "." + version + "." + isCompressed;
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
		var resolvedFile = ScriptCombiner.mappingsLocation.format(setName);		
		
        string setPath = httpContext.Server.MapPath(resolvedFile);		
		
		if (setPath.fileExists())		
			using (var setDefinition = File.OpenText(setPath))
			{
				string fileName = null;
				while (setDefinition.Peek() >= 0)
				{
					fileName = setDefinition.ReadLine();
					if (!String.IsNullOrEmpty(fileName))
						scripts.Add(fileName);						
				}
			}	
        return scripts.ToArray();

    }
}
