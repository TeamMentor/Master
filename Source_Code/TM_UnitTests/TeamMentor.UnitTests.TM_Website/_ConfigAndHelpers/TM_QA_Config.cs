using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentSharp.CoreLib;
using FluentSharp.Web35;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{   
    public class TM_QA_Config     
    {        
        public static bool   Force_Server_Offline { get; set; }

        public String Url_Target_TM_Site   { get; set; }        
        public String Firebase_Site        { get; set; }
        public String Firebase_AuthToken   { get; set; }
        public String Firebase_Area        { get; set; }
        public String SMTP_Server          { get; set; }
        public String SMTP_UserName        { get; set; }
        public String SMTP_Password        { get; set; }
        public String Default_Admin_Email  { get; set; }
        public String Default_Admin_User   { get; set; }
        public String Default_Admin_Pwd    { get; set; }        
        public List<Test_User> TestUsers   { get; set; }
    }

    public class Test_User
    {
        public string UserName  { get; set;}
        public string Password  { get; set;}
        public string Email     { get; set;}
        public int    GroupId { get; set;}
        public Guid   AuthToken { get; set;}
        public int    UserId    { get; set;}
    }

    public class TM_QA_Config_Loader
    {
        public string LocalFolder { get; set; }
        public string FileName    { get; set; }
        
        public TM_QA_Config_Loader() : this (Tests_Consts.TM_QA_ConfigFile_LOCAL_FOLDER, Tests_Consts.TM_QA_ConfigFile_FILE_NAME)
        {
            
        }
        public TM_QA_Config_Loader(string localFolder, string fileName)
        {
            LocalFolder = localFolder;
            FileName    = fileName;
        }
    }

    public static class TM_QA_Config_Loader_ExtensionMethods
    {        
        //Load,  Create, save
        public static string                        localFilePath       (this TM_QA_Config_Loader configLoader          ) 
        {
            return configLoader.LocalFolder.pathCombine(configLoader.FileName);            
        }
        public static TM_QA_Config                  load                (this TM_QA_Config_Loader configLoader          ) 
        {
            var localFilePath = configLoader.localFilePath();
            if (localFilePath.fileExists())
                return localFilePath.load<TM_QA_Config>();              
            return configLoader.create();            
        }
        public static TM_QA_Config                  create              (this TM_QA_Config_Loader configLoader          ) 
        {
            var tmQaConfig = new TM_QA_Config()
                {
                    Url_Target_TM_Site   = "http://localhost:3187",
                    Firebase_Site        = "tm-admin-test",  
                    Firebase_Area        = "Dinis_Dev",
                    Firebase_AuthToken   = "",
                    SMTP_Server          = "smtp.gmail.com",
                    SMTP_UserName        = "TM_Security_Dude@securityinnovation.com",
                    SMTP_Password        = "",
                    Default_Admin_Email  = "tm_security_dude@securityinnovation.com",
                    Default_Admin_User   = Tests_Consts.DEFAULT_ADMIN_USERNAME,
                    Default_Admin_Pwd    = Tests_Consts.DEFAULT_ADMIN_PASSWORD,                    
                };

            var targetFile   = configLoader.localFilePath();
            targetFile.parentFolder().createDir();              // ensure that is exists
            
            tmQaConfig.addDefaultTestUsers()
                      .saveAs(targetFile);
            return tmQaConfig;
        }
        
        public static TM_QA_Config                  save_QAConfig        (this TM_QA_Config qaConfig          )
        {
            if (qaConfig.notNull())
            {
                var localFilePath = new TM_QA_Config_Loader().localFilePath();
                qaConfig.saveAs(localFilePath);
            }
            return qaConfig;
        }

        //Test users
        public static List<Test_User>  testUsers           (this TM_QA_Config tmQaConfig)
        {
            return tmQaConfig.TestUsers;
        }
        public static Test_User        testUser            (this TM_QA_Config tmQaConfig, string userName  ) 
        {
            return (from testUser in tmQaConfig.TestUsers
                    where testUser.UserName ==  userName
                    select testUser).first();

        }
        public static TM_QA_Config     addDefaultTestUsers (this TM_QA_Config tmQaConfig                   ) 
        {            
            tmQaConfig.new_UserList()                      
                      .add_User("qa-admin"     , 1)                      
                      .add_User("qa-reader"    , 2)
                      .add_User("qa-editor"    , 3)
                      .add_User("qa-developer" , 4);
            return tmQaConfig;
        }
        public static List<Test_User>  new_UserList        (this TM_QA_Config tmQaConfig                   ) 
        {
            return tmQaConfig.TestUsers = new List<Test_User>();            
        }         
        public static List<Test_User>  add_User            (this List<Test_User> users, string username, int userGroup                                ) 
        {
            var password = "!!123".add_RandomLetters(10);
            var email    = "{0}@{1}.abc".format(username, 10.randomLetters());
            return users.add_User(username, password, email, userGroup);
        }
        public static List<Test_User>  add_User            (this List<Test_User> users, string username, string password , string email, int userGroup) 
        {
            users.add(new Test_User
                {
                    UserName  = username,
                    Password  = password,
                    Email     = email,
                    GroupId = userGroup
                });
            return users;
        }
        
        //Target server
        public static bool                          serverOffline       (this TM_QA_Config tmQaConfig)
        {
            return tmQaConfig.serverOffline(tmQaConfig.Url_Target_TM_Site.uri());
        }
        public static bool                          serverOffline       (this TM_QA_Config tmQaConfig, Uri targetServer) 
        {
            return tmQaConfig.serverOnline(targetServer).isFalse();
        }
        
        public static bool                          serverOnline        (this TM_QA_Config tmQaConfig)
        {
            return tmQaConfig.serverOnline(tmQaConfig.Url_Target_TM_Site.uri());    
        }
        public static bool                          serverOnline        (this TM_QA_Config tmQaConfig, Uri targetServer) 
        {            
            if (targetServer.notNull())
            {
                if (TM_QA_Config.Force_Server_Offline)
                    return false;
                return targetServer.append("default.htm")
                                   .HEAD();
            }
            return false;
        }

        public static void assertIgnore_If_Offine(this TM_QA_Config qaConfig, Uri targetServer)
        {
            if (qaConfig.serverOffline())
                Assert.Ignore("[TM_QA_Config] TM server is offline: {0}".info(targetServer));
        }
    }
        
    /*    {            
             return config_LocalFolder.pathCombine(config_FileName);            
        }

        public static TM_QA_Config Create()
        {
                    
        }
    }*/
}
