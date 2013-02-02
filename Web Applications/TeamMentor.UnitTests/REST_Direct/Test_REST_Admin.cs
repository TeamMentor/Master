using FluentSharp;
using NUnit.Framework;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST_Direct
{
    [TestFixture]
    public class Test_REST_Admin : TM_Rest_Direct
    {
        public Test_REST_Admin()
        {
            UserGroup.Admin.setThreadPrincipalWithRoles();
        }

        //O2 Script Library
        [Test]
        public void CompileAllScripts()
        {
            PublicDI.log.writeToDebug(true);
            CompileEngine.clearCompilationCache();
            foreach (var method in typeof (O2_Script_Library).methods())
            {
                var code = method.invoke().str();
                var assembly = code.compileCodeSnippet();
                Assert.IsNotNull(assembly, "Failed for compile {0} with code: \n\n {1}".format(method.Name, code));
                "Compiled OK: {0}".info(method.Name);                
            }
        }
        [Test]
        public void Invoke_O2_Script_Library()
        {
            var result = TmRest.Admin_InvokeScript("AAAAAAAAAA");            
            Assert.AreEqual("script not found", result);
            result = TmRest.Admin_InvokeScript("ping");            
            Assert.AreEqual("pong", result);
            
        }

    }
}
