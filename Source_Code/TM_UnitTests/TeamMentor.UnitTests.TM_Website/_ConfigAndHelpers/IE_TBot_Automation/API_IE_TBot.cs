using FluentSharp.CoreLib;
using FluentSharp.Watin;
using NUnit.Framework;

namespace TeamMentor.UnitTests.TM_Website
{

    public static class IE_UnitTest_ExtensiomMethods_For_TBot
    {
        public static T page_TBot<T>(this T tBot) where T : IE_UnitTest
        {
            tBot.open("/tbot");       
            return tBot;
        }

        public static IE_UnitTest page_Login(this IE_UnitTest tBot)
        {
            return tBot.page_Login("");
        }
        public static IE_UnitTest page_Login(this IE_UnitTest tBot, string loginReferer)
        {
            tBot.open_Page(loginReferer.valid() 
                                ? "login?LoginReferer={0}".format(loginReferer) 
                                : "login");            
            return tBot;
        } 
        public static IE_UnitTest login_As_Admin(this IE_UnitTest tBot)
        {
            tBot.open_Page("rest/login/{0}/{1}".format(Tests_Consts.DEFAULT_ADMIN_USERNAME,
                                                       Tests_Consts.DEFAULT_ADMIN_PASSWORD));                
            return tBot;
        }

        public static IE_UnitTest assert_Admin(this IE_UnitTest  tBot)
        {
            return null;
        }
    }
}