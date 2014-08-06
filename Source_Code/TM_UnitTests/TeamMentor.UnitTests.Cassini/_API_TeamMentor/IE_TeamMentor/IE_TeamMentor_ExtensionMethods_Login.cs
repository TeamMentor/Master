using System;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using FluentSharp.Watin;
using FluentSharp.WatiN;
using FluentSharp.Web;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    public static class IE_TeamMentor_ExtensionMethods_Login
    {
        public static IE_TeamMentor login_Using_AuthToken(this IE_TeamMentor ieTeamMentor, Guid authToken)
        {
            return ieTeamMentor.open("whoami?auth={0}".format(authToken));
        }
        public static IE_TeamMentor login(this IE_TeamMentor ieTeamMentor, string username, string password, string loginReferer = "/whoami")
        {   
            var ie = ieTeamMentor.ie;
            
            var expectedUri = ieTeamMentor.siteUri.append(loginReferer);                                    // map the expected login OK redirection target
            
            ieTeamMentor.open("login?LoginReferer={0}".format(loginReferer));

            ie.field("username").value(username);                                                           // populate login fields
            ie.field("password").value(password);                    
            ie.button("login").click();
                
            ie.wait_For_Uri(expectedUri);                                                                   // wait for redirection to expected page
            return ieTeamMentor;
        }
        
        public static IE_TeamMentor open_Page_Login_if_Needed(this IE_TeamMentor ieTeamMentor, string page)
        {            
            ieTeamMentor.open(page);
            return ieTeamMentor.login_Default_Admin_Account(page);
        }
        public static IE_TeamMentor open_TBot_Login_if_Needed(this IE_TeamMentor ieTeamMentor, string tbotPage = "Commands")
        {            
            var tbotLink    = "/rest/tbot/run/".append(tbotPage);
            var expectedUrl = ieTeamMentor.siteUri.mapPath(tbotLink);
            ieTeamMentor.open(tbotLink);
            return  ieTeamMentor.ie.uri() == expectedUrl                            // check if the page opened is the expected one
                        ? ieTeamMentor                                              // if it is continue
                        : ieTeamMentor.login_Default_Admin_Account(tbotLink);       // if not login with default admin account
        }
        public static IE_TeamMentor login_Default_Admin_Account(this IE_TeamMentor ieTeamMentor, string loginReferer = "/whoami")
        {
            return ieTeamMentor.login("admin", "!!tmadmin", loginReferer);          
        }

        public static WhoAmI whoAmI(this IE_TeamMentor ieTeamMentor)
        {
             return ieTeamMentor.open("/whoami")
                                .ie.body().innerHtml()
                                .json_Deserialize<WhoAmI>();
        }
        public static bool am_I_Default_Admin(this IE_TeamMentor ieTeamMentor)
        {
            return ieTeamMentor.whoAmI().UserName == "admin";
        }

    }
}