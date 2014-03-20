using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public class TM_Engine
    {
        public TM_Engine performHealthCheck()
        {
            if(TMConfig.Current.isNull())   // this is a catastrofic even and TM cannot recover from it
            {
                "[Fatal Error] TMConfig.Current was null".error();
                HttpContextFactory.Server.Transfer(TMConsts.PATH_HTML_PAGE_UNAVAILABLE);
            }
            return this;
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
