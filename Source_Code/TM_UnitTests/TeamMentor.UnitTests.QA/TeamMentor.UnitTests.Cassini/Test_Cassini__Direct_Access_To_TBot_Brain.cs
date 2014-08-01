using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.WinForms;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;
using TeamMentor.UnitTests.Cassini;

namespace TeamMentor.UnitTests.QA
{
    [TestFixture]
    public class Test_Cassini__Direct_Access_To_TBot_Brain : NUnitTests_Cassini_TeamMentor
    {         
        [SetUp] public void setup()
        {            
            this.tmProxy_Refresh();
        }    
        [Test] public void TBot_Brain__Create__SetAvailableScripts__ExecuteRazorPage()
        {
            var tbotBrain = tmProxy.invoke_Static<TBot_Brain>(typeof(TBot_Brain), "Create")
                                   .assert_Not_Null();
            
            var tbotScripts      = webRoot.pathCombine("Tbot");
            var tbotPage_Name    = "testPage";
            tmProxy.set_Property_Static<TBot_Brain>("TBotScriptsFolder", tbotScripts);
            var availableScripts = tmProxy.invoke_Static<Dictionary<string, string>>(typeof(TBot_Brain),"SetAvailableScripts")
                                          .assert_Not_Empty();
            
            availableScripts.hasKey(tbotPage_Name).assert_True();
            var tbotPage_Path  = availableScripts.value(tbotPage_Name).assert_File_Exists();
            tbotBrain.ExecuteRazorPage("testPage").assert_Is(tbotPage_Path.fileContents());
            
        }
        //var path_SiteData = tmFileStorage.path_SiteData().assert_Folder_Exists();
        [Test] public void TBot_Brain__SetAvailableScripts()
        {   
            var tbotScripts   = webRoot.pathCombine("Tbot");

            tmProxy.set_Property_Static<TBot_Brain>       ("TBotScriptsFolder", "");                      
            tmProxy.get_Property_Static<TBot_Brain,string>("TBotScriptsFolder").assert_Not_Valid();            
            tmProxy.set_Property_Static<TBot_Brain>       ("TBotScriptsFolder", tbotScripts);            
            tmProxy.get_Property_Static<TBot_Brain,string>("TBotScriptsFolder").assert_Is(tbotScripts);
            
            var avaiableScripts = tmProxy.invoke_Static<Dictionary<string, string>>(typeof(TBot_Brain),"SetAvailableScripts")
                                         .assert_Not_Null();

            avaiableScripts.keys().assert_Bigger_Than(20)
                                  .assert_Contains("testPage");
        }

        [Test] public void TBot_Brain__TmProxy_HelperMethods()
        {
            
        }
    }
}
