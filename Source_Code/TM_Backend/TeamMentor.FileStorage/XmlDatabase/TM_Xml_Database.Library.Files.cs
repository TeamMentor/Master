using System;
using FluentSharp.CoreLib;
using TeamMentor.FileStorage;


namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_Library_Files
    {
        [Admin] 
        public static bool upload_File_to_Library(this TM_FileStorage tmFileStorage, Guid libraryId, string filename, byte[] fileContents)
        { 
            UserRole.Admin.demand();
            var targetLibrary = tmFileStorage.tmXmlDatabase().tmLibrary(libraryId);
            if (targetLibrary.notNull())
            {                                 
                var targetFolder = tmFileStorage.xmlDB_Path_Library_RootFolder(targetLibrary)
                                                .pathCombine("_Images")
                                                .createDir();
                var targetFile = targetFolder.pathCombine(filename.safeFileName());
                return fileContents.saveAs(targetFile).fileExists();

            }
            return false;
        }

        [ReadArticles] 
        public static string Get_Path_To_File(this TM_FileStorage tmFileStorage, string fileKey)
        {
            UserRole.ReadArticles.demand();
            var filePath = tmFileStorage.path_XmlLibraries().pathCombine("_Images").pathCombine(fileKey);    
            if (filePath.fileExists())
                return filePath;

            var splitedFileKey = fileKey.split("/");
            if (splitedFileKey.size() == 2)
            { 
                var item = splitedFileKey[0].trim();
                var fileName = splitedFileKey[1].trim();
                if (item.isGuid())
                    return tmFileStorage.Get_Path_To_File(item.guid(), fileName);                   
                return tmFileStorage.Get_Path_To_File(item, fileName);
            }


            foreach (var library in tmFileStorage.tmXmlDatabase().tmLibraries())
            { 
                filePath = tmFileStorage.Get_Path_To_File(library.Caption, fileKey);
                if (filePath.notNull())
                    return filePath;
            }
            return null;
        }

        [ReadArticles] 
        public static string Get_Path_To_File(this TM_FileStorage tmFileStorage, Guid itemGuid, string fileName)
        { 
            UserRole.ReadArticles.demand();
            var library = tmFileStorage.tmXmlDatabase().tmLibrary(itemGuid);
            if (library.notNull())
                return tmFileStorage.Get_Path_To_File(library.Caption,fileName);            
            return null;

        }

        public static string Get_Path_To_File(this TM_FileStorage tmFileStorage, string libraryName, string fileName)
        {
            var library = tmFileStorage.tmXmlDatabase().tmLibrary(libraryName);
            if (library.notNull())
            {
                var filePath =tmFileStorage.xmlDB_Path_Library_RootFolder(library).pathCombine("_Images").pathCombine(fileName);
                if (filePath.fileExists())
                    return filePath;
            }
            return null;
        }
    }
}