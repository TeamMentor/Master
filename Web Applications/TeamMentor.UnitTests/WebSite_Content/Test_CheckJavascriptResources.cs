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
			PublicDI.log.writeToDebug();
		}

		[Test]
		public void Check_GoogleAnalitics()
		{
			Console.WriteLine("##teamcity[message text='Console normal text' errorDetails='stack trace' status='NORMAL']");
			"##teamcity[message text='Exception text' errorDetails='stack trace' status='ERROR']".info();
			
			var assembly = Assembly.GetExecutingAssembly();			
			"Location: {0}".info(assembly.Location);
			"CodeBase: {0}".info(assembly.CodeBase);
			"Escaped Evident: {0}".info(assembly.CodeBase);			

			var url = "http://www.google-analytics.com/ga.js";
			"Url:".info(url);
			//this.script_Me().waitForClose();
		}
	}
}
