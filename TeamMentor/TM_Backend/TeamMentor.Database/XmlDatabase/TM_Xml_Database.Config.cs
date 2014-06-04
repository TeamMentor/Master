using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluentSharp.CoreLib;

using TeamMentor.UserData;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Config
    {

        [Admin] public static TM_Xml_Database   set_Default_Values(this TM_Xml_Database tmXmlDatabase)
        {                        
            tmXmlDatabase.Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();
            tmXmlDatabase.GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            tmXmlDatabase.GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();
            tmXmlDatabase.GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer, string>();            
            tmXmlDatabase.VirtualArticles             = new Dictionary<Guid, VirtualArticleAction>();
            //tmXmlDatabase.Events                      = new Events_TM_Xml_Database(tmXmlDatabase);                        
            tmXmlDatabase.Path_XmlLibraries           = null;
            tmXmlDatabase.UserData                    = null;
;

            tmXmlDatabase.Events.After_Set_Default_Values.raise();

            return tmXmlDatabase;
        }
        /// <summary>
        /// This is the function that calculates the path to the TM XML Database (i.e. local file storage of TM files)
        /// </summary>
        /// <param name="tmXmlDatabase">this</param>
        /// <returns></returns>
               



/*TODO*/[Admin] public static TM_Xml_Database   load_SiteData(this TM_Xml_Database tmXmlDatabase)         
        {
            
            tmXmlDatabase.Events.After_Load_SiteData.raise();
            return tmXmlDatabase;
        }
    
        public static TM_UserData               userData(this TM_Xml_Database tmDatabase)                 
        {
            if (tmDatabase.isNull())
                return null;
            if (tmDatabase.UserData.isNull())
            { 
                tmDatabase.UserData = new TM_UserData();
                tmDatabase.Events.After_UserData_Ctor.raise();
            }
            return tmDatabase.UserData;
        }        
        

       
    }
}
