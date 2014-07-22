using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.NUnit;

namespace TeamMentor.UnitTests.Cassini
{
    [TestFixture]
    public class Test_TM_Proxy_ExtensionMethods_TBot_Brain :NUnitTests_Cassini_TeamMentor
    {
        [SetUp] public void setUp()
        {
            this.tmProxy_Refresh();
        }
        [Test] public void tbot_Brain()
        {
            var tBotBrain = tmProxy.tbot_Brain().assert_Not_Null();                         //need to call this first (before tbot_Brain_TBotScriptsFolder and tbot_Brain_AvailableScripts)

            var expected_Path_TBotScripts = webRoot.pathCombine("Tbot");

            tmProxy.tbot_Brain_TBotScriptsFolder().assert_Is(expected_Path_TBotScripts);
            tmProxy.tbot_Brain_AvailableScripts ().assert_Not_Empty()
                                                  .assert_Bigger_Than(20)
                                                  .assert_Has_Key("testPage")
                                                  .assert_Size_Is(tBotBrain.availableScripts().size());  
                                                
        }        
    }
}