using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Create
    {
        public static IE_TeamMentor new_IE_TeamMentor_Hidden(this NUnitTests_Cassini_TeamMentor nunitTests_Cassini, bool forceStartVisible = false)
        {
            return nunitTests_Cassini.new_IE_TeamMentor(forceStartVisible.isFalse());
        }
        public static IE_TeamMentor new_IE_TeamMentor(this NUnitTests_Cassini_TeamMentor nunitTests_Cassini, bool startHidden = false)
        {
            var webRoot           = nunitTests_Cassini.webRoot          .assert_Folder_Exists();
            var path_XmlLibraries = nunitTests_Cassini.path_XmlLibraries.assert_Folder_Exists();
            var siteUri           = nunitTests_Cassini.siteUri.assert_Not_Null();
            return new IE_TeamMentor(webRoot, path_XmlLibraries, siteUri, startHidden);
        }

        public static IE_TeamMentor waitForClose(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.parentForm().waitForClose();
            return ieTeamMentor;
        }
        
        public static IE_TeamMentor close(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.close();
            ieTeamMentor.ie.parentForm().close();
            return ieTeamMentor;
        }
    }
}