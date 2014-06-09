using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage.XmlDatabase
{
    public static class FileStorage_Articles
    {
        public static bool article_Save(this TM_Xml_Database tmXmlDatabase, TeamMentor_Article article)
        {
            var libraryId = article.Metadata.Library_Id;
                        
            var guidanceXmlPath = tmXmlDatabase.getXmlFilePathForGuidanceId(article.Metadata.Id, libraryId);
            if (guidanceXmlPath.valid())
            {
                "Saving GuidanceItem {0} to {1}".info(article.Metadata.Id, guidanceXmlPath);
                article.saveAs(guidanceXmlPath);
                return guidanceXmlPath.fileExists();
            }            
            return true;
        }

        public static bool article_Delete(this TM_Xml_Database tmDatabase,  TeamMentor_Article article)
        {
            var guidanceItemId = article.Metadata.Id;

             var guidanceItemXmlPath = tmDatabase.removeGuidanceItemFileMapping(guidanceItemId);
            "removing GuidanceItem with Id:{0} located at {1}".info(guidanceItemId, guidanceItemXmlPath);
            if (guidanceItemXmlPath.valid())				
                Files.deleteFile(guidanceItemXmlPath);
            
            tmDatabase.Events.Articles_Cache_Updated.raise(); //tmDatabase.queue_Save_GuidanceItemsCache();

            //TM_Xml_Database.mapGuidanceItemsViews();
            return true;
        }


        [ReadArticles]  public static string xmlDB_guidanceItemXml(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            if (guidanceItemId ==  Guid.Empty)
                return null;
            var guidanceXmlPath = tmDatabase.getXmlFilePathForGuidanceId(guidanceItemId);
            return guidanceXmlPath.fileContents();//.xmlFormat();
        }

        [Admin]	        public static string xmlDB_guidanceItemPath(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            if (guidanceItemId !=  Guid.Empty)                
                if (TM_Xml_Database.Current.guidanceItems_FileMappings().hasKey(guidanceItemId))                            
                    return TM_Xml_Database.Current.guidanceItems_FileMappings()[guidanceItemId];            
            return null;
        }


        public static Dictionary<Guid, string> guidanceItems_FileMappings(this TM_Xml_Database tmDatabase)
        {
            var tmFileStorage = TM_FileStorage.Current;
            return tmFileStorage.GuidanceItems_FileMappings;
        }
    }
}
