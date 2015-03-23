
using System;
using System.Xml.Serialization;

namespace TeamMentor.CoreLib    
{    
    [XmlRoot(ElementName="TMConfig")]
    public class TMConfig : MarshalByRefObject
    {
        public static TMConfig              Current                     { get; set; }

        public TMSetup_Config               TMSetup                     { get; set; }
        public TMSecurity_Config			TMSecurity				    { get; set; }
        public TMErrorMessages_Config       TMErrorMessages              { get; set; }
        public WindowsAuthentication_Config WindowsAuthentication		{ get; set; }       
        public OnInstallation_Config		OnInstallation				{ get; set; }
        public VirtualArticles_Config       VirtualArticles             { get; set; }


        public TMConfig()
        {
            Current                 = this;

            TMSetup                 = new TMSetup_Config();
            TMSecurity              = new TMSecurity_Config();
            TMErrorMessages         = new TMErrorMessages_Config();
            WindowsAuthentication   = new WindowsAuthentication_Config();
            OnInstallation          = new OnInstallation_Config();            
            VirtualArticles         = new VirtualArticles_Config();
        }
                       
        public class TMSetup_Config : MarshalByRefObject
        {            
            public string       TMLibraryDataVirtualPath    { get; set; }
            public string 		XmlLibrariesPath 	        { get; set; }            		                
            public string 		LibrariesUploadedFiles	    { get; set; }	                           
            public bool         EnableGZipForWebServices	{ get; set; }
            public bool         Enable304Redirects			{ get; set; }
            public bool         ShowDotNetDebugErrors	    { get; set; }
            public bool         ShowDetailedErrorMessages   { get; set; }

            public TMSetup_Config()
            {
                TMLibraryDataVirtualPath    = "..\\..";
                XmlLibrariesPath            = "TM_Libraries";                
                LibrariesUploadedFiles      = "LibrariesUploadedFiles";
                Enable304Redirects          = true;
                EnableGZipForWebServices    = true;                  
                ShowDotNetDebugErrors       = false;
                ShowDetailedErrorMessages = true;
            }
        }

        public class TMSecurity_Config : MarshalByRefObject
        {
            public bool 		Show_ContentToAnonymousUsers { get; set; }
            public bool 		Show_LibraryToAnonymousUsers { get; set; }
            public bool         SSL_RedirectHttpToHttps      { get; set; }
            public bool         NewAccounts_Enabled          { get; set; }
            public bool         NewAccounts_DontExpire       { get; set; }            
            public bool         EvalAccounts_Enabled         { get; set; }
            public int          EvalAccounts_Days            { get; set; }
            public bool         REST_AllowCrossDomainAccess  { get; set; }                       
            public bool 		Sanitize_HtmlContent         { get; set; }            
            public string 		Default_AdminUserName        { get; set; }
            public string 		Default_AdminPassword        { get; set; }	
            public string 		Default_AdminEmail           { get; set; }
            public bool 		EmailAdmin_On_NewUsers       { get; set; }

            public TMSecurity_Config()                                          //need to do this here so that they survive serialization
            {
                Show_LibraryToAnonymousUsers = true;
                Show_ContentToAnonymousUsers = false;               
                SSL_RedirectHttpToHttps      = true;
                EvalAccounts_Enabled         = true;
                NewAccounts_Enabled          = true;
                EvalAccounts_Days            = 15;
                Default_AdminUserName        = TMConsts.USERDATA_DEFAULT_ADMIN_USERNAME;
                Default_AdminPassword        = TMConsts.USERDATA_DEFAULT_ADMIN_PASSWORD;
                Default_AdminEmail           = TMConsts.USERDATA_DEFAULT_ADMIN_EMAIL;
                EmailAdmin_On_NewUsers       = true;
            }            
        }

        public class TMErrorMessages_Config : MarshalByRefObject
        {
            public string General_Login_Error_Message           { get; set; }
            public string General_SignUp_Error_Message          { get; set; }
            public string AccountExpiredErrorMessage            { get; set; }
            public string AccountDisabledErrorMessage           { get; set; }
            public string Login_PasswordDoNotMatch              { get; set; }
            public string UserNameDoNotExistErrorMessage        { get; set; }
            public string PasswordLengthErrorMessage            { get;set; }
            public string PasswordComplexityErroMessage         { get; set; }
            public string SignUpUsernameAlreadyExist            { get; set; }
            public string SignUpEmailAlreadyExist               { get; set; }
            public string NewPassword_ErrorMessage              { get; set; } //New password is equal to current password
            public string CurrentPasswordDoNotMatch             { get; set; } //Current password does not exist.
            public string General_PasswordChange_Error_Message  { get; set; }
            public string General_PasswordReset_Error_Message   { get; set; }
            public string Email_Does_Not_Exist_ErrorMessage     { get; set; }
            public string Email_Address_Is_Invalid              { get; set; }

            public TMErrorMessages_Config()
            {
                AccountExpiredErrorMessage              = TMConsts.DEFAULT_ACCOUNT_EXPIRED_MESSAGE;
                AccountDisabledErrorMessage             = TMConsts.DEFAULT_ACCOUNT_DISABLED_MESSAGE;
                Login_PasswordDoNotMatch                = TMConsts.DEFAULT_LOGIN_PASSWORD_DONOT_MATCH_MESSAGE;
                UserNameDoNotExistErrorMessage          = TMConsts.DEFAULT_LOGIN_USERNAME_DONOT_EXIST_MESSAGE;
                PasswordLengthErrorMessage              = TMConsts.DEFAULT_PASSWORD_LENGTH_MESSAGE;
                General_Login_Error_Message             = TMConsts.DEFAULT_LOGIN_ERROR_MESSAGE;
                General_SignUp_Error_Message            = TMConsts.DEFAULT_SIGNUP_ERROR_MESSAGE;
                PasswordComplexityErroMessage           = TMConsts.DEFAULT_PASSWORD_COMPLEXITY_ERROR_MESSAGE;
                SignUpUsernameAlreadyExist              = TMConsts.DEFAULT_SIGNUP_USERNAME_EXIST_MESSAGE;
                SignUpEmailAlreadyExist                 = TMConsts.DEFAULT_SIGNUP_EMAIL_EXIST_MESSAGE;
                NewPassword_ErrorMessage                = TMConsts.DEFAULT_NEW_PASSWORD_ERROR_MESSAGE;
                CurrentPasswordDoNotMatch               = TMConsts.CURRENT_PASSWORD_DONOT_MATCH_ERROR_MESSAGE;
                General_PasswordChange_Error_Message    = TMConsts.DEFAULT_PASSWORD_CHANGE_ERROR_MESSAGE;
                General_PasswordReset_Error_Message     = TMConsts.DEFAULT_PASSWORD_RESET_ERROR_MESSAGE;
                Email_Does_Not_Exist_ErrorMessage       = TMConsts.DEFAULT_EMAIL_DOES_NOT_EXIST_ERROR_MESSAGE;
                Email_Address_Is_Invalid                = TMConsts.DEFAULT_EMAIL_ADDRESS_IS_INVALID;

            }
        }

        public class WindowsAuthentication_Config : MarshalByRefObject
        {				
            public bool		    Enabled		                { get; set; }
            public string	    ReaderGroup                 { get; set; }
            public string	    EditorGroup                 { get; set; }
            public string	    AdminGroup	                { get; set; }

            public WindowsAuthentication_Config()
            {
                Enabled               = false;
                ReaderGroup           = "TM_Reader";
                EditorGroup           = "TM_Editor";
                AdminGroup            = "TM_Admin";
            }
        }        

        public class OnInstallation_Config : MarshalByRefObject
        {
            public bool         ForceDefaultAdminPassword			    { get; set; }
            
            public OnInstallation_Config()
            {
                ForceDefaultAdminPassword          = false;                
            }
        }  

        public class VirtualArticles_Config : MarshalByRefObject
        { 
            public bool         AutoRedirectIfGuidNotFound          { get; set; }
            public string       AutoRedirectTarget                  { get; set; }

            public VirtualArticles_Config()
              {
                AutoRedirectIfGuidNotFound = false;
                AutoRedirectTarget         = "https://teammentor.net/article/";
            }
        }
    }
}