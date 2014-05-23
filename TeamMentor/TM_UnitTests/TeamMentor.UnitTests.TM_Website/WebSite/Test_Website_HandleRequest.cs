using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website.WebSite
{
    // we don't need the IE for these cases
    [TestFixture]
    public class Test_Website_GET_Urls: TestFixture_WebServices
    {    
        [SetUp] public void setup()
        {
            webServices.logout();
        }
        [Test] public void csharpRepl()      
        {            
            var html = getHtmlForMapping("csharpRepl");                                    
            Assert.IsTrue (html.contains("REPL"));
        }
        [Test] public void login()      
        {            
            var html = getHtmlForMapping("login");                                    
            Assert.IsTrue (html.contains("Login"));
        }        
        [Test] public void teamMentor() 
        {            
            var html = getHtmlForMapping("teamMentor");                        
            Assert.IsTrue (html.contains("TeamMentor"));
            Assert.IsTrue (html.contains("scriptCombiner.axd?s=HomePage_JS"));
            Assert.IsFalse(html.contains("Login"));
        }
        [Test] public void whoAmI()     
        {                                    
            var html = getHtmlForMapping("whoAmI");                                    
            var whoAmI = (Dictionary<string,object>)html.json_Deserialize();
            Assert.NotNull (whoAmI);
            Assert.NotNull (whoAmI["UserName"]);
            Assert.AreEqual(whoAmI["UserName"] ,"");
            Assert.AreEqual(whoAmI["GroupName"],"Anonymous");
            Assert.AreEqual(whoAmI["UserId"]   ,-1);

            var adminUser = this.login_As_Admin();
            Assert.IsNotNull(adminUser);

            html = getHtmlForMapping("whoAmI");                                    
            whoAmI = (Dictionary<string,object>)html.json_Deserialize();            
            Assert.AreNotEqual(whoAmI["UserName" ], "");
            Assert.AreNotEqual(whoAmI["GroupName"], "Anonymous");
            Assert.AreNotEqual(whoAmI["UserId"   ], -1);            
            
            Assert.AreEqual   (whoAmI["UserName" ], adminUser.UserName);
            Assert.AreEqual   (whoAmI["GroupId"  ], adminUser.GroupId);
            Assert.AreEqual   (whoAmI["UserId"   ], adminUser.UserId);
        }
        public string getHtmlForMapping(string mapping)
        {            
            var html = this.http_GET(mapping);            
            Assert.AreNotEqual(html, "", "No Html for mapping: {0}".format(mapping));
            return html;
        }

    }
}
