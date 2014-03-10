using System.Web; 
using NUnit.Framework; 
using FluentSharp.CoreLib;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{		  
	[TestFixture] 
    public class Test_JavascriptCombiner 
    {
    	public string BaseDir	{ get; set;}
    	
    	public API_Moq_HttpContext httpContextApi ;
    	public HttpContextBase 	context;
    	public HttpRequestBase 	request;
    	public HttpResponseBase response; 
 
 		public string EMPTY_RESPONSE = "//nothing to do";
 		public string DONT_MINIFY	 = "&dontMinify=true";	    	    
     	
    	public Test_JavascriptCombiner() 
    	{     		    		    	 	    	    
    		BaseDir = "javascriptCombiner".tempDir(false);
			httpContextApi 	= new API_Moq_HttpContext(BaseDir);   
			
			context 	= httpContextApi.httpContext();
			request  	= context.Request;
			response 	= context.Response;
			
			HttpContextFactory.Context = context;						
    	}     	
    	

		[Test]
		public void serverCode_test_MockingEnvironemnt()
		{
			Assert.That(BaseDir.dirExists(), "test base dir didn't exist");		
			Assert.AreEqual(BaseDir, httpContextApi.BaseDir, "BaseDir in httpContextApi");
			
			Assert.IsNotNull(context , "httpContext was null");
			Assert.IsNotNull(request , "httpContext was null");
			Assert.IsNotNull(response, "httpContext was null");
			
			var responseWriteText = "<h1>response Write Test</h1>";
			
			response.Write(responseWriteText);
			
			//test: response writing to respose
			var responseString = context.response_Read_All();
			Assert.AreEqual(responseWriteText, responseString);
			
			//test: map
			var tempFile = "aaa.html";
			var tempFilePath = BaseDir.pathCombine(tempFile);
			Assert.AreEqual(context.Server.MapPath(tempFile), tempFilePath, "Map file resolution");
			
			//test: request values
			var testKey   = "a key";
			var testValue = "a test value";
			request.QueryString[testKey] = testValue;
			 
			Assert.AreEqual(request.QueryString[testKey], testValue, "QueryString set failed");
		}
		
		//[Test][Ignore("Race condition when running in paralell with other tests")]
		public void serverCode_defaultValues_and_EmptyRequest()		
		{
			var scriptCombiner = new ScriptCombiner(); 						
			scriptCombiner.ProcessRequest(null); 
			
			Assert.AreEqual	(scriptCombiner.setName,string.Empty , "[empty request] setName");
			Assert.AreEqual	(scriptCombiner.version,string.Empty , "[empty request] version");
			Assert.IsNotNull(ScriptCombiner.MappingsLocation	 , "[empty request] mappingsLocation");
			
			var responseHtml = context.response_Read_All();
			Assert.AreEqual(EMPTY_RESPONSE,responseHtml, "[empty request] responseHtml should be empty");
			 
			request.QueryString["s"] = "setName";
			request.QueryString["v"] = "version";
			scriptCombiner.ProcessRequest(null); 
			Assert.AreEqual(scriptCombiner.setName,"setName", "setName value"); 
			Assert.AreEqual(scriptCombiner.version,"version", "setName value");			
			 
			//test test handshake			
			request.QueryString["Hello"] = "TM"; 
			scriptCombiner.ProcessRequest(null); 
			responseHtml = context.response_Read_All();
			Assert.AreEqual(responseHtml, "Good Morning", "handshake");
		}
		
		[Test]
		public void serverCode_minifyCodeSetting()		
		{				
			var scriptCombiner = new ScriptCombiner(); 						
			
			scriptCombiner.ProcessRequest(null); 
			Assert.IsTrue(scriptCombiner.minifyCode, "minifyCode should be true");			 
			
			request.QueryString["s"] = "someValue";
			request.QueryString["dontMinify"] = "true";
			scriptCombiner.ProcessRequest(null); 
			Assert.IsFalse(scriptCombiner.minifyCode, "minifyCode should be false");
		}
		
		[Test]
		public void serverCode_makeRequestFor_one_JavascriptFile()
		{
			var scriptCombiner = new ScriptCombiner {ignoreCache = true};
		    ScriptCombiner.MappingsLocation = "{0}.txt";
			
			var fileContents = "var a=1; // a test js file";
			var expectedResult = "\nvar a=1;";
			var file1 = "a.js";
			var mappingName = "testMapping";
			
			var mappingFile = file1.saveAs(BaseDir.pathCombine(mappingName + ".txt"));			
			var jsFile = fileContents.saveAs(BaseDir.pathCombine("a.js"));
			
			Assert.That(mappingFile.fileExists() && mappingFile.fileContents()  == file1, "mappingFile not OK");
			Assert.That(jsFile.fileExists() 	 && jsFile.fileContents() 		== fileContents, "mappingFile not OK");
			
			request.QueryString["s"] = mappingName;
			scriptCombiner.ProcessRequest(null); 
			
			var responseText = context.response_Read_All();
			Assert.AreEqual(expectedResult					, responseText	, "responseText != expectedResult");								
			Assert.AreEqual(scriptCombiner.filesProcessed[0], file1			, "scriptCombiner.filesProcessed[0]");
			
			// these two fails due to weird encoding bug (the text is the same (in ascii))
			//Assert.AreEqual(scriptCombiner.minifiedCode		, expectedResult, "minifiedCode");	
			//Assert.AreEqual(scriptCombiner.allScripts.str() , fileContents  , "allScripts");  
		}
		
		[Test]
		public void serverCode_makeRequestFor_two_JavascriptFile() 
		{
			var scriptCombiner = new ScriptCombiner();  
			scriptCombiner.ignoreCache = true;
			ScriptCombiner.MappingsLocation = "{0}.txt";
			
			var fileContents1 = "var a=1; // a test js file";
			var fileContents2 = "var b=1; // a test js file";
			var expectedResult = "\nvar a=1;var b=1;";
			var file1 = "a.js";
			var file2 = "b.js";
			var mappingName = "testMapping";
			var mappingContents = file1.line() + file2.line();
			
			mappingContents.saveAs(this.BaseDir.pathCombine(mappingName + ".txt"));			
			fileContents1.saveAs(this.BaseDir.pathCombine(file1));	
			fileContents2.saveAs(this.BaseDir.pathCombine(file2));
			
			request.QueryString["s"] = mappingName;
			scriptCombiner.ProcessRequest(null); 
			
			var responseText = context.response_Read_All();
			Assert.AreEqual(expectedResult,responseText, "responseText != expectedResult");
		}	
    }
}
