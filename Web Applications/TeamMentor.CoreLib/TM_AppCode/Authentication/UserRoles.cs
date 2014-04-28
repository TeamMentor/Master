
using System.Collections.Generic;

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
        public static Dictionary<UserGroup, List<UserRole>> Mappings { get; set; }

        static UserRolesMappings()
        {
            Mappings = new Dictionary<UserGroup, List<UserRole>>
                {
                    {UserGroup.None, 
                        new List<UserRole>()},
                    {UserGroup.Anonymous, 
                        new List<UserRole>
                            {
                                UserRole.ReadArticlesTitles
                            }},
                    {UserGroup.Admin,
                        new List<UserRole>
                            {
                                UserRole.ReadArticlesTitles,
                                UserRole.ReadArticles,
                                UserRole.EditArticles,
                                UserRole.ManageUsers,
                                UserRole.Admin,
                            }
                    },
                    {UserGroup.Editor,
                        new List<UserRole>
                            {
                                UserRole.ReadArticlesTitles, 
                                UserRole.ReadArticles, 
                                UserRole.EditArticles
                            }},
                    {UserGroup.Reader, 
                        new List<UserRole>
                            {
                                UserRole.ReadArticlesTitles, 
                                UserRole.ReadArticles
                            }},
                    {UserGroup.Developer, 
                        new List<UserRole>()},
                };
        }
    }	
}
