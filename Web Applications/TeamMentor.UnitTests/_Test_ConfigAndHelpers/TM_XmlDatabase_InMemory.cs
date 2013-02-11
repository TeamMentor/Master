using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{    
	public class TM_XmlDatabase_InMemory
	{
		public TM_Xml_Database tmXmlDatabase;
		
		public TM_XmlDatabase_InMemory()
		{
		    UserGroup.Admin.setThreadPrincipalWithRoles();
			tmXmlDatabase = new TM_Xml_Database();
            UserGroup.Anonymous.setThreadPrincipalWithRoles();
		    //new TM_TestLibrary().CreateTestDatabase(tmXmlDatabase);
		}

		[TestFixtureSetUp]		
		public void TestFixtureSetUp()
		{			
            //all these values should be null since we are running TM memory (default setting)
			Assert.IsNull(tmXmlDatabase.Path_XmlDatabase		    , "Path_XmlDatabase");
			Assert.IsNull(tmXmlDatabase.Path_XmlLibraries		    , "Path_XmlLibraries");
			Assert.IsEmpty(tmXmlDatabase.Cached_GuidanceItems	    , "Cached_GuidanceItems");
			Assert.IsEmpty(tmXmlDatabase.UserData.ActiveSessions    , "ActiveSessions");
			Assert.AreEqual(tmXmlDatabase.UserData.TMUsers.size(),1 , "TMUsers");				// there should be admin
		}

	}
}
