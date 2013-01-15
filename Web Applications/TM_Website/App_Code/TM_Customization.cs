using TeamMentor.CoreLib;

namespace TeamMentor.Website
{
    public class TM_Customizations
    {
	    public static void AppInitialize()
	    {		 
		    new TM_Customizations().CustomizeTMConfig();			
	    }

	    public void CustomizeTMConfig()
	    {
		    var tmConfig = TMConfig.Current;
			tmConfig.ShowContentToAnonymousUsers = true;
	    }
    }
}