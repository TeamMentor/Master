using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_TM_Settings
    {
        public static TM_Proxy  set_Custom_WebRoot              (this TM_Proxy tmProxy, string value)
        {
            tmProxy.set_Property_Static<TM_FileStorage>("Custom_WebRoot", value);
            return tmProxy;
        }
        public static string    get_Custom_WebRoot               (this TM_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<TM_FileStorage, string>("Custom_WebRoot");
        }
        public static TM_Proxy  set_Custom_Path_XmlDatabase     (this TM_Proxy tmProxy, string value)
        {
            tmProxy.set_Property_Static<TM_FileStorage>("Custom_Path_XmlDatabase", value);
            return tmProxy;
        }
        public static string    get_Custom_Path_XmlDatabase     (this TM_Proxy tmProxy)
        {
            return tmProxy.get_Property_Static<TM_FileStorage, string>("Custom_Path_XmlDatabase");
        }
        
        public static TM_Proxy 	show_ContentToAnonymousUsers    (this TM_Proxy tmProxy, bool value)
        {
            tmProxy.TmConfig.TMSecurity.Show_ContentToAnonymousUsers = value;
            return tmProxy;
        }
            
        
    }
}