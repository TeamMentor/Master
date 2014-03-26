using System;
using System.Threading;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{
    public class IE_UnitTest
    {
        public static IE_UnitTest Current        { get; set; }

        public        String      TargetServer   { get; set;}        
        public        Form        parentForm;
        public        WatiN_IE    ie;          

        public IE_UnitTest()
        {
           this.open_IE(); 
        }        
        
        public string html()
        {
            return ie.html();
        }
        public IE_UnitTest waitForClose()
        {
            parentForm.waitForClose();
            return this;
        }
        public IE_UnitTest script_IE_WaitForClose()
        {
            script_IE();
            return waitForClose();
        }
        public ascx_Simple_Script_Editor script_IE()
        {
            var code = 
@"return ie.url();

//O2Ref:FluentSharp.WatiN.dll
//using FluentSharp.Watin;
//O2Ref:WatiN.Core.dll";

            return ie.script_Me("ie")
              .set_Code(code);            
        }
    }

    public static class IE_TBot_ExtensionMethods
    {
        public static Form newForm_Minimized<T>(this T tbot, int width, int height) where T : IE_UnitTest
        {
            var windowState = FormWindowState.Minimized; //FormWindowState.Normal; 
            Form form = null;             
            
            var formShown = new AutoResetEvent(false);
            O2Thread.staThread(()=>
                {
                    form      = new Form
                        {
                            WindowState = windowState,
                            Height      = height,
                            Width       = width
                        };                                                                                                           
                    form.Shown += (sender, e) => formShown.Set();                                                                            
                    form.ShowDialog();                                                                                                        
                });
            
            //wait for Form Load event            
            formShown.WaitOne(1000); // it should not take more than one sec to start the form
            form.set_DefaultIcon();

            //check that all is good
            Assert.NotNull (form);                       
            Assert.IsEmpty (form.controls());
            Assert.AreEqual(form.WindowState, windowState);
            if (form.WindowState == FormWindowState.Minimized)
            {
                Assert.AreEqual(form.Height     , 24);                          // these two values might be OS specific
                Assert.AreEqual(form.Width      , 160);
            }
            else
            {
                Assert.AreEqual(form.Height     , height);
                Assert.AreEqual(form.Width      , width);
            }
            return form;
        }
        public static T close_IE<T>(this T tbot) where T : IE_UnitTest
        {
            tbot.parentForm.close();
            IE_UnitTest.Current = null;
            return tbot;
        }
        public static T open_IE<T>(this T tbot) where T : IE_UnitTest
        {
            if (IE_UnitTest.Current.isNull())
            {
                IE_UnitTest.Current = tbot;            
                tbot.parentForm   = tbot.newForm_Minimized(800, 800);
                //tbot.parentForm = panel.parentForm()                                       
                //                       .width(800)
                //                       .height(500);                
                tbot.ie         = tbot.parentForm.add_IE();
            }
            else
            {
                tbot.parentForm = IE_UnitTest.Current.parentForm;
                tbot.ie         = IE_UnitTest.Current.ie;
            }
            return tbot;
        }

        public static IE_UnitTest open(this IE_UnitTest ieUnitTest, string virtualPath)     // can't use generics here in order to have simpler method calls: tbot.open("....");  vs tbot.open<API_IE_TBot>("...");
        {
            return ieUnitTest.open_Page(virtualPath);
        }

        public static IE_UnitTest open_Page(this IE_UnitTest tbot, string virtualPath)
        {
            tbot.ie.open(tbot.fullPath(virtualPath));
            return tbot;
        }
        public static string fullPath(this IE_UnitTest tbot, string virtualPath)
        {
            return tbot.TargetServer.uri().append(virtualPath).str();
        }

        public static IE_UnitTest open_ASync(this IE_UnitTest tbot, string virtualPath)
        {
            O2Thread.mtaThread(()=> tbot.open_Page(virtualPath));
            return tbot;
        }
    }
}
