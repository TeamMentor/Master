using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.Watin;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.TM_Website
{
    public class IE_TBot
    {
        public static IE_TBot Current { get; set; }

        public WatiN_IE        ie; 
        public Form            parentForm;

        /*static IE_TBot()
        {
            Current = new IE_TBot();
        }*/

        public IE_TBot()
        {
           openIE(); 
        }

        public IE_TBot openIE()
        {
            if (Current.isNull())
            {
                Current = this;            
                var panel  = "IE TBot".popupWindow(800,500);
                parentForm = panel.parentForm();
                ie         = panel.add_IE();
            }
            else
            {
                parentForm = Current.parentForm;
                ie         = Current.ie;
            }
            return this;
        }
        
        public string html()
        {
            return ie.html();
        }
        public IE_TBot waitForClose()
        {
            parentForm.waitForClose();
            return this;
        }
        public IE_TBot script_IE_WaitForClose()
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
}
