using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;
using Encoder = Microsoft.Security.Application.Encoder;

namespace TeamMentor.CoreLib
{
	public static class TMUser_ExtensionMethods
	{				
    	// it is better to do this since we can return null tmUsers.Where((tmUser)=> tmUser.UserName == name).First()
    	public static TMUser user(this List<TMUser> tmUsers, string name)
    	{
    	    return tmUsers.FirstOrDefault(tmUser => tmUser.UserName == name);
    	}

	    public static TMUser user(this List<TMUser> tmUsers, int id)
	    {
	        return tmUsers.FirstOrDefault(tmUser => tmUser.UserID == id);
	    }

	    public static bool updateUser(this List<TMUser> tmUsers, int userId, string userName, string firstname, string lastname, string title, string company, string email, int groupId)
    	{
    		var tmUser = tmUsers.user(userId);
    		if (tmUser.isNull())
    			return false;
    		if (groupId == -1)
    			groupId = tmUser.GroupID;
    		if (tmUser.notNull())
    		{
    			tmUser.EMail = 		Encoder.XmlEncode(email);
				tmUser.UserName = 	Encoder.XmlEncode(userName);
    			tmUser.FirstName = 	Encoder.XmlEncode(firstname);
    			tmUser.LastName = 	Encoder.XmlEncode(lastname);
				tmUser.Title = 		Encoder.XmlEncode(title);
				tmUser.Company = 	Encoder.XmlEncode(company);
				tmUser.GroupID = 	groupId;
    		    tmUser.saveTmUser();
    			return true;
    		}
    		return false;
    	}
					
		public static string createPasswordHash(this TMUser tmUser, string password)
		{
			var stringToHash = tmUser.ID + tmUser.UserName + password;   // The password hash is made of a GUID, the Unique username, and a password
			var sha256 = System.Security.Cryptography.SHA256.Create();
			var hashBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(stringToHash));
			var hashString = new StringBuilder();
			foreach (byte b in hashBytes)
				hashString.Append(b.ToString("x2"));
			return hashString.ToString();
		}
		public static List<string> toStringList(this List<Guid> guids)
		{
			return (from guid in guids
					select guid.str()).toList();
		}		
	}	
}