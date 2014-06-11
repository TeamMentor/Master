using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.TeamMentor.Aspx
{
    [TestFixture]
    public class Test_GZip
    {
        [Test]
        public void setGZipCompression_forAjaxRequests()
        {   
            var context  = HttpContextFactory.Context.mock();
            var request  = context.Request;
            var response = context.Response;
            // test nulls
            Assert.IsFalse(GZip.setGZipCompression_forAjaxRequests(null,null), "Should not throw exception");
            

            TMConfig.Current  = null;
            Assert.IsFalse(GZip.setGZipCompression_forAjaxRequests(request,response), "Should not throw exception");
        }
    }
}
