using System;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN.NUnit;
using FluentSharp.WinForms;
using TeamMentor.CoreLib;

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
        
        public IE_TeamMentor open(string virtualPath = "")
        {
            var fullUri = siteUri.append(virtualPath);
            if(fullUri.notNull())
                ie.open(fullUri.str());
            return this;
        }
    }

    public static class API_TeamMentor_WatiN_ExtensionMethods_CTors
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
            ieTeamMentor.ie.parentForm().close();
            return ieTeamMentor;
        }
    }
    public static class API_TeamMentor_WatiN_ExtensionMethods
    {
        //ie helpers
        public static IE_TeamMentor refresh(this IE_TeamMentor ieTeamMentor)
        {
            ieTeamMentor.ie.refresh();
            return ieTeamMentor;
        }

        public static string html(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.ie.html();            
        }

        //pages
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

        public static IE_TeamMentor login_Using_AuthToken(this IE_TeamMentor ieTeamMentor, Guid authToken)
        {
            return ieTeamMentor.open("whoami?auth={0}".format(authToken));
        }

        public static IE_TeamMentor article(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return ieTeamMentor.article(tmArticle.Metadata.Id);
        }

        public static IE_TeamMentor article(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("article/{0}".format(id));
        }
        public static IE_TeamMentor article_Html(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return (ieTeamMentor.notNull() && tmArticle.notNull()) 
                        ? ieTeamMentor.article_Html(tmArticle.Metadata.Id)
                        : ieTeamMentor;
        }

        public static IE_TeamMentor article_Html(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("html/{0}".format(id));
        }

        public static IE_TeamMentor article_Raw(this IE_TeamMentor ieTeamMentor, TeamMentor_Article tmArticle)
        {
            return ieTeamMentor.article_Raw(tmArticle.Metadata.Id);
        }

        public static IE_TeamMentor article_Raw(this IE_TeamMentor ieTeamMentor, Guid id)
        {
            return ieTeamMentor.open("raw/{0}".format(id));
        }
        
        public static IE_TeamMentor login(this IE_TeamMentor ieTeamMentor, string username, string password, string loginReferer = "/whoami")
        {   
            var ie = ieTeamMentor.ie;

            var expectedUri = ieTeamMentor.siteUri.append(loginReferer);			
            ieTeamMentor.open("login?LoginReferer={0}".format(loginReferer));

			ie.field("username").value(username);
			ie.field("password").value(password);                    
			ie.button("login").click();

            ie.wait_For_Uri(expectedUri);
			return ieTeamMentor;
        }

        public static IE_TeamMentor login_Default_Admin_Account(this IE_TeamMentor ieTeamMentor, string loginReferer = "/whoami")
        {
            return ieTeamMentor.login("admin", "!!tmadmin", loginReferer);          
        }

    }
}
