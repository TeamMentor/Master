using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient;
using TeamMentor.CoreLib;
using TeamMentor.CoreLib.WebServices;

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
			tmConfig.ShowContentToAnonymousUsers = false;
	    }
    }
}