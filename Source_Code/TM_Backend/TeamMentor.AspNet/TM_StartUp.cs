using System;
using System.Security;
using System.Web;
using FluentSharp.CoreLib;
using TeamMentor.FileStorage;


namespace TeamMentor.CoreLib
{
    public class TM_StartUp
    {
        public static TM_StartUp        Current                 { get; set; }
        public static TM_Engine         TMEngine                { get; set; }        
        public Tracking_Application     TrackingApplication     { get; set; }
        public TM_FileStorage           TmFileStorage           { get; set; }
        public TM_Xml_Database          TmXmlDatabase           { get; set; }

        public TM_StartUp()
        {
            Current  = this;
            TMEngine = new TM_Engine();
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
            "[TM_StartUp] Session Start".info();
        }
        public void Session_End()
        {   
            "[TM_StartUp] Session End".info();
            TrackingApplication.saveLog();
        }
        
        [Assert_Admin]                      // impersonate an admin to load the database
        public void Application_Start()
        {
            UserGroup.Admin.assert();
            "[TM_StartUp] Application Start".info();   

            TmFileStorage       = new TM_FileStorage();                 //this will trigger the load of all TM_Xml_Database data
            TmXmlDatabase       = TmFileStorage.TMXmlDatabase;

            //var tmServer            = new TM_Server().setDefaultData();
            //TmXmlDatabase           = new  TM_Xml_Database().setup();                                   // Create FileSystem Based database            
            TrackingApplication     = new Tracking_Application(TmXmlDatabase.path_XmlDatabase());    // Enabled Application Tracking

            TM_REST.SetRouteTable();	// Set REST routes            // TODO add Application_Start event

            TrackingApplication.saveLog();
             
            UserGroup.None.assert();
        } 
        public void Application_End()
        {
            "[TM_StartUp] Application End".info();            
            TrackingApplication.stop();
        }
        public void Application_Error()
        {              
            var lastError = HttpContextFactory.Server.GetLastError();
            if (lastError is HttpException && ((HttpException)lastError).GetHttpCode()== 404)
            {				                
                new HandleUrlRequest().routeRequestUrl_for404();
                // if we got this far it means that the request was not handled by one of TM's mappings
                "[TM] [Application_Error]: 404 Error on {0}".error(HttpContextFactory.Request.Url.str());    
                TM_Xml_Database.Current.logTBotActivity("404", HttpContextFactory.Request.Url.str());
            }
            else
            {
                "[TM] [Application_Error]: {0}".error(lastError);                        
                TM_Xml_Database.Current.logTBotActivity("Application Error", "{0} : {1}".format(lastError.Message, HttpContextFactory.Request.Url.str()));
            }

            if (lastError is SecurityException)
            {
                TM_Xml_Database.Current.logTBotActivity("Security Exception", HttpContextFactory.Request.Url.str());
               // HttpContextFactory.Response.Redirect("~/Error/Permission.aspx");
            }
            else
                
            "[TM][Application_Error]: {0}".error(lastError);
            TrackingApplication.saveLog();
            if (TMConfig.Current.TMSetup.ShowDotNetDebugErrors.isFalse())
                 HttpContextFactory.Response.Redirect(TMConsts.DEFAULT_ERROR_PAGE_REDIRECT);            
        }           
        public void Application_BeginRequest()
        {            
            TMEngine.performHealthCheck()
                    .logRequest()
                    .handleRequest();
            
            
        }
    }
}
