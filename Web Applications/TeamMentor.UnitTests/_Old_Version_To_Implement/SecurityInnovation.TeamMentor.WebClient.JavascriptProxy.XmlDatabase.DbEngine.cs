// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{		 
	[TestFixture]
    public class Test_DbEngine  : TM_XmlDatabase_InMemory
    { 
    	static bool skipLongerTests { get; set;}
    	
     	static Test_DbEngine()
     	{     		
     		//TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;    		
     	}
     	 
    	public Test_DbEngine() 
    	{   
    		skipLongerTests = true;    		    	    		
    		UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
    	}     		
    	 
    	[Test]    
    	public void Test_XmlDatabase_Setup()
    	{
    		Assert.IsNotNull(tmXmlDatabase,"JavascriptProxy");    		    		
    	}
    	
    	[Test]       	  
    	public void Test_getGuidanceExplorerObjects() 
    	{    
			var guidanceExplorers = tmXmlDatabase.Path_XmlLibraries.getGuidanceExplorerObjects();			
			Assert.IsNotNull(guidanceExplorers, "guidanceExplorers");
			Assert.That(guidanceExplorers.size()>0 , "guidanceExplorers was empty");			
			Assert.That(tmXmlDatabase.GuidanceExplorers_XmlFormat.size() > 0, "GuidanceExplorers_XmlFormat was empty");    		
    	}
    	
    	
    	[Test] 
    	public void Test_getLibraries()
    	{ 
    		//var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
    		//Assert.That(guidanceExplorers.size() > 0, "guidanceExplorers was empty");
    		var guidanceExplorers = tmXmlDatabase.GuidanceExplorers_XmlFormat.Values.toList();
    		var tmLibraries = tmXmlDatabase.tmLibraries();
    		Assert.IsNotNull(tmLibraries,"tmLibraries"); 
    		for(var i=0;  i < guidanceExplorers.size() ; i++)
    		{
    			Assert.AreEqual(tmLibraries[i].Caption,  guidanceExplorers[i].library.caption, "caption");
    			Assert.AreEqual(tmLibraries[i].Id, guidanceExplorers[i].library.name.guid(), "caption");
    		}
    		Assert.That(tmXmlDatabase.GuidanceExplorers_XmlFormat.size()>0, "GuidanceExplorers_XmlFormat empty");    		
    	}
    	 
    	   
    	[Test] 
    	public void Test_getFolders()
    	{
    		//var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
    		var libraryId = tmXmlDatabase.GuidanceExplorers_XmlFormat.Keys.first();
    		var guidanceExplorerFolders = tmXmlDatabase.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure.folder;    		
    		Assert.That(guidanceExplorerFolders.size() > 0,"guidanceExplorerFolders was empty");
    		
    		var tmFolders = tmXmlDatabase.tmFolders(libraryId);
    		Assert.IsNotNull(tmFolders,"folders"); 
    		Assert.That(tmFolders.size() > 0,"folders was empty");
    		//show.info(guidanceExplorerFolders);
    		//show.info(tmFolders);	
    		var mappedById = new Dictionary<Guid,Folder_V3>();
    		    		
    		foreach(var tmFolder in tmFolders)
    			mappedById.Add(tmFolder.folderId, tmFolder);
    			
    		//Add checks for sub folders	
    		foreach(var folder in guidanceExplorerFolders)
    		{
				Assert.That(mappedById.hasKey(folder.folderId.guid()), "mappedById didn't have key: {0}".format(folder.folderId));    				
				var tmFolder = mappedById[folder.folderId.guid()];				
				Assert.That(tmFolder.name == folder.caption);				
				Assert.That(tmFolder.libraryId == libraryId, "libraryId");	
    		}      		
    	} 
    	
    	[Test]
    	public void Test_getGuidanceHtml()
    	{
    		var guidanceItems = tmXmlDatabase.tmGuidanceItems();
    		var firstGuidanceItem = guidanceItems.first();
    		Assert.IsNotNull(firstGuidanceItem,"firstGuidanceItem");
    		var guid = firstGuidanceItem.Metadata.Id;
    		Assert.IsNotNull(guid,"guid");
    		Assert.AreNotEqual(guid, Guid.Empty,"guid.isGuid");
    	    var sessionId = Guid.Empty;
    		var html = tmXmlDatabase.getGuidanceItemHtml(sessionId,guid);
    		Assert.IsNotNull(html, "html");    		
    		Assert.That(html.valid(), "html was empty");    		
    	}
    	
    	
    } 
}    
	