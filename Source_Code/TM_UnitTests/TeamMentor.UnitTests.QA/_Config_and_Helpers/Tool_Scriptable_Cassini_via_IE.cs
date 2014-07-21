using FluentSharp.NUnit;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA
{
    public class Program
    {
        public static int Main(string[] args)
        {
            new Tool_Scriptable_Cassini_via_IE().Show_REPL_For_Cassini_And_IE__Runnnig_TeamMentor();
            return 0;
        }
    }
    [TestFixture]
    public class Tool_Scriptable_Cassini_via_IE  
    {
        [Test][Ignore("Manual Invocation")]  
        public void Show_REPL_For_Cassini_And_IE__Runnnig_TeamMentor()
        {
            var nUnitTests_Cassini = new NUnitTests_Cassini_TeamMentor().start();
            var tmProxy            = nUnitTests_Cassini.tmProxy();
            var ieTeamMentor       = nUnitTests_Cassini.new_IE_TeamMentor();
            var ie                 = ieTeamMentor.ie;
            var script             = ie.parentForm().insert_Below().add_Script()
                                                    .insert_LogViewer();
            
            script.add_InvocationParameter("ie"                , ieTeamMentor.ie)
                  .add_InvocationParameter("tmProxy"           , tmProxy)            
                  .add_InvocationParameter("ieTeamMentor"      , ieTeamMentor)
                  .add_InvocationParameter("nUnitTests_Cassini", nUnitTests_Cassini);            

            var code = @"//This script has the following objects provided as parameters


ieTeamMentor.page_Home();

return tmProxy;
return ie;
return tmProxy;
return ieTeamMentor;
return nUnitTests_Cassini;

//using FluentSharp.Watin;
//using FluentSharp.CassiniDev
//using TeamMentor.CoreLib
//using TeamMentor.UnitTests.Cassini

//O2Ref:TeamMentor.UnitTests.Cassini.dll
//O2Ref:TeamMentor.Schemas.dll
//O2Ref:TeamMentor.Users.dll
//O2Ref:TeamMentor.Database.dll
//O2Ref:FluentSharp.WatiN.dll
//O2Ref:FluentSharp.NUnit.dll
//O2Ref:FluentSharp.CassiniDev.dll
//O2Ref:Watin.Core.dll
";
            script.onCompileExecuteOnce()
                  .set_Code(code);

            nUnitTests_Cassini.siteUri.GET().assert_Contains("TeamMentor");     // make a request to trigger Asp.NET pipeline and TM Startup
            tmProxy.map_ReferencesToTmObjects();                                // these should be alive after startup

            ie.parentForm()
              .set_H2Icon()
              .waitForClose();
        }
    }
}
