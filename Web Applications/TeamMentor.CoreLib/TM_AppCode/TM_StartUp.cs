using System.Web;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public class TM_StartUp
    {
        public void SetupEvents()
        {
            TMEvents.OnSession_Start            .add(Session_Start);
            TMEvents.OnSession_End              .add(Session_End);
            TMEvents.OnApplication_Start        .add(Application_Start);
            TMEvents.OnApplication_End          .add(Application_End);
            TMEvents.OnApplication_Error        .add(Application_Error);
            TMEvents.OnApplication_BeginRequest.add(Application_BeginRequest);
        }
        public void Session_Start()
        {
        }
        public void Session_End()
        {
        }
        
        [Assert_Admin]                      // impersonate an admin to load the database
        public void Application_Start()     
        {            
            // ReSharper disable ObjectCreationAsStatement
            if (HttpContextFactory.Context.Server.MachineName == "WIN-FGNQ5AARJ8O")
                "".popupWindow().add_LogViewer();            
            new TM_Xml_Database(true);                                  // Create FileSystem Based database            
            TM_REST.SetRouteTable();			                        // Set REST routes
            // ReSharper restore ObjectCreationAsStatement
        } 
        public void Application_End()
        {
        }
        public void Application_Error()
        {            
            var lastError = HttpContextFactory.Server.GetLastError();
            if (lastError is HttpException && (lastError as HttpException).GetHttpCode() == 404)
            {				
                new HandleUrlRequest().routeRequestUrl_for404();
            }
            "LastError: {0}".error(lastError);
        }   
        
        public void Application_BeginRequest()
        {
            ResponseHeaders.addDefaultResponseHeaders();
            new HandleUrlRequest().routeRequestUrl();                                  
        }
    }
}
