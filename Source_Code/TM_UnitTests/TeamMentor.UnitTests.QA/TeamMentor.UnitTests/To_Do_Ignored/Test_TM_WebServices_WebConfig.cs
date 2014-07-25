using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.Xml;
using NUnit.Framework;
using TeamMentor.UnitTests.WebSite_Content;

namespace TeamMentor.UnitTests.QA.To_Do_Ignored
{
    [TestFixture]
    public class Test_TM_WebServices_WebConfig_Production
    {
        public string WebConfigFile { get; set; }

        public Test_TM_WebServices_WebConfig_Production()
        {
            WebConfigFile = new Test_TM_WebServices_WebConfig().WebConfigFile;            
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
    }
}
