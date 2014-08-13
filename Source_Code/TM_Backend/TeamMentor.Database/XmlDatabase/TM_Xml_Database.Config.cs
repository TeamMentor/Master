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
            UserRole.Admin.demand();
            tmXmlDatabase.Cached_GuidanceItems        = new Dictionary<Guid, TeamMentor_Article>();            
            tmXmlDatabase.GuidanceExplorers_XmlFormat = new Dictionary<Guid, guidanceExplorer>();            
            tmXmlDatabase.VirtualArticles             = new Dictionary<Guid, VirtualArticleAction>();
            //tmXmlDatabase.Events                      = new Events_TM_Xml_Database(tmXmlDatabase);                                                

            tmXmlDatabase.Events.After_Set_Default_Values.raise();

            return tmXmlDatabase;
        }
        /// <summary>
        /// This is the function that calculates the path to the TM XML Database (i.e. local file storage of TM files)
        /// </summary>
        /// <param name="tmXmlDatabase">this</param>
        /// <returns></returns>
               



        [Admin] public static TM_Xml_Database   load_SiteData(this TM_Xml_Database tmXmlDatabase)         
        {
            UserRole.Admin.demand();
            tmXmlDatabase.Events.After_Load_SiteData.raise();
            return tmXmlDatabase;
        }
       
    }
}
