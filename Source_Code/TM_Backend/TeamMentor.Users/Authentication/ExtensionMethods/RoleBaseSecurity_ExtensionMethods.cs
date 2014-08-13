using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    //Note: I'm not sure how to create a generic version of these classes, while keeping the current 
    //      easy to use calling convension. There are two probs to solve: 1) UserRole is an Enum , 2) need to use static methods for the assert and demand methods
    //
    //     best I could do is to format the code so that it is easy to read
    public class admin 
    {
        const UserRole Role = UserRole.Admin;
        public static void      assert()                                { Role.assert();                          }
        public static Action    assert(Action executeWithAssert)        { return Role.assert(executeWithAssert);  }
        public static T         assert<T>(Func<T> executeWithAssert)    { return Role.assert(executeWithAssert);  }
        public static void      demand()                                { Role.demand();                          }        
    }

    public class manageUsers
    {
        const UserRole Role = UserRole.ManageUsers;
        public static void      assert()                                { Role.assert();                          }
        public static Action    assert(Action executeWithAssert)        { return Role.assert(executeWithAssert);  }
        public static T         assert<T>(Func<T> executeWithAssert)    { return Role.assert(executeWithAssert);  }
        public static void      demand()                                { Role.demand();                          }        
    }

    public class editArticles 
    {
        const UserRole Role = UserRole.EditArticles;       
        public static void      assert()                                { Role.assert();                          }
        public static Action    assert(Action executeWithAssert)        { return Role.assert(executeWithAssert);  }
        public static T         assert<T>(Func<T> executeWithAssert)    { return Role.assert(executeWithAssert);  }

        public static void      demand()                                { Role.demand();                          }        
    }
    public class readArticles 
    {
        const UserRole Role = UserRole.ReadArticles;       
        public static void      demand()                                { Role.demand();                          }        
    }
    public class readArticlesTitles 
    {
        const UserRole Role = UserRole.ReadArticlesTitles;       
        public static void      demand()                                { Role.demand();                          }        
    }    
    public class viewLibrary 
    {
        const UserRole Role = UserRole.ViewLibrary;       
        public static void      demand()                                { Role.demand();                          }        
    }
    public class none 
    {
        const UserRole Role = UserRole.None;
        public static void      assert()                                { Role.assert();                          }
        public static Action    assert(Action executeWithAssert)        { return Role.assert(executeWithAssert);  }
        public static T         assert<T>(Func<T> executeWithAssert)    { return Role.assert(executeWithAssert);  }
        public static void      demand()                                { Role.demand();                          }        
    }

    public static class RoleBaseSecurity_ExtensionMethods
    {
        public static string[] getCurrentUserRoles(this HttpContextBase httpContext)
        {
            return httpContext.User.roles();
        }
        public static IPrincipal setCurrentUserRoles(this HttpContextBase httpContext, UserGroup userGroup)
        {
            return httpContext.setCurrentUserRoles(userGroup.userRoles().ToArray());
        }
		
        public static IPrincipal setCurrentUserRoles(this HttpContextBase httpContext, params UserRole[] userRoles)
        {
            var newPrincipal = userRoles.toStringArray().setThreadPrincipalWithRoles();
			httpContext.User = newPrincipal;
            return newPrincipal;			
        }				
		public static IPrincipal assert(this UserRole userRole)
		{
		    return userRole.setPrivilege();
		}
        public static Action assert(this UserRole userRole, Action executeWithAssert)
		{
            try
            {
                userRole.assert();
                executeWithAssert();
            }
            finally
            {
                UserRole.None.assert();
            }		    
            return executeWithAssert;
		}
        public static T assert<T>(this UserRole userRole, Func<T> executeWithAssert)
		{
            try
            {
                userRole.assert();
                return executeWithAssert();
            }
            finally
            {
                UserRole.None.assert();
            }		                
		}
        public static IPrincipal setPrivilege(this UserRole userRole)
        {
            return userRole.setThreadPrincipalWithRoles();
        }
		public static IPrincipal setThreadPrincipalWithRoles(this UserRole userRole)
		{
            var userRoles = new [] { userRole.str() };
			return  userRoles.setThreadPrincipalWithRoles();
		}
        public static IPrincipal assert(this UserGroup userGroup)
        {
            return userGroup.setThreadPrincipalWithRoles();
        }
        public static IPrincipal setPrivileges(this UserGroup userGroup)
        {
            return userGroup.setThreadPrincipalWithRoles();
        }
		public static IPrincipal setThreadPrincipalWithRoles(this UserGroup userGroup)
		{
			return userGroup.userRoles().ToArray().toStringArray().setThreadPrincipalWithRoles();
		}
		
		public static IPrincipal setThreadPrincipalWithRoles(this string[] userRoles)
		{
			var newIdentity = new GenericIdentity("TM_User"); // note that this needs to be set or the SecurityAction.Demand for roles will not work
            var newPrincipal = new GenericPrincipal(newIdentity, userRoles);						
            Thread.CurrentPrincipal = newPrincipal;
			return newPrincipal;
		}
        public static string[] getThreadPrincipalWithRoles(this HttpContextBase httpContext)
        {
            return Thread.CurrentPrincipal.roles();
        }
		
		public static string[] roles(this IPrincipal principal)
		{            
            if (principal.notNull())    
            {
                var roles = (string[])principal.field("m_roles");
                if (roles.notNull())                        // check for null array
                    if(roles.size() !=1 || roles[0] != "")  // check for case where there is only one rule but it is empty
                        return (string[])principal.field("m_roles");		
            }
            return new string[] {};
		}
		
    }
}
