using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;  
using System.Text; 
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using Moq;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.XRules.Database.Utils; 
//O2File:IJavascriptProxy.cs
//O2File:ExtensionMethods/TMUser_ExtensionMethods.cs
//O2File:UtilMethods.cs 
//O2Ref:moq.dll
//O2Ref:System.dll
//O2Ref:System.Web.Abstractions.dll
//O2File:O2_Scripts_APIs/_O2_Scripts_Files.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public class TM_Moq_Database
	{
		//users
		public List<TMUser> TMUsers { get; set; }		
		public Dictionary<string,string> UserPasswordHashes { get; set; }
		public Dictionary<string, List<Folder_V3>> Folders_by_Library{ get; set; }
		public Dictionary<string, List<TM_GuidanceItem>> GuidanceItems_in_Library { get; set; }
		public Dictionary<string, List<TM_GuidanceItem>> GuidanceItems_in_View   { get; set; }
		public Dictionary<string, string> GuidanceItemsHtml   { get; set; }
		
		public string MoqDatabasePath { get; set; }
		//libraries
		public List<TM_Library> TM_Libraries { get; set; }
		
		public TM_Moq_Database()
		{
			//NOTE:Need to add this mappings back
			//MoqDatabasePath = new API_TeamMentor().website_SourceCode()
			//						   		  	  .pathCombine(@"3_0_WebSite\WebServices\MoqDatabase");																	   		  	  
			//loadDefaultValues();
		}
		
		public TM_Moq_Database loadDefaultValues()
		{
			TMUsers = new List<TMUser> { 
											new TMUser { UserID = 1, UserName = "admin", FirstName = "Moq", LastName = "admin user"} ,
											new TMUser { UserID = 12345, UserName = "an_user", FirstName = "Moq", LastName = "randon user"}
									   };
			UserPasswordHashes = new Dictionary<string,string>();	
			Folders_by_Library = new Dictionary<string, List<Folder_V3>> ();
			GuidanceItems_in_Library = new Dictionary<string, List<TM_GuidanceItem>>();
			GuidanceItems_in_View = new Dictionary<string, List<TM_GuidanceItem>> ();
			GuidanceItemsHtml = new Dictionary<string, string> ();
			this.setUserPassword_PwdInClearText("admin", "!!tmbeta");
			this.setUserPassword_PwdInClearText("an_user", "an_password");			
						
			var getLibrariesXml = MoqDatabasePath.pathCombine("GetLibraries.xml");
			
			TM_Libraries = getLibrariesXml.load<List<TM_Library>>();  	
			foreach(var library in TM_Libraries)
			{
			//rewrite to take into account new Folder_V3 structure
/*				var GetFolders_for_Library_Xml = MoqDatabasePath.pathCombine("GetFolders for Library '{1}' ({0}).xml".format(library.Id, library.Caption));				
				var GuidanceItems_in_Library_Xml = MoqDatabasePath.pathCombine("GuidanceItems in Library '{1}' ({0}).xml".format(library.Id, library.Caption));
				
				Folders_by_Library.Add(library.Id.str(), GetFolders_for_Library_Xml.load<List<TM_Folder>>()); 
				
				GuidanceItems_in_Library.Add(library.Id.str(), GuidanceItems_in_Library_Xml.load<List<TM_GuidanceItem>>()); 
					
				var GetGuidanceItemsInView_for_Library_Xml = MoqDatabasePath.pathCombine("GetGuidanceItemsInView for Library '{1}' ({0}).xml".format(library.Id, library.Caption));
				foreach(var item in GetGuidanceItemsInView_for_Library_Xml.load<Items>())
					GuidanceItems_in_View.Add(item.Key, (List<TM_GuidanceItem>)Serialize.getDeSerializedObjectFromString(item.Value, typeof(List<TM_GuidanceItem>)));
				
				var guidanceItemsHtml_in_Library_Xml = MoqDatabasePath.pathCombine("GuidanceItemsHtml_for_Library '{1}' ({0}).xml".format(library.Id, library.Caption));				
				foreach(var item in guidanceItemsHtml_in_Library_Xml.load<Items>())
					GuidanceItemsHtml.add(item.Key, item.Value);
				
*/				
			} 
			
			return this;
		}
	}
    public class MoqObjects
    {        				
		
    	public static IJavascriptProxy IJavascriptProxy_Moq()
    	{
			
			return createMoqDatabaseObject();
		}
		
		public static IJavascriptProxy createMoqDatabaseObject()
		{			
						
			var tmDb = new TM_Moq_Database();
											
    		var javascriptProxy = new Mock<IJavascriptProxy>(); 
    						
								
    		//ProxyType
    		javascriptProxy.Setup(proxy => proxy.ProxyType)
    					   .Returns("Moq Object");
    		
    		//GetTime()
			javascriptProxy.Setup(proxy => proxy.GetTime())
						   .Returns("(Via Moq) 01/11/2222 01:02:03 PM");
			
			javascriptProxy .addMoqsFor_IJavascriptProxy_User(tmDb)		
							.addMoqsFor_IJavascriptProxy_Session(tmDb)
							.addMoqsFor_IJavascriptProxy_Libraries(tmDb);
			
			return javascriptProxy.Object;			
		}		
		
		//major hack to support MockSession
		public static Action<string> addHttpMockSessionSupport()
		{
			var MockContext = new Mock<HttpContextBase>();
			var MockSession = new Mock<HttpSessionStateBase>();
			MockContext.Setup(ctx => ctx.Session).Returns(MockSession.Object);
			
			var sessionData = new Dictionary<string, object>();
			Action<string> setSessionVariable = 
				(key)=>{				
				        	MockSession.SetupSet(x => { x[key] = It.IsAny<object>(); })
				        			   .Callback((string name, object val) => sessionData.Add(key,val) );		   		
						}; 
						
			MockSession.Setup(x => x[It.IsAny<string>()])    
					   .Returns((string key) => { return  (object)sessionData[key];;});  	 		 
			HttpContextFactory.Context  = MockContext.Object;
			return setSessionVariable;
		}
	}
	
	public static class Mock_IJavascriptProxy_ExtensionMethods	
	{
		public static Mock<IJavascriptProxy> addMoqsFor_IJavascriptProxy_User(this Mock<IJavascriptProxy> javascriptProxy, TM_Moq_Database tmDb)
		{
			//GetUser_byName(name)			
			javascriptProxy.Setup((proxy) => proxy.GetUser_byName(It.IsAny<String>()))
						   .Returns((string name)=> tmDb.TMUsers.user(name)  );
			
			//GetUser_byID(id)
			javascriptProxy.Setup((proxy) => proxy.GetUser_byID(It.IsAny<int>()))
						   .Returns((int id)=> tmDb.TMUsers.user(id));
						   
			//GetUsers()
			javascriptProxy.Setup((proxy) => proxy.GetUsers())
						   .Returns(tmDb.TMUsers);
						   
			//CreateUser(...)			
			javascriptProxy.Setup((proxy) => proxy.CreateUser(It.IsAny<NewUser>()))
						   .Returns((NewUser newUser)
						   	  =>{
						   			if (newUser.username.inValid() ||  tmDb.TMUsers.user(newUser.username).notNull())
						   				return 0;						   			
						   			return tmDb.newUser(newUser.username, newUser.passwordHash, newUser.email, newUser.firstname, newUser.lastname, newUser.note);
								});
			
			//CreateUsers(...)
			//CreateUser(...)			
			javascriptProxy.Setup((proxy) => proxy.CreateUsers(It.IsAny<List<NewUser>>()))
						   .Returns((List<NewUser> newUsers)
						   		=>{
						   			var newUsersIds = new List<int>();
						    		foreach(var newUser in newUsers)
						    			newUsersIds.Add(javascriptProxy.Object.CreateUser(newUser));
						    		return newUsersIds;
						   		  });
			//DeleteUser(...)
			javascriptProxy.Setup((proxy) => proxy.DeleteUser(It.IsAny<int>()))
						   .Returns((int id)=> tmDb.TMUsers.delete(id));
			
			
			//DeleteUsers(...)
			javascriptProxy.Setup((proxy) => proxy.DeleteUsers(It.IsAny<List<int>>()))
						   .Returns((List<int> userIds)
						   		=>{
						   			var results = new List<bool>();
						    		foreach(var userId in userIds)
						    			results.Add(javascriptProxy.Object.DeleteUser(userId));
						    		return results;
						   		  });						   		  			
			
			//UpdateUser(...)			
//			javascriptProxy.Setup((proxy) => proxy.UpdateUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
//						   .Returns((int userId, string email, string firstname, string lastname)=> tmDb.TMUsers.updateUser(userId, email, firstname, lastname));

			return javascriptProxy;
		}

		public static Mock<IJavascriptProxy> addMoqsFor_IJavascriptProxy_Session(this  Mock<IJavascriptProxy> javascriptProxy, TM_Moq_Database tmDb)
		{
		
			//Login(...)
			javascriptProxy.Setup((proxy) => proxy.Login(It.IsAny<String>(), It.IsAny<String>()))
						   .Returns((string username, string passwordHash)=>  tmDb.login(username,passwordHash));
			//Login_PwdInClearText(...)			   
			javascriptProxy.Setup((proxy) => proxy.Login_PwdInClearText(It.IsAny<String>(), It.IsAny<String>()))
						   .Returns((string username, string password)=>  tmDb.login_PwdInClearText(username,password));
			
			return javascriptProxy;
		}
		
		public static Mock<IJavascriptProxy> addMoqsFor_IJavascriptProxy_Libraries(this  Mock<IJavascriptProxy> javascriptProxy, TM_Moq_Database tmDb)
		{					
			//GetLibraries(...)
			javascriptProxy.Setup((proxy) => proxy.GetLibraries())
						   .Returns(tmDb.getLibraries());
			//GetFolders(...)
			javascriptProxy.Setup((proxy) => proxy.GetFolders(It.IsAny<Guid>()))
						   .Returns((Guid libraryId) => tmDb.Folders_by_Library[libraryId.str()]);			
						   
			//GetGuidanceItemsInView(...)
			javascriptProxy.Setup((proxy) => proxy.GetGuidanceItemsInView(It.IsAny<Guid>()))
						   .Returns((Guid viewId) => tmDb.GuidanceItems_in_View[viewId.str()]);
						   
			//GetGuidanceItemsInViews(...)
			javascriptProxy.Setup((proxy) => proxy.GetGuidanceItemsInViews(It.IsAny<List<Guid>>()))
						   .Returns((List<Guid> viewIds) 
								=>{
										var guidanceItems = new List<TM_GuidanceItem>();
										foreach(var viewId in viewIds)
											guidanceItems.AddRange(tmDb.GuidanceItems_in_View[viewId.str()]);
										return guidanceItems;
								  }); 
			//GetGuidanceItemHtml(...)
			javascriptProxy.Setup((proxy) => proxy.GetGuidanceItemHtml(It.IsAny<Guid>()))
						   .Returns((Guid guidanceId) => tmDb.GuidanceItemsHtml[guidanceId.str()]);			
			
			//GetAllGuidanceItems(...)
			javascriptProxy.Setup((proxy) => proxy.GetAllGuidanceItems())
						   .Returns(() 
								=>{
										var allGuidanceItems = new List<TM_GuidanceItem>();
										foreach(var guidanceItems in tmDb.GuidanceItems_in_Library.Values)
											allGuidanceItems.AddRange(guidanceItems);
										return allGuidanceItems;
								  }); 		
								  
			//GetGuidanceItemsInLibrary(...)
			javascriptProxy.Setup((proxy) => proxy.GetGuidanceItemsInLibrary(It.IsAny<Guid>()))
						   .Returns((Guid libraryId) => tmDb.GuidanceItems_in_Library[libraryId.str()]);
			return javascriptProxy;
		}
    }
    	
    public static class MoqObjects_ExtensionMethods
    {        	
    	public static int newUser(this TM_Moq_Database tmDb, string  username, string passwordHash, string email, string firstname, string lastname, string note)
    	{
    		var userId = 5.randomNumbers().toInt();
    		var tmUser = new TMUser {
    									UserID = userId,
    									UserName = username,
    									FirstName = firstname,
    									LastName = lastname,
    									Company = "",
    									EMail = email   									 
    								};	
			    								
			tmDb.TMUsers.Add(tmUser);
			tmDb.setUserPassword_PwdInClearText(username, passwordHash);
			return userId;    		
    	}
    	    	    	
    	public static TM_Moq_Database setUserPassword_PwdInClearText(this TM_Moq_Database tmDb, string username, string password)
    	{
    		tmDb.UserPasswordHashes.add(username, username.createPasswordHash(password));
    		return tmDb;
    	}
    	
    	public static Guid login(this TM_Moq_Database tmDb, string username, string passwordHash)
    	{
    		if(tmDb.UserPasswordHashes.hasKey(username))
    			if (tmDb.UserPasswordHashes[username] == passwordHash)
    				return Guid.NewGuid();
    		return Guid.Empty;    			
    	}
    	
    	public static Guid login_PwdInClearText(this TM_Moq_Database tmDb, string username, string password)
    	{
    		if(tmDb.UserPasswordHashes.hasKey(username))
    			if (tmDb.UserPasswordHashes[username] == username.createPasswordHash(password))
    				return Guid.NewGuid();
    		return Guid.Empty;    			
    	}    	
    }
	public static class MoqObjects_Libraries_ExtensionMethods	
	{
		public static List<TM_Library> getLibraries(this TM_Moq_Database tmDb)
		{
			return tmDb.TM_Libraries;
		}
	}
	
    
/*    public static class Misc_ExtensionMethods
    {
    	//move this to a generic helper classes
    	public static string createPasswordHash(this string username, string password)
		{			
			var stringToHash = username+password;
			var sha256 = System.Security.Cryptography.SHA256.Create();
			var hashBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(stringToHash));
			var hashString = new StringBuilder();	
			foreach (byte b in hashBytes)		
				hashString.Append(b.ToString("x2"));					
			return hashString.ToString();
		}
    }
*/
}
