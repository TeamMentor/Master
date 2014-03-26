using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.TM_Website
{
    public class API_IE_TBot : IE_UnitTest
    {        
        public API_IE_TBot()
        {
            TargetServer = "http://localhost:3187";            
        }                
        public API_IE_TBot tbot_V1()
        {
            this.open("tbot");
            return this;
        }
        public API_IE_TBot tbot_Users()
        {
            this.open("tbot_users");
            return this;
        }                   
    }

    public static class API_IE_TBot_ExtensiomMethods
    {
        public static T page_TBot<T>(this T tBot) where T : API_IE_TBot
        {
            tBot.open("/tbot");       
            return tBot;
        }

        public static API_IE_TBot page_Login(this API_IE_TBot tBot)
        {
            return tBot.page_Login("");
        }
        public static API_IE_TBot page_Login(this API_IE_TBot tBot, string loginReferer)
        {
            tBot.open_Page(loginReferer.valid() 
                                ? "login?LoginReferer={0}".format(loginReferer) 
                                : "login");            
            return tBot;
        } 
        public static API_IE_TBot login_As_Admin(this API_IE_TBot tBot)
        {
            tBot.open_Page("rest/login/{0}/{1}".format(Tests_Consts.DEFAULT_ADMIN_USERNAME,
                                                       Tests_Consts.DEFAULT_ADMIN_PASSWORD));                
            return tBot;
        }
    }
}