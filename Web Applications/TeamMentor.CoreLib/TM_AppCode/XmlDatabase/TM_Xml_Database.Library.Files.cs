using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using urn.microsoft.guidanceexplorer;
using urn.microsoft.guidanceexplorer.guidanceItem;
using Microsoft.Security.Application;
//O2File:TM_Xml_Database.cs
//O2File:../O2_Scripts_APIs/_O2_Scripts_Files.cs

namespace SecurityInnovation.TeamMentor.WebClient.WebServices
{
    public static class TM_Xml_Database_ExtensionMethods_Library_Files
    {
        [PrincipalPermission(SecurityAction.Demand, Role = "Admin")] 
        public static bool upload_File_to_Library(this TM_Xml_Database tmDatabase, Guid libraryId, string filename, byte[] fileContents)
        { 
            var targetLibrary = tmDatabase.tmLibrary(libraryId);
            if (targetLibrary.notNull())
            {                                 
                var targetFolder = tmDatabase.xmlDB_LibraryPath_GuidanceItems(targetLibrary.Caption)
                                            .pathCombine("_Images")
                                            .createDir();
                var targetFile = targetFolder.pathCombine(filename.safeFileName());
                return fileContents.saveAs(targetFile).fileExists();

            }
            return false;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
        public static string Get_Path_To_File(this TM_Xml_Database tmDatabase, string fileKey)
        { 
            var filePath = TM_Xml_Database.Path_XmlLibraries.pathCombine("_Images").pathCombine(fileKey);    
            if (filePath.fileExists())
                return filePath;

            var splitedFileKey = fileKey.split("/");
            if (splitedFileKey.size() == 2)
            { 
                var item = splitedFileKey[0].trim();
                var fileName = splitedFileKey[1].trim();
                if (item.isGuid())
                    return tmDatabase.Get_Path_To_File(item.guid(), fileName);   
                else
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

        [PrincipalPermission(SecurityAction.Demand, Role = "ReadArticles")] 
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
            var filePath = tmDatabase.xmlDB_LibraryPath_GuidanceItems(libraryName).pathCombine("_Images").pathCombine(fileName);            
            if (filePath.fileExists())
                return filePath;
            return null;
        }
    }
}