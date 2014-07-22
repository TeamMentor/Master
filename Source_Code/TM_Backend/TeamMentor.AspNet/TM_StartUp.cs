using System;
using System.Security;
using System.Web;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using TeamMentor.FileStorage;
using TeamMentor.UserData;


namespace TeamMentor.CoreLib
{
    [Serializable]
    public class TM_StartUp : MarshalByRefObject
    {
        public static TM_StartUp        Current                 { get; set; }
        public TM_Engine                TMEngine                { get; set; }        
        public Tracking_Application     TrackingApplication     { get; set; }
        public TM_FileStorage           TmFileStorage           { get; set; }        
        public string                   Version                 { get; set; }        
                
        public TM_StartUp()
        {
            Current  = this;
            TMEngine = new TM_Engine();
            Version  = this.type().assembly().version(); 
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
        
        [Assert_Admin]                                              
        public void Application_Start()
        {            
            UserGroup.Admin.assert();                                   // impersonate an admin to load the database

            "[TM_StartUp] Application Start".info();   

            TmFileStorage       = new TM_FileStorage();                 // this will trigger the load of all TM_Xml_Database data
            

            TmFileStorage.UserData.createDefaultAdminUser();            // ensures that there is an valid admin
            
            TrackingApplication   = new Tracking_Application(TmFileStorage.path_XmlDatabase());    // Enabled Application Tracking

            TM_REST.SetRouteTable();	                                // Set REST routes            
            MVC5.MapDefaultRoutes();                                    // Map MVC 5 routes

            TrackingApplication.saveLog();                              // save log                         

            UserGroup.None.assert();                                    // revert admin user impersonation
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
                 HttpContextFactory.Server.Transfer(TMConsts.DEFAULT_ERROR_PAGE_REDIRECT);            
     //          HttpContextFactory.Response.Redirect(TMConsts.DEFAULT_ERROR_PAGE_REDIRECT);            
        }           
        public void Application_BeginRequest()
        {   
            if(SendEmails.TM_Server_URL.isNull())                        // (for this version of SendEMails)
                SendEmails.mapTMServerUrl();                             // This configuration needs to be done using a live HttpContext object   

            TMEngine.performHealthCheck()
                    .logRequest()
                    .handleRequest();
            
            
        }
    }
}
