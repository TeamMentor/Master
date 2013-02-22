using TeamMentor.CoreLib;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.Website
{
    public class TM_Customization
    {
        public TMConfig			tmConfig;        

        public static void AppInitialize()
        {		 
            new TM_Customization().CustomizeTMConfig();			
        }

        public TM_Customization()
        {
            tmConfig = TMConfig.Current;            
        }

        public void CustomizeTMConfig()
        {	            
            //Google Analytics
            var googleAnalytics =  GoogleAnalytics.Current;
            googleAnalytics.AccountID				= "UA-37594728-3";			
            googleAnalytics.Enabled					= true;
            googleAnalytics.LogWebServicesCalls		= true;
                        
            //use reflection to invoke the extra customizations from the TM_Customization_Local.cs that should only exist locally
            var extraLocalCustomizations = this.type().Assembly.type("TM_Customization_Local");
            if (extraLocalCustomizations.notNull())	        
                extraLocalCustomizations.ctor().invoke("CustomizeTMConfig");

        }
    }
}
