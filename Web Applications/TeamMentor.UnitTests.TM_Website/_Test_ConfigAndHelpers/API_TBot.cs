using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.Watin;

namespace TeamMentor.UnitTests.TM_Website
{
    public class API_IE_TBot : IE_TBot
    {
        public string TargetServer {get; set;}

        public API_IE_TBot()
        {
            TargetServer = "http://localhost:3187";            
        }
        public API_IE_TBot open_ASync(string virtualPath)
        {
            O2Thread.mtaThread(()=> open(virtualPath));
            return this;
        }
        public API_IE_TBot open(string virtualPath)
        {
            ie.open(fullPath(virtualPath));
            return this;
        }
        public string fullPath(string virtualPath)
        {
            return TargetServer.uri().append(virtualPath).str();
        }
        public API_IE_TBot tbot_V1()
        {
            open("tbot");
            return this;
        }
        public API_IE_TBot tbot_V2()
        {
            open("tbot_v2");
            return this;
        }
                   
    }
    public static class API_IE_TBot_ExtensiomMethods
    {
        public static T page_Login<T>(this T tBot) where T : API_IE_TBot
        {
            return tBot.page_Login("");
        }
        public static T page_Login<T>(this T tBot, string loginReferer) where T : API_IE_TBot
        {
            tBot.open(loginReferer.valid() 
                            ? "login?LoginReferer={0}".format(loginReferer) 
                            : "login");            
            return tBot;
        } 
        public static T login_As_Admin<T>(this T tBot) where T : API_IE_TBot
        {
            tBot.open("rest/login/admin/!!tmadmin");    
            return tBot;
        }
    }
}
