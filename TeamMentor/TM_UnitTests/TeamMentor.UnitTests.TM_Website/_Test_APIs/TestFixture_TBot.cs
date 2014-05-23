using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{        
    public class TestFixture_TBot
    {    
        public TM_QA_Config     TmQAConfig      { get; set; }
    	public Uri 				WebSite_Url 	{ get; set; }    	
                        
        public API_IE_TBot      tbot;
        public WatiN_IE         ie;
                
        public TestFixture_TBot()
        {            
            Assert.Ignore("Disable IE tests");
            TmQAConfig = new TM_QA_Config_Loader().load();
            if (TmQAConfig.serverOffline())
                Assert.Ignore("[TestFixture_WebServices]TM server is offline: {0}".info(WebSite_Url));
            
            tbot = new API_IE_TBot
                {
                    TargetServer = TmQAConfig.Url_Target_TM_Site
                };
            ie = tbot.ie;
        }
    }
}
