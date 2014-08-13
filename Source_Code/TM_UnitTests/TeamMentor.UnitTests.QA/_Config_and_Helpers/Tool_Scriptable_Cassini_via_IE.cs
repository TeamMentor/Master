using FluentSharp.NUnit;
using FluentSharp.REPL;
using FluentSharp.Watin;
using FluentSharp.Web35;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA.Tools
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

            var script = nUnitTests_Cassini.script_IE();

            script.parentForm()
                  .set_H2Icon()
                  .waitForClose();
        }
    }
}
