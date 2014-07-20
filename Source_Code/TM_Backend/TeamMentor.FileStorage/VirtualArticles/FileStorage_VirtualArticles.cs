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
            UserRole.Admin.demand();
            var tmXmlDatabase = tmFileStorage.TMXmlDatabase;

            tmXmlDatabase.Events.VirtualArticles_Loaded.add((tmArticle)=> tmFileStorage.loadVirtualArticles());
            tmXmlDatabase.Events.VirtualArticles_Saved.add((tmArticle)=> tmFileStorage.saveVirtualArticles());

//            tmXmlDatabase.Articles_Cache_Updated.add((tmArticle)=> tmXmlDatabase.queue_Save_GuidanceItemsCache());

            return tmFileStorage;
        }
        public static TM_FileStorage loadVirtualArticles(this TM_FileStorage tmFileStorage)
		{
            var virtualArticles = tmFileStorage.tmXmlDatabase().VirtualArticles;
            virtualArticles.Clear();

			var virtualArticles_ToMap = tmFileStorage.loadVirtualArticles_FromDisk();
			foreach (var virtualArticle in virtualArticles_ToMap)
				virtualArticles.add(virtualArticle.Id, virtualArticle);
			return tmFileStorage;
		}
        public static List<VirtualArticleAction> loadVirtualArticles_FromDisk(this TM_FileStorage tmFileStorage)
		{            
			var virtualArticlesFile = tmFileStorage.getVirtualArticlesFile();
			if (virtualArticlesFile.fileExists())
				return virtualArticlesFile.load<List<VirtualArticleAction>>();
			return new List<VirtualArticleAction>();
		}
        public static string getVirtualArticlesFile(this TM_FileStorage tmFileStorage)
		{			
			return tmFileStorage.path_XmlLibraries().pathCombine("Virtual_Articles.xml");
		}
        public static TM_FileStorage saveVirtualArticles(this TM_FileStorage tmFileStorage)
		{
			var virtualArticlesFile = tmFileStorage.getVirtualArticlesFile();

			var virtualArticles = tmFileStorage.tmXmlDatabase().getVirtualArticles().Values.toList();
			virtualArticles.saveAs(virtualArticlesFile);
			return tmFileStorage;
		}
    }
}
