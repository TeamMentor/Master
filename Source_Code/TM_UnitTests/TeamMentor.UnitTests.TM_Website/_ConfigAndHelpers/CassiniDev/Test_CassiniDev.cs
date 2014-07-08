using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    [TestFixture]
    public class Test_CassiniDev : NUnitTests
    {
        //workflows
        [Test][Ignore] public void Open_Cassini_On_TM_Root_With_REPL()
        {
            var api_Cassini = new API_Cassini();
            //api_Cassini.PhysicalPath.startProcess();
            api_Cassini.start();
            api_Cassini.url().assert_Not_Null()
                             .uri().GET();
            
            var extraCode = @"
//using FluentSharp.CassiniDev
//O2Ref:FluenSharp.CassiniDev.dll
//O2Ref:TeamMentor.UnitTests.TM_Website.dll
";
     

            "Cassini Dev Test".add_IE_PopupWindow()
                              .open(api_Cassini.url())
                              .script_IE()
                              .add_InvocationParameter("cassini", api_Cassini)
                              .code_Append(extraCode)
                              .waitForClose();

        }
    }
}
