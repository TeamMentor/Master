using System;
using System.Text;
using FluentSharp.CoreLib;
using System.IO;
using FluentSharp.Web;

namespace TeamMentor.CoreLib
{
    public class API_ScriptCombiner
    {
        public string   MappingsLocation			 {get;set;}
			
		public string setName 				{ get; set;}
		public string version 				{ get; set;}
        public string baseFolder 			{ get; set;}
		public string contentType 			{ get; set;}
	
		public string[] 	 filesProcessed	{ get; set;}
		public StringBuilder allScripts		{ get; set;}
		public string 		 minifiedCode	{ get; set;}
		public bool 		 minifyCode		{ get; set;}
		public bool 		 ignoreCache	{ get; set; }
        public bool 		 isCompressed	{ get; set;}
        public byte[]        CombinedBytes  { get; set;}

        public API_ScriptCombiner()
        {
            MappingsLocation = "/javascript/_mappings/{0}.txt";				
            CombinedBytes  = new byte[0] {};
            filesProcessed = new String[0] {};            
        }
        public API_ScriptCombiner CombineFiles()
        {
            if (baseFolder.dirExists().isFalse())
            {
                "[API_ScriptCombiner] baseFolder Not set".error();
                return this;
            }
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
						var fullPath = baseFolder.pathCombine(fileName.trim());
                        
                        if(fullPath.contains(baseFolder).isFalse())
                            "[API_ScriptCombiner][CombineFiles] resolved full path ('{0}') did not contain baseFolder ('{1}')".format(fullPath, baseFolder);
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
				CombinedBytes = memoryStream.ToArray();                
            }
            return this;
        }

        public string[] GetScriptFileNames(string setName)
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
    }
}
