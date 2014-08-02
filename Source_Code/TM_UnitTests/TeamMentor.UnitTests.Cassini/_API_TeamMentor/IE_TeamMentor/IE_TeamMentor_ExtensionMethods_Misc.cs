using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.Watin;
using FluentSharp.WatiN;
using FluentSharp.WatiN.NUnit;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Misc
    {
        //ie helpers
        public static IE_TeamMentor click(this IE_TeamMentor ieTeamMentor, string linkOrButtonRef)
        {
            var result = Extra_Watin__Extension_Methods.click(ieTeamMentor.ie(), linkOrButtonRef);
            //var result =  ieTeamMentor.ie().click(linkOrButtonRef).notNull();
            return result.notNull()
                        ? ieTeamMentor
                        : null;
        }
        public static WatiN_IE ie(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.notNull() ? ieTeamMentor.ie : null;
        }
        public static IE_TeamMentor open(this IE_TeamMentor ieTeamMentor, string virtualPath = "")
        {
            var fullUri = ieTeamMentor.siteUri.append(virtualPath);
            if(fullUri.notNull())
                ieTeamMentor.ie.open(fullUri.str());
            return ieTeamMentor;
        }
        public static IE_TeamMentor refresh(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.refresh();
            return ieTeamMentor;
        }

        public static string html(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.ie.html();            
        }        
        public static ascx_Simple_Script_Editor script_IE(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.parentForm().show();                                        //in case it was started in hidden mode
            return ieTeamMentor.ie.script_IE()
                               .add_InvocationParameter("ieTeamMentor", ieTeamMentor)
                               .code_Insert("return ieTeamMentor;")                               
                               .code_Append("//using TeamMentor.UnitTests.Cassini")
                               .code_Append("//O2Ref:TeamMentor.UnitTests.Cassini.dll");
                               
        }
        public static IE_TeamMentor script_IE_WaitForComplete(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.script_IE().parentForm()
                                    .waitForClose();
            return ieTeamMentor;
        }
    }
}