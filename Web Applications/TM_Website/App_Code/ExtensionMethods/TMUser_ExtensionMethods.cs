using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
//using System.Text;
using Microsoft.Security.Application;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:../UtilMethods.cs
//O2File:../IJavascriptProxy.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
	public static class TMUser_ExtensionMethods
	{				
    	// it is better to do this since we can return null tmUsers.Where((tmUser)=> tmUser.UserName == name).First()
    	public static TMUser user(this List<TMUser> tmUsers, string name)
    	{
    		foreach(var tmUser in tmUsers)
    			if (tmUser.UserName == name)
    				return tmUser;
    		return null;
    	}		
		
		public static TMUser user(this List<TMUser> tmUsers, int id)
    	{
    		foreach(var tmUser in tmUsers)
    			if (tmUser.UserID == id)
    				return tmUser;
    		return null;
    	}

    	public static bool delete(this List<TMUser> tmUsers, int id)
    	{    		
    		foreach(var tmUser in tmUsers)
    			if (tmUser.UserID == id)
    			{
    				tmUsers.remove(tmUser);
    				return true;
    			}
    		return false;
    	}

    	public static bool updateUser(this List<TMUser> tmUsers, int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId)
    	{
    		var tmUser = tmUsers.user(userId);
    		if (tmUser.notNull())
    		{
    			tmUser.EMail = 		Encoder.XmlEncode(email);
				tmUser.UserName = 	Encoder.XmlEncode(userName);
    			tmUser.FirstName = 	Encoder.XmlEncode(firstname);
    			tmUser.LastName = 	Encoder.XmlEncode(lastname);
				tmUser.Title = 		Encoder.XmlEncode(title);
				tmUser.Company = 	Encoder.XmlEncode(company);
				tmUser.GroupID = 	groupId;
    			return true;
    		}
    		return false;
    	}	
	}	
}