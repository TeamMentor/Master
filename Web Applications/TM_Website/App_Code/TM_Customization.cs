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

			TMEvents.OnApplication_BeginRequest.Add(
				()=>{
						if (HttpContextFactory.Request.Url.str().contains(".js", ".ashx",".asmx", ".jpg", ".gif").isFalse())
							HttpContextFactory.Response.Write("<h1>Custom code</h1>");
					});
	    }

	    public void CustomizeTMConfig()
	    {
		    var tmConfig = TMConfig.Current;
			tmConfig.ShowContentToAnonymousUsers = true;
	    }
    }
}