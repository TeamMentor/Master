using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_Http
{
    [TestFixture]
    public class Test_QA__HandleUrlRequest : NUnitTests_Cassini_TeamMentor
    {
        [SetUp] public void setup()
        {
            tmProxy.assert_Null();
            this.tmProxy_Refresh();
            tmProxy.assert_Not_Null();
            tmProxy.TmFileStorage.assert_Not_Null();
        }
        /// <summary>
        /// Checks that we can get the control the Content-type via an extra 'json' parameter
        /// </summary>
        [Test] public void whoami()
        {
            var whoami_Html_Url = siteUri.append("whoami");
            var whoami_Json_Url = siteUri.append("whoami/json");

            var webClient = new WebClient();
            webClient.DownloadString(whoami_Html_Url).assert_Contains("GroupId");
            var contentType = webClient.ResponseHeaders["Content-type"];
            contentType.assert_Is("text/html; charset=utf-8");
            
            webClient.DownloadString(whoami_Json_Url).assert_Contains("GroupId");
            contentType = webClient.ResponseHeaders["Content-type"];
            contentType.assert_Is("application/json; charset=utf-8");
        }
    }
}
