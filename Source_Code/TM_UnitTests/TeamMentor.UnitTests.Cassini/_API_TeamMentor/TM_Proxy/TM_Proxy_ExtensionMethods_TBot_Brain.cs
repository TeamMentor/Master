using System.Collections.Generic;
using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_TBot_Brain
    {
        public static TBot_Brain tbot_Brain(this TeamMentor_Objects_Proxy tmProxy)
        {
            if (tmProxy.isNull())
                return null;
            var tbotBrain = tmProxy.invoke_Static<TBot_Brain>(typeof(TBot_Brain), "Create");

            if(tmProxy.tbot_Brain_TBotScriptsFolder().folder_Not_Exists())
            {
                var path_TBotScripts   = tmProxy.apiCassini.webRoot().pathCombine("Tbot");
                tmProxy.tbot_Brain_TBotScriptsFolder(path_TBotScripts);
                tmProxy.tbot_Brain_SetAvailableScripts();
            }            
            return tbotBrain;                      
        }
        
        public static string tbot_Brain_TBotScriptsFolder(this TeamMentor_Objects_Proxy tmProxy)
        {
            return  tmProxy.get_Property_Static<TBot_Brain,string>("TBotScriptsFolder");
        }
        public static TeamMentor_Objects_Proxy tbot_Brain_TBotScriptsFolder(this TeamMentor_Objects_Proxy tmProxy, string value)
        {
            tmProxy.set_Property_Static<TBot_Brain>("TBotScriptsFolder", value);  
            return tmProxy;
        }        
        public static Dictionary<string, string> tbot_Brain_SetAvailableScripts(this TeamMentor_Objects_Proxy tmProxy)
        {
            return tmProxy.invoke_Static<Dictionary<string, string>>(typeof(TBot_Brain),"SetAvailableScripts");    
        }

        public static Dictionary<string, string> tbot_Brain_AvailableScripts(this TeamMentor_Objects_Proxy tmProxy)
        {
            return  tmProxy.get_Property_Static<TBot_Brain,Dictionary<string, string>>("AvailableScripts");
        }

    }
}