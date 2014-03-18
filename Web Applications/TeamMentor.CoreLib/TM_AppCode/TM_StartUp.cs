using System;
using System.Security;
using System.Web;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_StartUp
    {
        public static TM_StartUp        Current               { get; set; }
        public Tracking_Application     TrackingApplication   { get; set; }
        public TM_Xml_Database          TmXmlDatabase         { get; set; }

        public TM_StartUp()
        {
            Current = this;
        }

        public void SetupEvents()
        {
            TMEvents.OnSession_Start            .add(Session_Start);
            TMEvents.OnSession_End              .add(Session_End);
            TMEvents.OnApplication_Start        .add(Application_Start);
            TMEvents.OnApplication_End          .add(Application_End);
            TMEvents.OnApplication_Error        .add(Application_Error);
            TMEvents.OnApplication_BeginRequest .add(Application_BeginRequest);
        }
        public void Session_Start()
        {
        }
        public void Session_End()
        {            
            TrackingApplication.saveLog();
        }
        
        [Assert_Admin]                      // impersonate an admin to load the database
        public void Application_Start()
        {
            //O2_Utils.showLogViewer_if_LocalHost();                
            Logger_Firebase.createAndHook();
            TmXmlDatabase           = new  TM_Xml_Database(true);                                   // Create FileSystem Based database            
            TrackingApplication     = new Tracking_Application(TmXmlDatabase.Path_XmlDatabase);    // Enabled Application Tracking
            TM_REST.SetRouteTable();	// Set REST routes            
            TrackingApplication.saveLog();
        } 
        public void Application_End()
        {
            TrackingApplication.stop();
        }
        public void Application_Error()
        {              
            var lastError = HttpContextFactory.Server.GetLastError();
            if (lastError is HttpException && (lastError as HttpException).GetHttpCode() == 404)
            {				
                new HandleUrlRequest().routeRequestUrl_for404();
            }
            if (lastError is SecurityException)
            {
               // HttpContextFactory.Response.Redirect("~/Error/Permission.aspx");
            }
                
            "[TM][Application_Error]: {0}".error(lastError);
            TrackingApplication.saveLog();
            //disabling error redirection while in dev
         //   HttpContextFactory.Response.Redirect("/error");
        }           
        public void Application_BeginRequest()
        {            
            Requests_Firebase.Current.logRequest();
            ResponseHeaders.addDefaultResponseHeaders();
            new HandleUrlRequest().routeRequestUrl();                                  
        }
    }
}
