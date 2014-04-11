using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using FluentSharp.CoreLib;
using PostSharp.Aspects;

namespace TeamMentor.CoreLib
{
    [Serializable]
    public sealed class LogUrlAttribute : OnMethodBoundaryAspect
    {
        public string Category { get; set; }

        public LogUrlAttribute(string category)
        {
            Category = category;
        }
        
        public override void OnEntry(MethodExecutionArgs args)
        {										
            var url = HttpContextFactory.Request.Url;
            var title = "[TM_Log] : {0}".format(Category);
            title.ga_LogEntry(url.str());			

            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class LogAttribute : OnMethodBoundaryAspect
    {
        public string Category { get; set; }

        public LogAttribute(string category)
        {
            Category = category;
        }

        public override void OnEntry(MethodExecutionArgs args)
        { 			
            "[TM_Log]".ga_LogEntry(Category);			

            base.OnEntry(args);
        }
    }


    public static class PostSharp_ExtensionMethods
    {
        public static bool has_Arguments(this MethodInterceptionArgs args)
        {
            return args.Arguments.empty().isFalse();
        }

        public static object first_Argument(this MethodInterceptionArgs args)
        {
            //return args.Arguments.first();  //returns null;
            if (args.has_Arguments())
                return args.Arguments[0];
            return null;
        }

        public static T argument<T>(this MethodInterceptionArgs args)
        {
            foreach(var argument in args.Arguments)
            if (argument is T)
                return (T) argument;
            return default(T);
        }
    }

    [Serializable]
    public sealed class TM_AdminAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {
            try
            {
                UserRole.Admin.demand();
                base.OnEntry(args);
            }
            catch (Exception)
            {
                if (HttpContextFactory.Request.Url != null)
                {
                    var absolutePath = HttpContextFactory.Request.Url.AbsolutePath;

                    HttpContextFactory.Response.Redirect("/Login?LoginReferer=" + absolutePath);
                }
            }							

            
        }
    }
    
    [Serializable]
    public sealed class AdminAttribute : OnMethodBoundaryAspect
    {
        public static bool GlobalDisableFor_AdminAttribute { get; set; }

        public override void OnEntry(MethodExecutionArgs args)
        {            
           // try
           // {
              //  "[AdminAttribute] Thread id: {0}".error(Thread.CurrentThread.ManagedThreadId);
                var userRoles = Thread.CurrentPrincipal.roles().toList().join(",");
                if (HttpContextFactory.Session.notNull())
                {
                    if (userRoles.empty() && HttpContextFactory.Session["principal"] is IPrincipal)
                    {
                        var sessionPrincipal = HttpContextFactory.Session["principal"] as IPrincipal;
                        "[AdminAttribute] changing thread principal from {0} to {1} (last from session variable)".debug(Thread.CurrentPrincipal, sessionPrincipal);
                        Thread.CurrentPrincipal = sessionPrincipal;
                    }                    
                }
                
                UserRole.Admin.demand();
         /*   }
            catch (Exception)
            {
                
                if (GlobalDisableFor_AdminAttribute)
                    "[GlobalDisableFor_AdminAttribute] is set".error();
                 else
                    UserRole.Admin.demand();  // rigger the original exception
            }            
          */ 
            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class EditArticlesAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {		    
            UserRole.EditArticles.demand();
            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class EditGuiAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {		    
            UserRole.EditGui.demand();
            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class ManageUsersAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {		    
            UserRole.ManageUsers.demand();
            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class ReadArticlesAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {		    
            UserRole.ReadArticles.demand();
            base.OnEntry(args);
        }
    }
    [Serializable]
    public sealed class ReadArticlesTitlesAttribute : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {		    
            UserRole.ReadArticlesTitles.demand();
            base.OnEntry(args);
        }
    }

    //asserts  : there is a possible race condition here if the user is able to make more requests into the same thread
    [Serializable]
    public sealed class Assert_Reader : OnMethodBoundaryAspect
    {				
        public object lockThread;
        public override void OnEntry(MethodExecutionArgs args)
        {
            UserGroup.Reader.setPrivileges();            
            base.OnEntry(args);            
        }
        public override void  OnExit(MethodExecutionArgs args)
        {
            UserGroup.None.setPrivileges();
 	         base.OnExit(args);
        }        
    }
    [Serializable]
    public sealed class Assert_Editor : OnMethodBoundaryAspect
    {				
        public object lockThread;
        public override void OnEntry(MethodExecutionArgs args)
        {
            UserGroup.Editor.setPrivileges();            
            base.OnEntry(args);            
        }
        public override void  OnExit(MethodExecutionArgs args)
        {
            UserGroup.None.setPrivileges();
 	         base.OnExit(args);
        }        
    }
    [Serializable]
    public sealed class Assert_Admin : OnMethodBoundaryAspect
    {				
        public override void OnEntry(MethodExecutionArgs args)
        {            
            UserGroup.Admin.setPrivileges();
            base.OnEntry(args);
        }
        public override void  OnExit(MethodExecutionArgs args)
        {
            UserGroup.None.setPrivileges();
 	        base.OnExit(args);
        }        
    }    
}
