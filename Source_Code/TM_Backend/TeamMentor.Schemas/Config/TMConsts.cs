namespace TeamMentor.CoreLib
{
	public class TMConsts
	{
        public static string AUTH_TOKEN_REQUEST_VAR_NAME                    = "auth";
		public static string PATH_HTML_PAGE_UNAVAILABLE                     = "/Html_Pages/Gui/Pages/unavailable.html";      

        public static string DEFAULT_ARTICLE_FOLDER_NAME                    = "Articles";
        public static string VIRTUAL_PATH_MAPPING			                = "..\\..\\..";					  
        public static string XML_DATABASE_VIRTUAL_PATH		                = "TeamMentor";
		public static string XML_DATABASE_VIRTUAL_PATH_LEGACY               = "Library_Data\\XmlDatabase"; // for legacy support (pre 3.3)
                
        public static string DEFAULT_TM_LOCALHOST_SERVER_URL                = "http://localhost";
	    public static string APPLICATION_LOGS_FOLDER_NAME                   = "Application_Logs";

        public static string USERDATA_FIRST_SCRIPT_TO_INVOKE                = "H2Scripts//FirstScriptToInvoke.h2";
        public static string USERDATA_PATH_USER_XML_FILES                   = "Users";        
        public static string USERDATA_PATH_WEB_ROOT_FILES                   = "WebRoot_Files";
        public static string USERDATA_FORMAT_USER_XML_FILE                  = "{0}_{1}.userData.xml";
        public static string USERDATA_DEFAULT_ADMIN_USERNAME                = "admin";
        public static string USERDATA_DEFAULT_ADMIN_PASSWORD                = "!!tmadmin";        
        public static string USERDATA_DEFAULT_ADMIN_EMAIL                   = "TM_alerts@securityinnovation.com";
        public static string DEFAULT_LOGIN_ERROR_MESSAGE                    = "An error occurred";
        public static string DEFAULT_SIGNUP_ERROR_MESSAGE                   = "An error occurred creating a new account";
        public static string DEFAULT_ACCOUNT_EXPIRED_MESSAGE                = "Account Expired";
        public static string DEFAULT_ACCOUNT_DISABLED_MESSAGE               = "Account Disabled";
        public static string DEFAULT_LOGIN_PASSWORD_DONOT_MATCH_MESSAGE     = "Wrong username or bad password";
	    public static string DEFAULT_LOGIN_USERNAME_DONOT_EXIST_MESSAGE     = "Wrong username or bad password";
        public static string DEFAULT_SIGNUP_USERNAME_EXIST_MESSAGE          = "Username already exist";
        public static string DEFAULT_SIGNUP_EMAIL_EXIST_MESSAGE             = "Email already exist";
        public static string DEFAULT_PASSWORD_LENGTH_MESSAGE                = "Password must be 8 to 256 character long";
	    public static string DEFAULT_PASSWORD_COMPLEXITY_ERROR_MESSAGE      = "Password should be at least 8 characters long. It should have one uppercase and one lowercase letter, a number and a special character.";
        public static string DEFAULT_NEW_PASSWORD_ERROR_MESSAGE             = "The new password must differ from your current password";
        public static string CURRENT_PASSWORD_DONOT_MATCH_ERROR_MESSAGE     = "Current Password does not match provided value";
        public static string DEFAULT_PASSWORD_CHANGE_ERROR_MESSAGE          = "The password cannot be changed at this time";
        public static string DEFAULT_PASSWORD_RESET_ERROR_MESSAGE           = "The password cannot be reset at this time";
	    public static string DEFAULT_EMAIL_DOES_NOT_EXIST_ERROR_MESSAGE     = "Email address does not exist";
	    public static string DEFAULT_EMAIL_ADDRESS_IS_INVALID               = "This is not a valid e-mail address";

        public static int    FIREBASE_SUBMIT_QUEUE_MAX_WAIT                 = 60000;
        public static string FIREBASE_AREA_DEBUG_MSGS                       = "debugMsgs";
        public static string FIREBASE_AREA_ACTIVITIES                       = "activities";
        public static string FIREBASE_AREA_REQUESTS_URLS                    = "requestUrls";
        
        public static string EMAIL_SUBJECT_NEW_USER_WELCOME                 = "Welcome to TeamMentor";
        public static string EMAIL_TEST_SUBJECT                             = "Test Email";
        public static string EMAIL_TEST_MESSAGE                             = "From TeamMentor";

        
        public static string TM_CONFIG_FILENAME                             = "TMConfig.config";

        public static string TM_SERVER_FILENAME                             = "TMServer.config";
        public static string TM_SERVER_DEFAULT_NAME_USERDATA                = "User_Data";
        public static string TM_SERVER_DEFAULT_NAME_SITEDATA                = "Site_Data";

        public static string NGIT_DEFAULT_AUTHOR_NAME                       = "tm-bot";
        public static string NGIT_DEFAULT_AUTHOR_EMAIL                      = "tm-bot@teammentor.net";

        public static string DEFAULT_ERROR_PAGE_REDIRECT                    = "/error";        
        public static string EMAIL_BODY_NEW_USER_WELCOME                    =
@"Hello,

It's a pleasure to confirm that a new TeamMentor account has been created for you and that you'll now be able to access
the entire set of guidance available in the TM repository.

To access the service:

- Go to {0} and login at the top right-hand corner of the page.
- Use your username : {1}

Thanks,

";
        public static string EMAIL_DEFAULT_FOOTER =
@"The TEAMMentor team. 

";        
	


        public static string EMAIL_BODY_NEW_USER_NEEDS_APPROVAL =
@"Hello,

Thanks for creating an account in TeamMentor. 

The account is being reviewed and you will be notified if it is approved.

Thanks,

";

        public static string EMAIL_BODY_ADMIN_EMAIL_ON_NEW_USER = 
@"New TeamMentor User Created:

    UserId: {0}
    UserName: {1}
    Company: {2}
    Email: {3}
    FirstName: {4}
    LastName: {5}
    Title: {6}
    Creation Date: {7}";

        public static string EMAIL_BODY_ADMIN_NEW_USER_APPROVAL = 
@"Hello TeamMentor admin, the following new User request as been received:

    UserId: {0}
    UserName: {1}
    Company: {2}
    Email: {3}
    FirstName: {4}
    LastName: {5}
    Title: {6}
    Creation Date: {7}

Please click on this link if you want to approve it: {8}

If you don't want to enable this account, you can ignore this email and the account will stay in the current disabled state.

If you want to see this user details (or delete it), please visit: {9}
";
        

    }
}