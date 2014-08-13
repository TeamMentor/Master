using System;
using System.Collections.Generic;
using System.Web;
using FluentSharp.CoreLib;

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
            foreach (var action in TMEvents.OnApplication_Start) 
            {
                try
                {
                    action.Invoke();    
                }
                catch (Exception ex)
                {                 
                    ex.logWithStackTrace("[in OnApplication_Start]");
                }
            }
            //TMEvents.OnApplication_Start.invoke();            
        }
        
        protected void Application_BeginRequest			(object sender, EventArgs e)		
        {                                  
            foreach (var action in TMEvents.OnApplication_BeginRequest)  // we need to do this loop manually due to the "Thread was being aborted" exception on request.redirects or server.transfers
            {
                try
                {
                    action.Invoke();    
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Thread was being aborted.") // as per the recommendation in http://support.microsoft.com/kb/312629
                        ex.logWithStackTrace("[in Application_BeginRequest]");
                }
            }
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