using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{        
    public class TestFixture_TBot
    {    
        public TM_QA_Config     TmQAConfig      { get; set; }
    	public Uri 				WebSite_Url 	{ get; set; }    	
                        
        public IE_UnitTest      tbot;
        public WatiN_IE         ie;
                
        [TestFixtureSetUp] public void testFixtureSetUp()
        {                        
            TmQAConfig = new TM_QA_Config_Loader().load();            
            if (TmQAConfig.serverOffline())
                Assert.Fail("[TestFixture_WebServices]TM server is offline: {0}".info(WebSite_Url));
            
            tbot = new IE_UnitTest
                {
                    TargetServer = TmQAConfig.Url_Target_TM_Site
                };            
            ie = tbot.ie;
        }        
        [TestFixtureTearDown] public void testFixtureTearDown()
        {   
            //ie.close();         
            ie.parentForm().close();
            IE_UnitTest.Current = null;
        }
    }
}
