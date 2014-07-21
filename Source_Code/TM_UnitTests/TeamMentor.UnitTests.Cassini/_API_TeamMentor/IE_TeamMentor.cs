using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WinForms;

namespace TeamMentor.UnitTests.Cassini
{
    public class IE_TeamMentor
    {
        public WatiN_IE ie;
        public string   webRoot;
        public string   path_XmlLibraries;
        public Uri      siteUri;
        public IE_TeamMentor(string webRoot, string path_XmlLibraries, Uri siteUri, bool startHidden)
        {
            this.ie                = "Test_IE_TeamMentor".popupWindow(1000,700,startHidden).add_IE();            
            this.path_XmlLibraries = path_XmlLibraries;
            this.webRoot           = webRoot;
            this.siteUri           = siteUri;
        }
        
        public IE_TeamMentor open(string virtualPath)
        {
            var fullUri = siteUri.append(virtualPath);
            if(fullUri.notNull())
                ie.open(fullUri.str());
            return this;
        }
    }

    public static class API_TeamMentor_WatiN_ExtensionMethods_CTors
    {
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
            ieTeamMentor.ie.parentForm().close();
            return ieTeamMentor;
        }
    }
    public static class API_TeamMentor_WatiN_ExtensionMethods
    {
        public static IE_TeamMentor page_Admin(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Admin");
        }
        public static IE_TeamMentor page_Home(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/TeamMentor");
        }    
        public static IE_TeamMentor page_Login(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Login");
        }        
        public static IE_TeamMentor page_TBot(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.open("/Tbot");
        }
    }
}
