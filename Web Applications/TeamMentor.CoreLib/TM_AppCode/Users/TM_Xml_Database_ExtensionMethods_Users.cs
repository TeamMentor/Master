using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Microsoft.Security.Application;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_Users
    {   		
        public static int           createDefaultAdminUser(this TM_Xml_Database tmDb)
        {  
            var tmConfig = TMConfig.Current;
            lock (tmConfig)
            {
                UserGroup.Admin.setThreadPrincipalWithRoles();

                var defaultAdminUser_name = tmConfig.DefaultAdminUserName;
                var defaultAdminUser_pwd = tmConfig.DefaultAdminPassword;
                var passwordHash = defaultAdminUser_name.createPasswordHash(defaultAdminUser_pwd);
                var adminUser = tmDb.tmUser(defaultAdminUser_name);
                
                if (adminUser.notNull())
                {
                    if (adminUser.PasswordHash.notValid())
                        adminUser.PasswordHash = passwordHash;
                    adminUser.Stats.ExpirationDate = default(DateTime);
                    //"[TM_Xml_Database] in TMUser createDefaultAdminUser, defaultAdminUser_name already existed in the database (returning its Id): {0}".debug(defaultAdminUser_name);
                    return adminUser.UserID;
                }				

                var userId = tmDb.newUser(defaultAdminUser_name, passwordHash, 1);
                UserGroup.Anonymous.setThreadPrincipalWithRoles();
                return userId;
            }            
        }        
        public static TMUser        tmUser(this string name)
        {
            return TM_Xml_Database.Current.tmUser(name);
        }        
        public static List<int>     userIds(this List<TMUser> tmUsers)
        {
            return (from tmUser in tmUsers
                    where tmUser.notNull()
                    select tmUser.UserID).toList();
        }        
        public static int           newUser(this TM_Xml_Database tmDb)
        {
            return tmDb.newUser("test_user_{0}".format(5.randomLetters()));
        }        
        public static int           newUser(this TM_Xml_Database tmDb, string  username)
        {
            return tmDb.newUser(username, username.createPasswordHash(5.randomLetters()));
        }        
        public static int           newUser_ClearTextPassword(this TM_Xml_Database tmDb, string  username, string password)
        {
            return tmDb.newUser(username, username.createPasswordHash(password));
        }        
        public static int           newUser(this TM_Xml_Database tmDb, string  username, string passwordHash)
        {
            return tmDb.newUser(username,passwordHash, 0);
        }        
        public static int           newUser(this TM_Xml_Database tmDb, string  username, string passwordHash, int groupId)
        {
            return tmDb.newUser(username, passwordHash, "","","","", "","",groupId);
        }        
        public static int           newUser(this TM_Xml_Database tmDb, string  username, string passwordHash, string email, string firstname, string lastname, string note , string title, string company, int groupId)
        {			
            var userId = Guid.NewGuid().hash();  //10000000.random();//10.randomNumbers().toInt();
            if (userId < 0)						// find a .net that does this (maybe called 'invert')
                userId = -userId;
            "...Creating new user: {0} with id {1}".debug(username, userId);
            
            if (groupId <1)						//set default user type						
                groupId = 2;					//by default new users are of type 2 (i.e. Reader)
            else
                UserRole.ManageUsers.demand();	// only users with UserRole.ManageUsers should be able to create non-default users
                
            var tmUser = new TMUser {
                UserID 		 = userId,
                UserName 	 = Encoder.XmlEncode(username),
                FirstName 	 = Encoder.XmlEncode(firstname),
                LastName 	 = Encoder.XmlEncode(lastname),
                Company 	 = Encoder.XmlEncode(company),
                GroupID 	 = groupId,
                Title 		 = Encoder.XmlEncode(title), 										
                EMail 		 = Encoder.XmlEncode(email) ?? "",
                PasswordHash = passwordHash
            };										
            
            TM_Xml_Database.Current.TMUsers.Add(tmUser);
            //tmDb.setUserPassword_PwdInClearText(username, passwordHash);
            //tmDb.setUserPassword(username, passwordHash);
        
            //save it
            tmDb.saveTmUserDataToDisk();   // saved on setUserPassword
        
            
            return userId;    		
        }				                
        public static bool          setUserPassword(this TM_Xml_Database tmDb, int userId, string passwordHash)
        {
            var tmUser = tmDb.tmUser(userId);
            return tmDb.setUserPassword(tmUser, passwordHash);
        }        
        public static bool          setUserPassword(this TM_Xml_Database tmDb, string username, string passwordHash)
        {
            var tmUser = tmDb.tmUser(username);
            return tmDb.setUserPassword(tmUser, passwordHash);
        }
        public static bool          setCurrentUserPassword(this TM_Xml_Database tmDb, TM_Authentication tmAuthentication, string passwordHash)
        {
            var tmUser = tmAuthentication.currentUser;
            if (tmUser.notNull())
            {
                tmUser.PasswordHash = passwordHash;
                tmDb.saveTmUserDataToDisk(); 
                return true;
            }
            return false;
        }        
        public static bool          setUserPassword_PwdInClearText(this TM_Xml_Database tmDb, string username, string password)
        {
            return tmDb.setUserPassword(username,username.createPasswordHash(password));    		
        }    	
        public static Guid          login(this TM_Xml_Database tmDb, string username, string passwordHash)
        {
            tmDb.sleep(TM_Xml_Database.FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION, false);      // to slow down brute force attacks
            if (username.valid() && passwordHash.valid())
            {
                var tmUser = tmDb.TMUsers.user(username);
                
                if (TMConfig.Current.Eval_Accounts.Enabled)
                    if (tmUser.notNull() &&tmUser.Stats.ExpirationDate < DateTime.Now 
                                               && tmUser.Stats.ExpirationDate != default(DateTime))
                    {
                        "Account Expired".logActivity(tmUser.UserName);
                        return Guid.Empty;
                    }

                if (tmUser.notNull() && tmUser.PasswordHash == passwordHash)
                    //if (TM_Xml_Database.Current.TMUsersPasswordHashes[username] == passwordHash)					
                    return tmDb.registerUserSession(tmUser, Guid.NewGuid());
            }
            return Guid.Empty;    			
        }
        public static Guid          login_PwdInClearText(this TM_Xml_Database tmDb, string username, string password)
        {
            tmDb.sleep(TM_Xml_Database.FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION, false);  // to slow down brute force attacks
            if (username.valid() && password.valid())
            {
                var tmUser = tmDb.TMUsers.user(username);
                if (tmUser.notNull() && tmUser.PasswordHash == username.createPasswordHash(password))
                //if (TM_Xml_Database.Current.TMUsersPasswordHashes[username] == username.createPasswordHash(password))
                    return tmDb.registerUserSession(tmUser, Guid.NewGuid());
            }
            return Guid.Empty;    			
        }    	                
        public static List<int>     createTmUsers(this TM_Xml_Database tmDb, string batchUserData) 
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
        public static string        getUserGroupName(this TM_Xml_Database tmDb, int userId)
        {
            var tmUser = tmDb.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.userGroup().str();
            return null;
        }        
        public static int           getUserGroupId(this TM_Xml_Database tmDb, int userId)
        {			
            var tmUser = tmDb.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.GroupID;
            return -1;
        }        

        [ManageUsers]   public static TMUser        tmUser(this TM_Xml_Database tmDb, string name)
        {
            return TM_Xml_Database.Current.TMUsers.user(name);
        }
        [ManageUsers]   public static TMUser        tmUser(this TM_Xml_Database tmDb, int userId)
        {
            return TM_Xml_Database.Current.TMUsers.user(userId);
        }        
        [ManageUsers]   public static List<TMUser>  tmUsers(this List<int> usersId)
        {
            var tmUsers = new List<TMUser>();
            foreach(var userId in usersId)								
                tmUsers.Add(TM_Xml_Database.Current.TMUsers.user(userId));
            return tmUsers;
        }                
        [ManageUsers]   public static List<TMUser>  tmUsers(this TM_Xml_Database tmDb)
        {
            return TM_Xml_Database.Current.TMUsers.toList();
        }        
        [ManageUsers]   public static bool          setUserPassword(this TM_Xml_Database tmDb, TMUser tmUser, string passwordHash)
        {		
            "in setUserPassword".info();
            if (tmUser.notNull())
            {
                tmUser.PasswordHash = passwordHash;
                tmDb.saveTmUserDataToDisk(); 
                return true;
            }
            return false;    		
        }                
        [ManageUsers]   public static List<bool>    deleteTmUsers(this TM_Xml_Database tmDb, List<int> userIds)
        {
            var results = new List<bool>();
            foreach(var userId in userIds)
                results.Add(tmDb.deleteTmUser(userId));
            return results;			
        }				
        [ManageUsers]   public static bool          deleteTmUser(this TM_Xml_Database tmDb, int userId)
        {
            var result = TM_Xml_Database.Current.TMUsers.delete(userId);
            if (result)
                tmDb.saveTmUserDataToDisk(); 
            return result;
        }		
        [ManageUsers]   public static bool          updateTmUser(this TM_Xml_Database tmDb, int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId)
        {
            var result = TM_Xml_Database.Current.TMUsers.updateUser(userId, userName, firstname, lastname,  title, company, email, groupId);
            if (result) //save it			
                tmDb.saveTmUserDataToDisk(); 
            return result;
        }		                
        [ManageUsers]   public static List<string>  getUserRoles(this TM_Xml_Database tmDb, int userId)
        {
            var tmUser = tmDb.tmUser(userId);
            if (tmUser.notNull()) 
                return tmUser.userGroup().userRoles().toStringList();
            return new List<string>();
        }        
        [ManageUsers]   public static bool          setUserGroupId(this TM_Xml_Database tmDb, int userId, int groupId)
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
        [ManageUsers]   public static int           createTmUser(this TM_Xml_Database tmDb, NewUser newUser)
        {			
            if (newUser.groupId !=0)		// if there is a groupId provided we must check if the user has the manageUsers Priviledge						
                UserRole.ManageUsers.demand();			
            if (newUser.username.inValid() ||  tmDb.TMUsers.user(newUser.username).notNull())
                return 0;
            return tmDb.newUser(newUser.username, newUser.passwordHash, newUser.email, newUser.firstname, newUser.lastname, newUser.note, newUser.title, newUser.company, newUser.groupId);						
        }									
        [ManageUsers]   public static List<int>     createTmUsers(this TM_Xml_Database tmDb, List<NewUser> newUsers)
        {						
            var newUsersIds = new List<int>();
            foreach(var newUser in newUsers)
                newUsersIds.Add(tmDb.createTmUser(newUser));
            return newUsersIds;
        }				
        [Admin]         public static TMUser        set_PostLoginView(this TMUser tmUser, string postLoginView)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginView = postLoginView;
                TM_Xml_Database.Current.saveTmUserDataToDisk();
            }
            return tmUser;
        }
        [Admin]         public static TMUser        set_PostLoginScript(this TMUser tmUser, string postLoginScript)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginScript = postLoginScript;
                TM_Xml_Database.Current.saveTmUserDataToDisk();
            }
            return tmUser;
        }
    }
}