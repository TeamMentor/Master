using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_QA_Config   
    {
        public String Firebase_Site      { get; set; }
        public String Firebase_AuthToken { get; set; }
        public String SMTP_Server        { get; set; }
        public String SMTP_UserName      { get; set; }
        public String SMTP_Password      { get; set; }
        public String Default_AdminEmail { get; set; }
        public List<Test_User> testUsers { get; set; }

        public TM_QA_Config()
        {
            // so that the new file created by TM_QA_ConfigFile.Current has the property's values placeholders
            Firebase_Site       = "";            
            Firebase_AuthToken  = "";
            SMTP_Server         = "";
            SMTP_UserName       = "";
            SMTP_Password       = "";
            Default_AdminEmail  = "";
            testUsers           = new List<Test_User>();
        }

        public class Test_User
        {
            public string Username  { get; set;}
            public string Password  { get; set;}
            public string Email     { get; set;}
            public string UserGroup { get; set;}
            public string AuthToken { get; set;}
        }

        public static TM_QA_Config Current
        {
            get
            {
                var configFile = Current_Path;                 
                if (configFile.fileExists())
                {
                    //new TM_QA_Config().saveAs(configFile);                                    
                    return configFile.load<TM_QA_Config>();
                }
                return new TM_QA_Config();
            }
        }
        public static string Current_Path
        {
            get
            {
                if(HttpContextFactory.Request.isLocal())
                {
                    var configFolder = TMConsts.TM_QA_ConfigFile_LOCAL_FOLDER;
                    return configFolder.pathCombine(TMConsts.TM_QA_ConfigFile_FILE_NAME);
                }
                return "";                
            }
        }
    }
}
