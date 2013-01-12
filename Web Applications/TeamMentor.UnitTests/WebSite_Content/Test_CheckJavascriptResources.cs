using System;
using System.Reflection;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;
using System.Diagnostics;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.WebSite_Content
{
	[TestFixture]
	public class Test_CheckJavascriptResources
	{
		[SetUp]
		public void setup()
		{
			PublicDI.log.writeToDebug(true);						
		}

		[Test]
		public void Check_GoogleAnalitics()
		{			
			var assembly = this.type().Assembly;

			"CodeBase: {0}".info(assembly.CodeBase);
			"Location: {0}".info(assembly.Location);

			//this.script_Me().waitForClose();
		}

		/*Console.WriteLine("Executing Assembly: {0}".format(Assembly.GetExecutingAssembly()));
			var assembly = Assembly.GetExecutingAssembly();			
			"Location: {0}".info(assembly.Location);
			"CodeBase: {0}".info(assembly.CodeBase);
			"Escaped Evident: {0}".info(assembly.CodeBase);			

			//var url = "http://www.google-analytics.com/ga.js";
			//"Url:".info(url);
			
		}*/
	}
}
