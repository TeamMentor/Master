using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public static class TM_Proxy_ExtensionMethods_TM_Content
    {
        //articles
        public static List<TeamMentor_Article>  articles(this TM_Proxy tmProxy)
        {
            if(tmProxy.TmXmlDatabase.notNull())
                return tmProxy.TmXmlDatabase.xmlDB_GuidanceItems();
            return new List<TeamMentor_Article>();
        }
        public static TeamMentor_Article        article_New(this TM_Proxy tmProxy)
        {
            return tmProxy.invoke_Static<TeamMentor_Article>(typeof(TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItem),"xmlDB_RandomGuidanceItem", tmProxy.TmXmlDatabase);
        }
        public static TeamMentor_Article        article_New(this TM_Proxy tmProxy, Guid libraryId)
        {
            return tmProxy.invoke_Static<TeamMentor_Article>(typeof(TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItem),"xmlDB_RandomGuidanceItem", tmProxy.TmXmlDatabase, libraryId);
        }

        //Libraries
        public static List<TM_Library>          libraries(this TM_Proxy tmProxy)
        {
            if(tmProxy.TmXmlDatabase.notNull())
                return tmProxy.TmXmlDatabase.tmLibraries();
            return new List<TM_Library>();
        }
        public static TM_Library                library_New(this TM_Proxy tmProxy)
        {
            return tmProxy.invoke_Static<TM_Library>(typeof(TM_Xml_Database_ExtensionMethods_XML_DataSources_TM_Library),"new_TmLibrary", tmProxy.TmXmlDatabase);
        }
        public static TeamMentor_Article        library_New_Article_New(this TM_Proxy tmProxy)
        {
            var library = tmProxy.library_New();
            return tmProxy.article_New(library.Id);
        }

        public static bool library_Install_From_Zip(this TM_Proxy tmProxy, string zipFile)
	    {
		    return tmProxy.invoke_Static<bool>(typeof(FileStorage_Libraries), 
											   "xmlDB_Libraries_ImportFromZip", 
											   tmProxy.TmFileStorage,
											   zipFile, "");
	    }
            
       public static bool library_Install_Lib_Docs(this TM_Proxy tmProxy)
       {           
		    var zipFile = tmProxy.TmFileStorage.downloadLibraryIntoTempFolder("Lib_Docs.zip", "https://github.com/TMContent/Lib_Docs/archive/master.zip");
		    return tmProxy.library_Install_From_Zip(zipFile);
	   }	
       public static bool library_Install_Lib_OWASP(this TM_Proxy tmProxy)
       {           
		    var zipFile = tmProxy.TmFileStorage.downloadLibraryIntoTempFolder("Lib_OWASP.zip", "https://github.com/TMContent/Lib_OWASP/archive/master.zip");
		    return tmProxy.library_Install_From_Zip(zipFile);
	   }	
        public static bool library_Install_Lib_Vulnerabilities(this TM_Proxy tmProxy)
       {           
		    var zipFile = tmProxy.TmFileStorage.downloadLibraryIntoTempFolder("Lib_Vulnerabilities.zip", "https://github.com/TMContent/Lib_Vulnerabilities/archive/master.zip");
		    return tmProxy.library_Install_From_Zip(zipFile);
	   }
       public static bool  library_Delete(this  TM_Proxy tmProxy, string libraryName)
	    {
		    var library = tmProxy.TmXmlDatabase.tmLibrary(libraryName);
		    if (library.isNull())
			    return false;
		    return tmProxy.invoke_Static<bool>(typeof(TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceExplorer), 
											            "xmlDB_DeleteGuidanceExplorer", 
											            tmProxy.TmXmlDatabase, 
											            library.Id);
	    }

        //Server side caches                
        public static TM_Proxy  gui_ResetCache(this TM_Proxy tmProxy)
        {
            tmProxy.invoke_Instance(typeof(TM_WebServices),"resetCache");
            return tmProxy;
        }
        public static string                    cache_Reload__Data(this TM_Proxy tmProxy)
        {
            tmProxy.gui_ResetCache();
            return tmProxy.invoke_Static<string>(typeof(TM_Xml_Database_FileStorage),"reloadData", tmProxy.TmFileStorage);
        }
    }
}