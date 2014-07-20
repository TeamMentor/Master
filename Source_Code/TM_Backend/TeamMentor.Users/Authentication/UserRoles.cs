
using System.Collections.Generic;

namespace TeamMentor.CoreLib
{
    /// <summary>
    /// TeamMentor User Groups
    /// 
    /// See UserRolesMappings for the mappings of these User Groups into multiple User Roles
    ///    
    /// Ideally these should be ordered by order of privildge (like the User Role, but there is some UI dependencies on these numbers (which are now legacy))
    /// </summary>
    public enum UserGroup       
    {        
        /// <summary>
        /// Main admin of the app, can also manage users
        /// </summary>
        Admin  = 1,
        /// <summary>
        /// Can edit articles (which since they allow HTML, editors can become admins via social enginerring)
        /// </summary>          
        Editor = 3,
        /// <summary>
        /// Can read the content and metadata of articles
        /// </summary>
        Reader = 2,
        /// <summary>
        ///  Can view article's titles and library stucture. Dending on <code>TM_Config</code> mappings, non-logged in users can be given this Group
        /// </summary>
        Viewer = 0,
        /// <summary>
        /// Cannot view any content managed by TeamMentor. Should only be able to Login
        /// </summary>
        None   = -1 
    }

    /// <summary>
    /// TeamMentor User Roles
    /// 
    /// See UserRolesMappings for the mappings of multiple User Roles into User Groups
    /// </summary>
    public enum UserRole
    {         
        Admin, 		
        ManageUsers,
        EditArticles,
        ReadArticles,
        ReadArticlesTitles,
        ViewLibrary,
        None        
    }

    public class UserRolesMappings
    {
        public static Dictionary<UserGroup, List<UserRole>> Mappings { get; set; }

        static UserRolesMappings()
        {
            Mappings = new Dictionary<UserGroup, List<UserRole>>
                {
                    {UserGroup.None, 
                        new List<UserRole>()
                            {
                                UserRole.None                                
                            }},
                    {UserGroup.Viewer, 
                        new List<UserRole>
                            {                                
                                UserRole.ReadArticlesTitles,
                                UserRole.ViewLibrary
                            }},
                    {UserGroup.Admin,
                        new List<UserRole>
                            {                                
                                UserRole.Admin,
                                UserRole.ManageUsers,
                                UserRole.EditArticles,                                
                                UserRole.ReadArticles,
                                UserRole.ReadArticlesTitles,
                                UserRole.ViewLibrary
                            }
                    },
                    {UserGroup.Editor,
                        new List<UserRole>
                            {                                
                                UserRole.EditArticles,                          
                                UserRole.ReadArticles, 
                                UserRole.ReadArticlesTitles,
                                UserRole.ViewLibrary
                            }},
                    {UserGroup.Reader, 
                        new List<UserRole>
                            {
                                UserRole.ReadArticles,
                                UserRole.ReadArticlesTitles, 
                                UserRole.ViewLibrary
                            }},                    
                };
        }
    }	
}
