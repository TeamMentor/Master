using NUnit.Framework;  
using FluentSharp.CoreLib;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.WebSite_Content
{		  
	[TestFixture]  
    public class Test_TM_WebServices_WebConfig
    {     			 
    	public string WebConfigFile { get; set; }
    	
    	public Test_TM_WebServices_WebConfig()    
    	{    		
            var assembly		 = this.type().Assembly;

            var dllLocation		 = assembly.CodeBase.subString(8);
            var webApplications  = dllLocation.parentFolder()
                                              .pathCombine(@"\..\..\..\..");
            var tmWebsite 		 = webApplications.pathCombine(@"TM_Websites\Website_3.4");
            		
    		WebConfigFile        = tmWebsite.pathCombine("web.config");

            Assert.IsTrue(WebConfigFile.fileExists(), "Could not find WebConfigFile: {0}".format(WebConfigFile));

    	} 
    	    	
    	[Test]  
    	public void loadWebConfigFile()
    	{       		
    		Assert.That(WebConfigFile.fileExists(), "could not find, web.config file: {0}".format(WebConfigFile));
    		var xRoot = WebConfigFile.xRoot();
    		Assert.IsNotNull(xRoot, "xRoot of webConfigFile");
    	}    	
    	
    	[Test][Ignore("Move to production specific QA tests")]  
    	public void system_web_compilation_debug_IS_False()
    	{   
    		var compilation = WebConfigFile.xRoot().element("system.web").element("compilation");
    		Assert.IsNotNull(compilation, "compilation element");
    		var debugAttribute = compilation.attribute("debug");
    		Assert.IsNotNull(debugAttribute, "debug attribute");			
			var debugValue = debugAttribute.value();
			Assert.AreNotEqual(debugValue.lower() ,"true", "system.web / compilation / debug attribute value should not be true");		
            Assert.AreEqual(debugValue.lower() ,"false");
		}
		
		[Test]  
    	public void system_web_compilation_customErrors_IS_Off()
    	{   
    		var customErrors = WebConfigFile.xRoot().element("system.web").element("customErrors");
    		Assert.IsNotNull(customErrors, "customErrors element");
    		var modeAttribute = customErrors.attribute("mode");
    		Assert.IsNotNull(modeAttribute, "mode attribute");			
			var value = modeAttribute.value();
			Assert.AreEqual(value.lower() ,"off", "system.web / customErrors / mode value should be Off");		
		}					
	}       
}