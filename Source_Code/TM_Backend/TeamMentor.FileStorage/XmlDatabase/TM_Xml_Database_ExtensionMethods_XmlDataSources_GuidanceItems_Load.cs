using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XmlDataSources_GuidanceItems_Load
    {				
        public static TeamMentor_Article teamMentor_Article(this string pathToXmlFile)
        { 
            var article = pathToXmlFile.load<TeamMentor_Article>();//.htmlEncode(); 
            return article;
        }

        public static TeamMentor_Article setHashes(this TeamMentor_Article article)
        { 
            article.Metadata_Hash = article.Metadata.serialize(false).hash();
            article.Content_Hash  = article.Content.serialize(false).hash();
            return article;
        }

               
        public static List<TeamMentor_Article>  xmlDB_Load_GuidanceItemsV3(this TM_Xml_Database tmDatabase, Guid libraryId, List<string> guidanceItemsFullPaths)
        {
            var o2Timer = new O2Timer("xmlDB_GuidanceItems").start();
            var itemsLoaded = 0;
            //var maxToLoad = 1000;
            var guidanceItems = new List<TeamMentor_Article>();
            foreach(var fullPath in guidanceItemsFullPaths)
            { 
                var guidanceItemId = fullPath.fileName().remove(".xml");
                if (guidanceItemId.isGuid())
                {
                    var guidanceItem = tmDatabase.xmlDB_GuidanceItem(guidanceItemId.guid(),fullPath);
                    if (guidanceItem.notNull())
                    {
                        guidanceItems.add(guidanceItem);						    
                        guidanceItem.Metadata.Library_Id = libraryId;						
                    }
                    //if (maxToload-- < 1)
                    //	break;
                    if (itemsLoaded++ % 200 == 0)
                        "loaded {0} items".info(itemsLoaded);
                }
                //if (itemsLoaded > maxToLoad)
                //	break; 
                    
            }
            o2Timer.stop();
            return guidanceItems;
        }        
        public static TeamMentor_Article        xmlDB_GuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId)
        {
            return tmDatabase.xmlDB_GuidanceItem(guidanceItemId, null);
        }
        public static TeamMentor_Article        fixGuidanceItemFileDueToGuidConflict(this TM_Xml_Database tmDatabase, Guid original_Guid, string fullPath)
        {			
            var newGuid = Guid.NewGuid();
            var newPath = fullPath.replace(original_Guid.str(), newGuid.str());
            Files.moveFile(fullPath, newPath);
            "[xmlDB_GuidanceItem] resolved GuidanceItem ID conflict for  Id '{0}' was already mapped. \nExisting path: \t{1} \nNew path:  \t{2}".error(original_Guid, fullPath , newPath);
            var guidanceItemV3 = tmDatabase.xmlDB_GuidanceItem(newGuid, newPath);			
            return guidanceItemV3;
        }        
        //[EditArticles]
        public static TeamMentor_Article        xmlDB_GuidanceItem(this TM_Xml_Database tmDatabase, Guid guidanceItemId, string fullPath)
        {
            var tmFileStorage = TM_FileStorage.Current;
            try
            {
                if (TM_Xml_Database.Current.Cached_GuidanceItems.hasKey(guidanceItemId))
                {
                    //"found match for id: {0} in {1}".info(guidanceItemId, fullPath);
                    if (tmFileStorage.GuidanceItems_FileMappings[guidanceItemId] != fullPath)
                    {						
                        //"[xmlDB_GuidanceItem] GuidanceItem ID conflict, the Id '{0}' was already mapped. \nExisting path: \t{1} \nNew path:  \t{2}".error(
                        //	guidanceItemId, TM_Xml_Database.GuidanceItems_FileMappings[guidanceItemId] , fullPath);
                        return tmDatabase.fixGuidanceItemFileDueToGuidConflict(guidanceItemId, fullPath);
                    }
                    return TM_Xml_Database.Current.Cached_GuidanceItems[guidanceItemId]; 
                }
                
                if(fullPath.notNull())
                {
                    //"trying to load id {0} from virtualPath: {1}".info(guidanceItemId, virtualPath);				
                    //var pathXmlLibraries = TM_Xml_Database.Current.Path_XmlLibraries;
                    /*var fullPath = virtualPath.fileExists() 
                                        ? virtualPath
                                        : pathXmlLibraries.pathCombine(virtualPath).fullPath();*/
                    if (fullPath.fileExists())									
                    {
                        //"loading {0}".info(fullPath);

                        var article = fullPath.teamMentor_Article(); //.transform_into_guidanceItem();
                        if (article.isNull())
                        {
                            // _guidanceItem = guidanceItem.Load(fullPath).transform();
                            article = fullPath.load<Guidance_Item_Import>().transform();
                            article.saveAs(fullPath);   // to do an import in place
                        }  
                        if (article.notNull())
                        {
                            if(article.Metadata.Id != guidanceItemId)
                            {
                                "FOUND GUID CHANGE".error();
                                article.Metadata.Id_History += article.Metadata.Id.str() + ",";
                                article.Metadata.Id 		 = guidanceItemId;
                                article.saveAs(fullPath);								
                            }
                            //guidanceItemV3.guidanceItemId		 = original_Guid;		// this gives us the ability to track its source 
                            //guidanceItemV3.source_guidanceItemId = newGuid;				// also provides support for moving GuidanceItems across libraries
                            //var _guidanceItemV3 = _guidanceItem.tmGuidanceItemV3();
                            
                            TM_Xml_Database.Current.Cached_GuidanceItems.Add(guidanceItemId, article);
                            tmFileStorage.GuidanceItems_FileMappings.add(guidanceItemId, fullPath);
                            
                            
                            return article;
                        }
                        else
                            "[xmlDB_GuidanceItem] Failed to load article at path: {0} (see errors for reason)".error(fullPath);
                    }
                    else
                        "[xmlDB_GuidanceItem] could not find file: {0}".error(fullPath);
                }
                else
                    "no virtualPath provided for id: {0}".error(guidanceItemId);
            }
            catch(Exception ex)
            {
                "[TM_Xml_Database] in xmlDB_GuidanceItem: {0}".error(ex.Message);
            }
            return null;
        }								
    }
}