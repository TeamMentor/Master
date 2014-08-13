using System.Runtime.Remoting;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_Misc
    {
        public static TM_Proxy  map_ReferencesToTmObjects(this TM_Proxy tmProxy)
        {
            TMConfig        .Current = tmProxy.TmConfig      = tmProxy.get_Current<TMConfig       >();
            TM_FileStorage  .Current = tmProxy.TmFileStorage = tmProxy.get_Current<TM_FileStorage >();            
            TM_Status       .Current = tmProxy.TmStatus      = tmProxy.get_Current<TM_Status      >();
            TM_Server       .Current = tmProxy.TmServer      = tmProxy.get_Current<TM_Server      >();
            TM_UserData     .Current = tmProxy.TmUserData    = tmProxy.get_Current<TM_UserData    >();
            TM_Xml_Database .Current = tmProxy.TmXmlDatabase = tmProxy.get_Current<TM_Xml_Database>();
                      
            return tmProxy;   
        }
        

        public static bool   isTransparentObject(object target)
        {
         return RemotingServices.IsTransparentProxy(target); 
        }
    }
}