using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_Ex_Config
    {        
        public static bool copy_FilesIntoWebRoot(this TM_Xml_Database tmDatabase)
        {            
            var sourceFolder = tmDatabase.UserData.webRootFiles();
            if (sourceFolder.notValid())
                return false;
            var targetFolder =  tmDatabase.webRoot();  
            if (targetFolder.pathCombine("web.config").fileExists().isFalse())
            {
                "[copy_FilesIntoWebRoot] failed because web.config was not found on targetFolder: {0}".debug(targetFolder);
                return false;
            }
            if (sourceFolder.dirExists().isFalse())
            {
                //"[copy_FilesIntoWebRoot] skipped because targetFolder was not found: {0}".error(targetFolder);
                return false;
            }            
            Files.copyFolder(sourceFolder, targetFolder,true,true,"");            
            "[copy_FilesIntoWebRoot] from '{0}' to '{1}'".info(sourceFolder, targetFolder);
            return true;
        }
    }
}
