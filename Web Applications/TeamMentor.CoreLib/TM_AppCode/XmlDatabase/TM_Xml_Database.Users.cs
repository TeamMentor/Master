using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
//using System.Text;
using Microsoft.Security.Application;
using System.Security.Permissions;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
//using Moq;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
//O2File:TM_Xml_Database.cs
//O2File:../ExtensionMethods/TMUser_ExtensionMethods.cs
//O2File:../Authentication/UserRoles.cs
//O2File:../Authentication/ExtensionMethods/TeamMentorUserManagement_ExtensionMethods.cs
//O2Ref:AntiXSSLibrary.dll

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{	
	public partial class TM_Xml_Database 
	{
        public static int FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION = 500;

		public static void loadTmUserObjects(string xmlDatabasePath)
		{
			if(xmlDatabasePath.dirExists().isFalse())
			{
				"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in loadTmUserObjects, provided xmlDatabasePath didn't exist: {0}".error(xmlDatabasePath);
				return;
			}
			var tmUsersXmlFile = xmlDatabasePath.getTmUsersXmlFile();
			var tmUsersPasswordsXmlFile = xmlDatabasePath.getTmUsersPasswordsXmlFile();
			if(tmUsersXmlFile.fileExists().isFalse())			
				new List<TMUser>().saveAs(tmUsersXmlFile);
			
			if(tmUsersPasswordsXmlFile.fileExists().isFalse())			
				new O2.DotNetWrappers.DotNet.Items().saveAs(tmUsersPasswordsXmlFile);
				
			TM_Xml_Database.TMUsers = tmUsersXmlFile.load<List<TMUser>>();	
			TM_Xml_Database.TMUsersPasswordHashes = tmUsersPasswordsXmlFile.load<O2.DotNetWrappers.DotNet.Items>();				
		}
		
		//[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static void saveTmUserObjects(string xmlDatabasePath)
		{
			"in saveTmUserObjects".info(); 
			lock(xmlDatabasePath)
			{
				var tmUsersXmlFile = xmlDatabasePath.getTmUsersXmlFile();
				var tmUsersPasswordsXmlFile = xmlDatabasePath.getTmUsersPasswordsXmlFile();
				TM_Xml_Database.TMUsers.saveAs(tmUsersXmlFile);
				TM_Xml_Database.TMUsersPasswordHashes.saveAs(tmUsersPasswordsXmlFile);
			}
		}			
	}
	
	public static class TM_Xml_Database_ExtensionMethods_Users_Persistance
	{		
		public static string getTmUsersXmlFile(this string xmlDatabasePath)
		{
			if(xmlDatabasePath.dirExists())
				return xmlDatabasePath.pathCombine("TmUsers.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersXmlFile, provided xmlDatabasePath didn't exist: {0}".error(xmlDatabasePath);
			return null;
		}
		
		public static string getTmUsersXmlFile(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.Path_XmlDatabase.getTmUsersXmlFile();
		}
		
		public static string getTmUsersPasswordsXmlFile(this string xmlDatabasePath)
		{
			if(xmlDatabasePath.dirExists())
				return xmlDatabasePath.pathCombine("TmUsers_Passwords.xml");
			"[TM_Xml_Database_ExtensionMethods_Users_Persistance] in getTmUsersPasswordsXmlFile, provided xmlDatabasePath didn't exist: {0}".error(xmlDatabasePath);
			return null;
		}
		
		public static string getTmUsersPasswordsXmlFile(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.Path_XmlDatabase.getTmUsersPasswordsXmlFile();
		}
		
		public static TM_Xml_Database saveTmUserDataToDisk(this TM_Xml_Database tmDb)
		{
			TM_Xml_Database.saveTmUserObjects(TM_Xml_Database.Path_XmlDatabase);			
			return tmDb;
		}
		
	}
	
	public static class TM_Xml_Database_ExtensionMethods_Users
    {   		
		public static int createDefaultAdminUser(this TM_Xml_Database tmDb)
		{  
			var tmConfig = TMConfig.Current;
			var defaultAdminUser_name = tmConfig.DefaultAdminUserName;
			var defaultAdminUser_pwd =  tmConfig.DefaultAdminPassword;
			var adminUser = tmDb.tmUser(defaultAdminUser_name);
			if (adminUser.notNull())
			{
				//"[TM_Xml_Database] in TMUser createDefaultAdminUser, defaultAdminUser_name already existed in the database (returning its Id): {0}".debug(defaultAdminUser_name);
				return adminUser.UserID;
			}
			var passwordHash = defaultAdminUser_name.createPasswordHash(defaultAdminUser_pwd);
			return tmDb.newUser(defaultAdminUser_name, passwordHash, 1);
		}
		
		public static TMUser tmUser(this string name)
		{
			return TM_Xml_Database.TMUsers.user(name);
		}
		
		public static TMUser tmUser(this TM_Xml_Database tmDb, string name)
		{
			return TM_Xml_Database.TMUsers.user(name);
		}
		
		public static TMUser tmUser(this TM_Xml_Database tmDb, int userId)
		{
			return TM_Xml_Database.TMUsers.user(userId);
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static List<TMUser> tmUsers(this List<int> usersId)
		{
			var tmUsers = new List<TMUser>();
			foreach(var userId in usersId)								
				tmUsers.Add(TM_Xml_Database.TMUsers.user(userId));
			return tmUsers;
		}
				
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static List<TMUser> tmUsers(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.TMUsers.toList();
		}
		
		public static List<int> userIds(this List<TMUser> tmUsers)
		{
			return (from tmUser in tmUsers
					where tmUser.notNull()
					select tmUser.UserID).toList();
		}
		
		public static int newUser(this TM_Xml_Database tmDb)
		{
			return tmDb.newUser("test_user_{0}".format(5.randomLetters()));
		}
		
		public static int newUser(this TM_Xml_Database tmDb, string  username)
		{
			return tmDb.newUser(username, username.createPasswordHash(5.randomLetters()));
		}
		
		public static int newUser_ClearTextPassword(this TM_Xml_Database tmDb, string  username, string password)
		{
			return tmDb.newUser(username, username.createPasswordHash(password));
		}
		
		public static int newUser(this TM_Xml_Database tmDb, string  username, string passwordHash)
		{
			return tmDb.newUser(username,passwordHash, 0);
		}
		
		public static int newUser(this TM_Xml_Database tmDb, string  username, string passwordHash, int groupId)
		{
			return tmDb.newUser(username, passwordHash, "","","","", groupId);
		}
		
    	public static int newUser(this TM_Xml_Database tmDb, string  username, string passwordHash, string email, string firstname, string lastname, string note , int groupId)
    	{			
    		var userId = Guid.NewGuid().hash();  //10000000.random();//10.randomNumbers().toInt();
			if (userId < 0)						// find a .net that does this (maybe called 'invert')
				userId = -userId;
			"...Creating new user: {0} with id {1}".debug(username, userId);
			
			if (groupId ==0)				//set default user type			
				groupId = 2;				//by default new users are of type 2 (i.e. Reader)
				
    		var tmUser = new TMUser {
    									UserID 		= userId,
    									UserName 	= Encoder.XmlEncode(username),
    									FirstName 	= Encoder.XmlEncode(firstname),
    									LastName 	= Encoder.XmlEncode(lastname),
    									Company 	= "",
										GroupID 	= groupId,
										Title 		= "", 										
    									EMail 		= Encoder.XmlEncode(email) ?? ""   									 
    								};										
			
			TM_Xml_Database.TMUsers.Add(tmUser);
			//tmDb.setUserPassword_PwdInClearText(username, passwordHash);
			tmDb.setUserPassword(username, passwordHash);
						
			//save it
			tmDb.saveTmUserDataToDisk(); 
						
			
			return userId;    		
    	}
		
    	public static bool setUserPassword(this TM_Xml_Database tmDb, int userId, string passwordHash)
		{
			var tmUser = tmDb.tmUser(userId);
			return tmDb.setUserPassword(tmUser, passwordHash);
		}
		
		public static bool setUserPassword(this TM_Xml_Database tmDb, string username, string passwordHash)
		{
			var tmUser = tmDb.tmUser(username);
			return tmDb.setUserPassword(tmUser, passwordHash);
		}
		
		public static bool setUserPassword(this TM_Xml_Database tmDb, TMUser tmUser, string passwordHash)
    	{		
			"in setUserPassword".info();
			if (tmUser.notNull())
			{
				"tmUser was not null".info();
				if (TM_Xml_Database.TMUsersPasswordHashes[tmUser.UserName].isNull())
					TM_Xml_Database.TMUsersPasswordHashes.add(tmUser.UserName, passwordHash);
				else
				{
					//to deal with lack of setter in O2 Items object 
					foreach(var item in TM_Xml_Database.TMUsersPasswordHashes)  
						if (item.Key == tmUser.UserName)
							item.Value = passwordHash;				
				}
				tmDb.saveTmUserDataToDisk(); 
				return true;
			}
			return false;    		
    	}
		    	
		public static bool setUserPassword_PwdInClearText(this TM_Xml_Database tmDb, string username, string password)
    	{
			return tmDb.setUserPassword(username,username.createPasswordHash(password));    		
    	}
    	
    	public static Guid login(this TM_Xml_Database tmDb, string username, string passwordHash)
    	{			
            tmDb.sleep(TM_Xml_Database.FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION);      // to slow down brute force attacks
    	    if(username.valid() && passwordHash.valid())
			    if (TM_Xml_Database.TMUsersPasswordHashes[username] == passwordHash)
				    return tmDb.registerUserSession(tmDb.tmUser(username), Guid.NewGuid());
    		return Guid.Empty;    			
    	}
    	
		
    	public static Guid login_PwdInClearText(this TM_Xml_Database tmDb, string username, string password)
    	{	
		    tmDb.sleep(TM_Xml_Database.FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION);  // to slow down brute force attacks
    		if(username.valid() && password.valid())
			    if (TM_Xml_Database.TMUsersPasswordHashes[username] == username.createPasswordHash(password))
				    return tmDb.registerUserSession(tmDb.tmUser(username), Guid.NewGuid());				
    		return Guid.Empty;    			
    	}    	
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 		
		public static List<bool> deleteTmUsers(this TM_Xml_Database tmDb, List<int> userIds)
		{
			var results = new List<bool>();
			foreach(var userId in userIds)
				results.Add(tmDb.deleteTmUser(userId));
			return results;			
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static bool deleteTmUser(this TM_Xml_Database tmDb, int userId)
		{
			var result = TM_Xml_Database.TMUsers.delete(userId);
			if (result)
				tmDb.saveTmUserDataToDisk(); 
			return result;
		}
		
		//[PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
		public static int createTmUser(this TM_Xml_Database tmDb, NewUser newUser)
		{			
			if (newUser.groupId !=0)		// if there is a groupId provided we must check if the user has the manageUsers Priviledge						
				UserRole.ManageUsers.demand();			
			if (newUser.username.inValid() ||  tmDb.tmUser(newUser.username).notNull())
				return 0;						   			
			return tmDb.newUser(newUser.username, newUser.passwordHash, newUser.email, newUser.firstname, newUser.lastname, newUser.note, newUser.groupId );
		}							
		
		[PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
		public static List<int> createTmUsers(this TM_Xml_Database tmDb, List<NewUser> newUsers)
		{						
			var newUsersIds = new List<int>();
			foreach(var newUser in newUsers)
				newUsersIds.Add(tmDb.createTmUser(newUser));
			return newUsersIds;
		}
		
		
		public static List<int> createTmUsers(this TM_Xml_Database tmDb, string batchUserData) 
		{						
			var newUsers = new List<NewUser>();
			foreach(var line in batchUserData.fixCRLF().split_onLines())
			{
				var newUser = new NewUser();
				//return _newUser;
				var items = line.split(",");
				
				newUser.username = items.size()>0 ? items[0].trim() : "";	 
				newUser.passwordHash = items.size()>1 ? newUser.username.createPasswordHash(items[1].trim()) : "";	 
				newUser.firstname = items.size()>2 ? items[2].trim() : "";	 
				newUser.lastname = items.size()>3 ? items[3].trim() : "";	 
				newUser.groupId = items.size()>4 ? items[4].trim().toInt() : 0;	 
				newUsers.Add(newUser);
			} 
			return tmDb.createTmUsers(newUsers);
		}
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static bool updateTmUser(this TM_Xml_Database tmDb, int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId)
		{
			var result = TM_Xml_Database.TMUsers.updateUser(userId, userName, firstname, lastname,  title, company, email, groupId);
			if (result) //save it			
				tmDb.saveTmUserDataToDisk(); 
			return result;
		}		
				
		public static string getUserGroupName(this TM_Xml_Database tmDb, int userId)
		{
			var tmUser = tmDb.tmUser(userId);
			if (tmUser.notNull())
				return tmUser.userGroup().str();
			return null;
		}
		
		public static int getUserGroupId(this TM_Xml_Database tmDb, int userId)
		{			
			var tmUser = tmDb.tmUser(userId);
			if (tmUser.notNull())
				return tmUser.GroupID;
			return -1;
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static List<string> getUserRoles(this TM_Xml_Database tmDb, int userId)
		{
			var tmUser = tmDb.tmUser(userId);
			if (tmUser.notNull()) 
				return tmUser.userGroup().userRoles().toStringList();
			return new List<string>();
		}
		
		[PrincipalPermission(SecurityAction.Demand, Role = "ManageUsers")] 
		public static bool setUserGroupId(this TM_Xml_Database tmDb, int userId, int groupId)
		{			
			var tmUser = tmDb.tmUser(userId);
			if (tmUser.notNull()) 
			{
				tmUser.GroupID = groupId;
				tmDb.saveTmUserDataToDisk(); 
				return true;
			}
			return false;
		}
		//
	}	
	
	public static class TM_Xml_Database_ExtensionMethods_ActiveSessions
	{
		public static Guid registerUserSession(this string userName, Guid userGuid)
		{
			var tmUser = userName.tmUser();			
			return tmUser.registerUserSession(userGuid);
		}

		public static Guid registerUserSession(this TMUser tmUser, Guid userGuid)
		{
			try
			{
				if (tmUser.notNull())
				{
					TM_Xml_Database.ActiveSessions.add(userGuid, tmUser);
					return userGuid;
				}
			}
			catch (Exception ex)
			{
				ex.log();
			}
			return Guid.Empty;
		}
				
		
		public static Guid registerUserSession(this TM_Xml_Database tmDb, TMUser tmUser, Guid userGuid)
		{
			TM_Xml_Database.ActiveSessions.add(userGuid, tmUser);
			return userGuid; 
		}
		
		public static Dictionary<Guid, TMUser> activeSessions(this TM_Xml_Database tmDb)
		{
			return TM_Xml_Database.ActiveSessions;
		}		
		
		public static bool validSession(this Guid sessionID)
		{
			return TM_Xml_Database.ActiveSessions.hasKey(sessionID);
		}

        public static bool invalidateSession(this Guid sessionID)
		{
            if (sessionID.validSession())
            {
                TM_Xml_Database.ActiveSessions.Remove(sessionID);
                return true;
            }
            return false;
		}
		
		public static TMUser session_TmUser(this Guid sessionID)
		{
			if(sessionID.validSession())
				return TM_Xml_Database.ActiveSessions[sessionID];
			return null;	
		}
		public static string session_UserName(this Guid sessionID)
        {
			"resolving username for sessionID: {0}".info(sessionID);
			if(sessionID.validSession())
				return sessionID.session_TmUser().UserName;
            
			
//            if (sessionID != null && sessionID != Guid.Empty)            
//                return ObjectFactory.AuthenticationManagement().LookupUsernameFromSessionID(sessionID);
            return null;
        }

        public static int session_GroupID(this Guid sessionID)
        { 
            var tmUser = sessionID.session_TmUser();
            if (tmUser != null)
                return tmUser.GroupID;
            return -1;            
        }
        
        public static UserGroup session_UserGroup(this Guid sessionID)
        {
            return (UserGroup)sessionID.session_GroupID();              
        }
		
		public static List<UserRole> session_UserRoles(this Guid sessionID)
        {
			var userGroup = sessionID.session_UserGroup();
			if (UserRolesMappings.Mappings.hasKey(userGroup))
				return UserRolesMappings.Mappings[userGroup];
			return new List<UserRole>();
        }
        
        public static bool session_isAdmin(this Guid sessionID)
        {
            return UserGroup.Admin == sessionID.session_UserGroup();
        }  

    }
    
}