using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.FileStorage.VirtualArticles
{
    public static class FileStorage_VirtualArticles
    {
         [Admin] public static TM_FileStorage                hook_Events_VirtualArticles(this TM_FileStorage tmFileStorage)
        {
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            tmXmlDatabase.Events.VirtualArticles_Loaded.add((tmArticle)=> tmXmlDatabase.loadVirtualArticles());
            tmXmlDatabase.Events.VirtualArticles_Saved.add((tmArticle)=> tmXmlDatabase.saveVirtualArticles());

//            tmXmlDatabase.Articles_Cache_Updated.add((tmArticle)=> tmXmlDatabase.queue_Save_GuidanceItemsCache());

            return tmFileStorage;
        }
        public static TM_Xml_Database loadVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
            var virtualArticles = tmXmlDatabase.VirtualArticles;
            virtualArticles.Clear();

			var virtualArticles_ToMap = tmXmlDatabase.loadVirtualArticles_FromDisk();
			foreach (var virtualArticle in virtualArticles_ToMap)
				virtualArticles.add(virtualArticle.Id, virtualArticle);
			return tmXmlDatabase;
		}
        public static List<VirtualArticleAction> loadVirtualArticles_FromDisk(this TM_Xml_Database tmXmlDatabase)
		{            
			var virtualArticlesFile = tmXmlDatabase.getVirtualArticlesFile();
			if (virtualArticlesFile.fileExists())
				return virtualArticlesFile.load<List<VirtualArticleAction>>();
			return new List<VirtualArticleAction>();
		}
        public static string getVirtualArticlesFile(this TM_Xml_Database tmXmlDatabase)
		{
			//return TM_Xml_Database.Path_XmlDatabase.pathCombine("Virtual_Articles.xml");
			return TM_Xml_Database.Current.path_XmlLibraries().pathCombine("Virtual_Articles.xml");
		}
        public static TM_Xml_Database saveVirtualArticles(this TM_Xml_Database tmXmlDatabase)
		{
			var virtualArticlesFile = tmXmlDatabase.getVirtualArticlesFile();

			var virtualArticles = tmXmlDatabase.getVirtualArticles().Values.toList();
			virtualArticles.saveAs(virtualArticlesFile);
			return tmXmlDatabase;
		}
    }
}
