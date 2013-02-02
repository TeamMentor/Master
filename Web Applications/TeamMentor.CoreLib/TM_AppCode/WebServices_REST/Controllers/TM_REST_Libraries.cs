using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib
{
    public partial class TM_REST
    {
        public List<Library_V3>     Libraries()
        {
            return TmWebServices.GetLibraries().librariesV3();
        }
        public Library_V3           Library(string nameOrId)
        {
            var library = (nameOrId.isGuid())
                              ? TmWebServices.GetLibraryById(nameOrId.guid()).libraryV3()
                              : TmWebServices.GetLibraryByName(nameOrId).libraryV3();
            return (library.notNull())
                       ? TmWebServices.GetFolderStructure_Library(library.libraryId)
                       : null;
        }
        public List<Folder_V3>      Folders(string libraryId)
        {
            return TmWebServices.GetFolders(libraryId.guid());
        }
        public View_V3              View(string viewId)
        {
            return TmWebServices.GetViewById(viewId);
        }        
        public string               Article(string articleId)
        {
            var article = TmWebServices.GetGuidanceItemById(articleId);
            return article.serialize(false);
            //			return article;		// this was failing
        }
        public string               Article_Html(string articleId)
        {
            return TmWebServices.GetGuidanceItemHtml(articleId.guid());
        }
    }
}