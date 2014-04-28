using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods
    {
        
        public static TM_UserData     userData                   (this TM_Xml_Database tmDatabase)  
        {
            return tmDatabase.notNull()
                       ? tmDatabase.UserData
                       : null;
        }
        public static bool            setupThread_Active         (this TM_Xml_Database tmDatabase)  
        {
            return tmDatabase.SetupThread.isNull();
        }
        public static TM_Xml_Database setupThread_WaitForComplete(this TM_Xml_Database tmDatabase)  
        {
            if (tmDatabase.SetupThread.notNull())
                tmDatabase.SetupThread.Join();
            return tmDatabase;
        }        
        public static TM_Xml_Database reload_WithoutFileStorage  (this TM_Xml_Database tmDatabase)  
        {
            //disable for now (need to add a load_WithoutFileStorage (or the Security unit tests will leave the DB in this mode
         //   return new TM_Xml_Database();
            return null;
        }        
    }
}