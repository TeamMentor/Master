using System;
using FluentSharp.CoreLib;


namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_Library_Files
    {
        [Admin] 
        public static bool upload_File_to_Library(this TM_Xml_Database tmDatabase, Guid libraryId, string filename, byte[] fileContents)
        { 
            var targetLibrary = tmDatabase.tmLibrary(libraryId);
            if (targetLibrary.notNull())
            {                                 
                var targetFolder = tmDatabase.xmlDB_Path_Library_RootFolder(targetLibrary)
                                            .pathCombine("_Images")
                                            .createDir();
                var targetFile = targetFolder.pathCombine(filename.safeFileName());
                return fileContents.saveAs(targetFile).fileExists();

            }
            return false;
        }

        [ReadArticles] 
        public static string Get_Path_To_File(this TM_Xml_Database tmDatabase, string fileKey)
        {
            var filePath = TM_Xml_Database.Current.Path_XmlLibraries.pathCombine("_Images").pathCombine(fileKey);    
            if (filePath.fileExists())
                return filePath;

            var splitedFileKey = fileKey.split("/");
            if (splitedFileKey.size() == 2)
            { 
                var item = splitedFileKey[0].trim();
                var fileName = splitedFileKey[1].trim();
                if (item.isGuid())
                    return tmDatabase.Get_Path_To_File(item.guid(), fileName);                   
                return tmDatabase.Get_Path_To_File(item, fileName);
            }


            foreach (var library in tmDatabase.tmLibraries())
            { 
                filePath = tmDatabase.Get_Path_To_File(library.Caption, fileKey);
                if (filePath.notNull())
                    return filePath;
            }
            return null;
        }

        [ReadArticles] 
        public static string Get_Path_To_File(this TM_Xml_Database tmDatabase, Guid itemGuid, string fileName)
        { 
            var library = tmDatabase.tmLibrary(itemGuid);
            if (library.notNull())
                return tmDatabase.Get_Path_To_File(library.Caption,fileName);
            /*var article = tmDatabase.getGuidanceItem(itemGuid);
            if (article.notNull())
            { 
                var path = tmDatabase.getXmlFilePathForGuidanceId(itemGuid);
                if (path.fileExists())
                { 
                    var file
                }
            }*/
            return null;

        }

        public static string Get_Path_To_File(this TM_Xml_Database tmDatabase, string libraryName, string fileName)
        {
            var library = tmDatabase.tmLibrary(libraryName);
            if (library.notNull())
            {
                var filePath =tmDatabase.xmlDB_Path_Library_RootFolder(library).pathCombine("_Images").pathCombine(fileName);
                if (filePath.fileExists())
                    return filePath;
            }
            return null;
        }
    }
}