using System;
using System.Collections.Generic;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;
using O2.Kernel.InterfacesBaseImpl;

namespace TeamMentor.CoreLib
{
    public class TMEvents
    {
        public static List<Action> OnSession_Start			    { get; set; }
        public static List<Action> OnSession_End			    { get; set; }
        public static List<Action> OnApplication_Error			{ get; set; }
        public static List<Action> OnApplication_Start			{ get; set; }
        public static List<Action> OnApplication_End			{ get; set; }
        public static List<Action> OnApplication_BeginRequest	{ get; set; }
        
        static TMEvents()
        {
            OnSession_Start			    = new List<Action>();
            OnSession_End			    = new List<Action>();
            OnApplication_Error			= new List<Action>();
            OnApplication_Start			= new List<Action>();
            OnApplication_End			= new List<Action>();
            OnApplication_BeginRequest	= new List<Action>();			
        }
    }

    public class Global : HttpApplication
    {
        static Global()
        {
            new TM_StartUp().SetupEvents();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            TMEvents.OnApplication_Error.invoke();         
        }
         
        protected void Application_Start				(object sender, EventArgs e)
        {
            TMEvents.OnApplication_Start.invoke();
            
        }
        
        protected void Application_BeginRequest			(object sender, EventArgs e)		
        {
            TMEvents.OnApplication_BeginRequest.invoke();            
        }

        protected void Session_Start					(object sender, EventArgs e)        
        {
            TMEvents.OnSession_Start.invoke();
        }	        
        protected void Session_End						(object sender, EventArgs e)		
        {
            TMEvents.OnSession_End.invoke();
        }
        protected void Application_End					(object sender, EventArgs e)		
        {
            TMEvents.OnApplication_End.invoke();
        }
        protected void Application_AcquireRequestState  (object sender, EventArgs e)		{ }        
        protected void Application_AuthenticateRequest	(object sender, EventArgs e)		{ }
    }
}