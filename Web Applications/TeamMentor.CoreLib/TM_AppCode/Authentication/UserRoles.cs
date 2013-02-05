using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;

namespace TeamMentor.CoreLib
{
    public enum UserGroup
    {
        None = -1,
        Anonymous = 0,
        Admin = 1,
        Reader = 2, 
        Editor = 3,		
        Developer = 4               
    }

    public enum UserRole
    { 
        Admin, 		
        ManageUsers,
        EditArticles,
        ReadArticles,
        ReadArticlesTitles,
        EditGui
    }

    public class UserRolesMappings
    {
        public static Dictionary<UserGroup, List<UserRole>> Mappings;

        static UserRolesMappings()
        {
            Mappings = new Dictionary<UserGroup, List<UserRole>>();
            Mappings.Add(UserGroup.None  		, new List<UserRole>() { });
            Mappings.Add(UserGroup.Anonymous    , new List<UserRole>() { UserRole.ReadArticlesTitles });
            Mappings.Add(UserGroup.Admin 		, new List<UserRole>() { UserRole.ReadArticlesTitles , UserRole.ReadArticles, UserRole.EditArticles, UserRole.ManageUsers , UserRole.Admin, });
            Mappings.Add(UserGroup.Editor		, new List<UserRole>() { UserRole.ReadArticlesTitles , UserRole.ReadArticles, UserRole.EditArticles  });
            Mappings.Add(UserGroup.Reader		, new List<UserRole>() { UserRole.ReadArticlesTitles , UserRole.ReadArticles });            
        }
    }	
}
