using TeamMentor.CoreLib;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.Website
{
    public class TM_Customization
    {
        public TMConfig			tmConfig;
        public GoogleAnalytics	googleAnalytics;

        public static void AppInitialize()
        {		 
            new TM_Customization().CustomizeTMConfig();			
        }

        public TM_Customization()
        {
            tmConfig = TMConfig.Current;
            googleAnalytics =  GoogleAnalytics.Current;
        }

        public void CustomizeTMConfig()
        {	
            //TM Security
            tmConfig.TMSecurity.SSL_RedirectHttpToHttps		    = false;    // true
            tmConfig.TMSecurity.Show_ContentToAnonymousUsers	= false;
            tmConfig.TMSecurity.EvalAccounts_Enabled			= true;
            tmConfig.TMSecurity.EvalAccounts_Days				= 15;

            //General TM Configuration
            tmConfig.TMSetup.EnableGZipForWebServices           = false;    // true
            tmConfig.TMSetup.Enable302Redirects                 = true;


            //Google Analytics
            googleAnalytics.AccountID				= "UA-37594728-3";			
            googleAnalytics.Enabled					= true;
            googleAnalytics.LogWebServicesCalls		= true;

            
            

            //OnInstallation
            tmConfig.OnInstallation.ForceAdminPasswordReset          = true;
            tmConfig.OnInstallation.DefaultLibraryToInstall_Name     = "Top Vulnerabilities";
            tmConfig.OnInstallation.DefaultLibraryToInstall_Location = "https://github.com/TeamMentor/Library_Top_Vulnerabilities/zipball/master";

            //use reflection to invoke the extra customizations from the TM_Customization_Local.cs that should only exist locally
            var extraLocalCustomizations = this.type().Assembly.type("TM_Customization_Local");
            if (extraLocalCustomizations.notNull())	        
                extraLocalCustomizations.ctor().invoke("CustomizeTMConfig");
            /*{
                var obj = extraLocalCustomizations.ctor();
                var method = obj.type().method("CustomizeTMConfig");
                method.invoke();
                extraLocalCustomizations.invoke("CustomizeTMConfig");
            }*/

        }
    }
}
