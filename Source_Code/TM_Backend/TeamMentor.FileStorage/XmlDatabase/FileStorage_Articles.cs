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
        [EditArticles]
        public static bool article_Save(this TM_FileStorage tmFileStorage, TeamMentor_Article article)
        {            
            editArticles.demand();
            var libraryId = article.Metadata.Library_Id;
                        
            var guidanceXmlPath = tmFileStorage.getXmlFilePathForGuidanceId(article.Metadata.Id, libraryId);
            if (guidanceXmlPath.valid())
            {
                "Saving GuidanceItem {0} to {1}".info(article.Metadata.Id, guidanceXmlPath);
                article.saveAs(guidanceXmlPath);
                return guidanceXmlPath.fileExists();
            }            
            return true;
        }
        [EditArticles]
        public static bool article_Delete(this TM_FileStorage tmFileStorage,  TeamMentor_Article article)
        {
            editArticles.demand();
            var guidanceItemId = article.Metadata.Id;

             var guidanceItemXmlPath = tmFileStorage.removeGuidanceItemFileMapping(guidanceItemId);
            "removing GuidanceItem with Id:{0} located at {1}".info(guidanceItemId, guidanceItemXmlPath);
            if (guidanceItemXmlPath.valid())				
                Files.deleteFile(guidanceItemXmlPath);
            
            tmFileStorage.tmXmlDatabase().Events.Articles_Cache_Updated.raise(); //tmDatabase.queue_Save_GuidanceItemsCache();

            //TM_Xml_Database.mapGuidanceItemsViews();
            return true;
        }


        [ReadArticles]  public static string xmlDB_guidanceItemXml(this TM_FileStorage tmFileStorage, Guid guidanceItemId)
        {
            UserRole.ReadArticles.demand();
            if (guidanceItemId ==  Guid.Empty)
                return null;
            var guidanceXmlPath = tmFileStorage.getXmlFilePathForGuidanceId(guidanceItemId);
            return guidanceXmlPath.fileContents();//.xmlFormat();
        }

        [Admin]	        public static string xmlDB_guidanceItemPath(this TM_FileStorage tmFileStorage, Guid guidanceItemId)
        {
            UserRole.Admin.demand();
            if (guidanceItemId !=  Guid.Empty)                
                if (tmFileStorage.guidanceItems_FileMappings().hasKey(guidanceItemId))                            
                    return tmFileStorage.guidanceItems_FileMappings()[guidanceItemId];            
            return null;
        }


        public static Dictionary<Guid, string> guidanceItems_FileMappings(this TM_FileStorage tmFileStorage)
        {
            return tmFileStorage.notNull()
                        ? tmFileStorage.GuidanceItems_FileMappings
                        : new Dictionary<Guid, string>();            
        }
    }
}
