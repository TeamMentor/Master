using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.Git;
using FluentSharp.NUnit;
using FluentSharp.Web35;
using NUnit.Framework;
using TeamMentor.FileStorage;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA_Http
{
    [TestFixture]
    public class Test_QA__Git_Support
    {
        [Test]
        public void Check_That_New_UserData_Is_Git_REPO()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor();
            nUnitTests_Cassini.start(true);

            nUnitTests_Cassini.siteUri.GET().assert_Contains("TeamMentor");

            var tmProxy = nUnitTests_Cassini.tmProxy_Refresh()
                                            .tmProxy;

            tmProxy.TmServer.assert_Not_Null()
                            .Git.UserData_Git_Enabled.assert_True();
            
            tmProxy.TmFileStorage.path_UserData()
                                 .isGitRepository().assert_Is_True();

            ////tmProxy.TmFileStorage.tmServer_Location().assert_Is("ad");
        }
    }
}
