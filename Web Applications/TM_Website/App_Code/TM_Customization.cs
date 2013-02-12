using TeamMentor.CoreLib;

namespace TeamMentor.Website
{
    public class TM_Customizations
    {
	    public TMConfig			tmConfig;
	    public GoogleAnalytics	googleAnalytics;

	    public static void AppInitialize()
	    {		 
		    new TM_Customizations().CustomizeTMConfig();			
	    }

	    public TM_Customizations()
	    {
			tmConfig = TMConfig.Current;
			googleAnalytics =  GoogleAnalytics.Current;
	    }

	    public void CustomizeTMConfig()
	    {	
	    	//General TM Configuration
			tmConfig.ShowContentToAnonymousUsers	= false;
			tmConfig.Eval_Accounts.Enabled			= true;
			tmConfig.Eval_Accounts.Days				= 15;

	        tmConfig.TMDebugAndDev.EnableGZipForWebServices = false;//true;
            tmConfig.TMDebugAndDev.Enable302Redirects       = true;


			//Google Analytics
		    googleAnalytics.AccountID				= "UA-37594728-3";			
			googleAnalytics.Enabled					= true;
			googleAnalytics.LogWebServicesCalls		= true;

			//TM Security
			tmConfig.SSL_RedirectHttpToHttps		= true;

            //OnInstallation
	        tmConfig.OnInstallation.ForceAdminPasswordReset          = true;
	        tmConfig.OnInstallation.DefaultLibraryToInstall_Name     = "Top Vulnerabilities";
            tmConfig.OnInstallation.DefaultLibraryToInstall_Location = "https://github.com/TeamMentor/Library_Top_Vulnerabilities/zipball/master";

	    }
    }
}
