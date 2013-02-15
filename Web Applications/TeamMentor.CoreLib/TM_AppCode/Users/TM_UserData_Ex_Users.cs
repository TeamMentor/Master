using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Security.Application;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public static class TM_UserData_Ex_Users
    {
        public static int           FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION = 500;

        public static int           createDefaultAdminUser      (this TM_Xml_Database tmDb)
        {  
            var tmConfig = TMConfig.Current;
            lock (tmConfig)
            {                
                var defaultAdminUser_Name = tmConfig.DefaultAdminUserName;
                var defaultAdminUser_Pwd = tmConfig.DefaultAdminPassword;                
                var adminUser = tmDb.tmUser(defaultAdminUser_Name);
                
                if (adminUser.notNull())
                {
                    if (adminUser.PasswordHash.notValid() || tmConfig.OnInstallation.ForceAdminPasswordReset)
                        adminUser.PasswordHash = adminUser.createPasswordHash(defaultAdminUser_Pwd);
                    adminUser.Stats.ExpirationDate = default(DateTime);
                    adminUser.saveTmUser();
                    return adminUser.UserID;
                }				

                var userId = tmDb.newUser(defaultAdminUser_Name, defaultAdminUser_Pwd, 1);                
                return userId;
            }            
        }        

        public static TMUser        tmUser                      (this string name)
        {
            return TM_Xml_Database.Current.tmUser(name);
        }        
        public static List<int>     userIds                     (this List<TMUser> tmUsers)
        {
            return (from tmUser in tmUsers
                    where tmUser.notNull()
                    select tmUser.UserID).toList();
        }        
        public static int           newUser                     (this TM_Xml_Database tmDb)
        {
            return tmDb.newUser("test_user_{0}".format(5.randomLetters()));
        }        
        public static int           newUser                     (this TM_Xml_Database tmDb, string  username)
        {
            return tmDb.newUser(username, 5.randomLetters());
        }                
        public static int           newUser                     (this TM_Xml_Database tmDb, string  username, string password)
        {
            return tmDb.newUser(username,password, 0);
        }        
        public static int           newUser                     (this TM_Xml_Database tmDb, string  username, string password, int groupId)
        {
            return tmDb.newUser(username, password, "","","","", "","",groupId);
        }        
        public static int           newUser                     (this TM_Xml_Database tmDb, string  username, string password, string email, string firstname, string lastname, string note , string title, string company, int groupId)
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
            };
            tmUser.PasswordHash = tmUser.createPasswordHash(password);
            TM_UserData.Current.TMUsers.Add(tmUser);            
        
            //save it
            tmUser.saveTmUser();            
                    
            return userId;    		
        }				                
        public static bool          setUserPassword             (this TM_Xml_Database tmDb, int userId, string password)
        {
            var tmUser = tmDb.tmUser(userId);
            return tmDb.setUserPassword(tmUser, password);
        }        
        public static bool          setUserPassword             (this TM_Xml_Database tmDb, string username, string password)
        {
            var tmUser = tmDb.tmUser(username);
            return tmDb.setUserPassword(tmUser, password);
        }
        public static bool          setCurrentUserPassword      (this TM_Xml_Database tmDb, TM_Authentication tmAuthentication, string password)
        {
            var tmUser = tmAuthentication.currentUser;
            if (tmUser.notNull())
            {
                tmUser.PasswordHash = tmUser.createPasswordHash(password);
                tmUser.saveTmUser();
                return true;
            }
            return false;
        }        
        public static bool          setUserPassword_PwdInClearText(this TM_Xml_Database tmDb, string username, string password)
        {
            return tmDb.setUserPassword(username,password);    		
        }    	
        public static Guid          login                       (this TM_Xml_Database tmDb, string username, string password)
        {
            try
            {
                tmDb.sleep(FORCED_MILLISEC_DELAY_ON_LOGIN_ACTION, false);      // to slow down brute force attacks
                if (username.valid() && password.valid())
                {
                    var tmUser = TM_UserData.Current.TMUsers.user(username);
                
                    if (TMConfig.Current.Eval_Accounts.Enabled)
                        if (tmUser.notNull() &&tmUser.Stats.ExpirationDate < DateTime.Now && tmUser.Stats.ExpirationDate != default(DateTime))
                        {
                            tmUser.logUserActivity("Account Expired",tmUser.UserName);
                            return Guid.Empty;
                        }

                    if (tmUser.notNull() && tmUser.PasswordHash == tmUser.createPasswordHash(password)) 
                        return tmUser.registerUserSession(Guid.NewGuid());
                }
            }
            catch (Exception ex)
            {
                ex.log("[TM_Xml_Database] login");                
            }
            return Guid.Empty;    			
        }

        public static List<int>     createTmUsers               (this TM_Xml_Database tmDb, string batchUserData) 
        {						
            var newUsers = new List<NewUser>();
            foreach(var line in batchUserData.fixCRLF().split_onLines())
            {
                var newUser = new NewUser();
                //return _newUser;
                var items = line.split(",");
                
                newUser.username = items.size()>0 ? items[0].trim() : "";	 
                newUser.password = items.size()>1 ? items[1].trim() : "";	 
                newUser.firstname = items.size()>2 ? items[2].trim() : "";	 
                newUser.lastname = items.size()>3 ? items[3].trim() : "";	 
                newUser.groupId = items.size()>4 ? items[4].trim().toInt() : 0;	 
                newUsers.Add(newUser);
            } 
            return tmDb.createTmUsers(newUsers);
        }
        public static string        getUserGroupName            (this TM_Xml_Database tmDb, int userId)
        {
            var tmUser = tmDb.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.userGroup().str();
            return null;
        }        
        public static int           getUserGroupId              (this TM_Xml_Database tmDb, int userId)
        {			
            var tmUser = tmDb.tmUser(userId);
            if (tmUser.notNull())
                return tmUser.GroupID;
            return -1;
        }        
        public static int           createTmUser(this TM_Xml_Database tmDb, NewUser newUser)
        {			
            if (newUser.groupId !=0)		// if there is a groupId provided we must check if the user has the manageUsers Priviledge						
                UserRole.ManageUsers.demand();			
            if (newUser.username.inValid() ||  TM_UserData.Current.TMUsers.user(newUser.username).notNull())
                return 0;
            return tmDb.newUser(newUser.username, newUser.password, newUser.email, newUser.firstname, newUser.lastname, newUser.note, newUser.title, newUser.company, newUser.groupId);						
        }
        [ManageUsers]   public static TMUser        tmUser(this TM_Xml_Database tmDb, string name)
        {
            return TM_UserData.Current.TMUsers.user(name);
        }
        [ManageUsers]   public static TMUser        tmUser(this TM_Xml_Database tmDb, int userId)
        {
            return TM_UserData.Current.TMUsers.user(userId);
        }        
        [ManageUsers]   public static List<TMUser>  tmUsers(this List<int> usersId)
        {
            return usersId.Select(userId => TM_UserData.Current.TMUsers.user(userId)).toList();
        }

        [ManageUsers]   public static List<TMUser>  tmUsers(this TM_Xml_Database tmDb)
        {
            return TM_UserData.Current.TMUsers.toList();
        }        
        [ManageUsers]   public static bool          setUserPassword(this TM_Xml_Database tmDb, TMUser tmUser, string password)
        {		
            "in setUserPassword".info();
            if (tmUser.notNull())
            {
                tmUser.PasswordHash = tmUser.createPasswordHash(password);
                tmUser.saveTmUser();
                return true;
            }
            return false;    		
        }                
        [ManageUsers]   public static List<bool>    deleteTmUsers(this TM_Xml_Database tmDb, List<int> userIds)
        {
            return userIds.Select(userId => tmDb.deleteTmUser(userId)).toList();
        }

        [ManageUsers]   public static bool          deleteTmUser(this TM_Xml_Database tmDb, int userId)
        {
            return TM_UserData.Current.TMUsers.delete(userId);            
        }		
        [ManageUsers]   public static bool          updateTmUser(this TM_Xml_Database tmDb, int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId)
        {
            return TM_UserData.Current.TMUsers.updateUser(userId, userName, firstname, lastname,  title, company, email, groupId);
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
                tmUser.saveTmUser();                
                return true;
            }
            return false;
        }        									
        [ManageUsers]   public static List<int>     createTmUsers(this TM_Xml_Database tmDb, List<NewUser> newUsers)
        {
            return newUsers.Select(newUser => tmDb.createTmUser(newUser)).toList();
        }

        [Admin]         public static TMUser        set_PostLoginView(this TMUser tmUser, string postLoginView)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginView = postLoginView;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }
        [Admin]         public static TMUser        set_PostLoginScript(this TMUser tmUser, string postLoginScript)
        {
            if (tmUser.notNull())
            {
                tmUser.PostLoginScript = postLoginScript;
                tmUser.saveTmUser();                
            }
            return tmUser;
        }
    }
}