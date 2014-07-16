using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.Web;

namespace TeamMentor.CoreLib
{
    [Serializable]
    public class TM_Engine : MarshalByRefObject
    {
        public TM_Engine performHealthCheck()
        {
            // these are a catastrofic errors and TM cannot recover from it
            if(TM_Xml_Database.Current.isNull())   // this is a catastrofic even and TM cannot recover from it
            {
                "[Fatal Error] TM_Xml_Database.Current was null".error();
                transferToPageUnavailable();                
            }
            else if(TMConfig.Current.isNull())       
            {
                "[Fatal Error] TMConfig.Current was null".error();
                transferToPageUnavailable(); 
            }                        
            return this;
        }

        public void transferToPageUnavailable()
        {
            HttpContextFactory.Response.ContentType = "text/html";
            HttpContextFactory.Server.Transfer(TMConsts.PATH_HTML_PAGE_UNAVAILABLE);
            // the Server.Transfer call will break the execution (by throwing an exception)
        }

        public TM_Engine logRequest()
        {
            Requests_Firebase.Current.logRequest();
            return this;
        }

        public TM_Engine handleRequest()
        {
            ResponseHeaders.addDefaultResponseHeaders();
            new HandleUrlRequest().routeRequestUrl();  
            return this;                    
        }
    }
}
