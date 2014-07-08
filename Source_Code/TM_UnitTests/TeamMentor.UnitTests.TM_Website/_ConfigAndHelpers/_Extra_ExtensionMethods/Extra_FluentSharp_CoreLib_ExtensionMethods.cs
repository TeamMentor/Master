using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

namespace FluentSharp.CoreLib
{
    public static class Extra_FluentSharp_CoreLib_ExtensionMethods
    {
        /// <summary>
        /// Inserts (ie adds to the beggining) 5 randoms letters to the provided target (separted by _ )
        ///
        /// If <code>target = "abc"</code> the return value will be <code>FujNe_abc</code> (with "FujNe" being the random letters)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string insert_5_RandomLetters(this string target)
        {
            return target.notNull() ? "{0}_{1}".format(5.randomLetters(), target) : null;
        }
        public static bool contains_Not(this string target, string value)
        {
            return target.contains(value).isFalse();
        }
        public static bool doesnt_Contains(this string target, string value)
        {
            return target.contains_Not(value);
        }
        public static bool folder_Delete_Files(this string folder)
        {
            if (folder.isFolder() && folder.files().notEmpty())
            {                
                Files.deleteAllFilesFromDir(folder);
                return folder.files().empty();
            }
            return false;
        }
        /// <summary>
        /// Creates a file in the provided folder
        /// 
        /// Returns path to created file.
        /// 
        /// retuns null if: 
        ///   - folder doesn't exist
        ///   - file already exists
        ///   - resolved file path is located outside the provided folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        public static string folder_Create_File(this string folder, string fileName, string fileContents)
        {   
            if(folder.folder_Exists())
            {
                var path = folder.mapPath(fileName);
                if (path.valid() && path.file_Doesnt_Exist())
                { 
                    fileContents.saveAs(path);
                    if (path.file_Exists())
                        return path;
                }
            }
            return null;
        }
        /// <summary>
        /// Waits for a folder to be deleted from disk. 
        /// 
        /// The maxWait defaults to 2000 and there will be 10 checks (at maxWait / 10 intervals).
        /// 
        /// If the folder was still there after maxWait an error message will be logged 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="maxWait"></param>
        /// <returns></returns>
        public static string folder_Wait_For_Deleted(this string folder, int maxWait = 2000)
        {
            if(folder.isFolder() && folder.folder_Exists())
            {
                var loopCount = 10;
                var sleepValue = maxWait/loopCount;
                for(int i=0;i< loopCount;i++)
                {
                    if (folder.folder_Not_Exists())
                        break;
                    sleepValue.sleep();
                }
                if (folder.folder_Exists())
                    "[string][folder_Wait_For_Deleted] after {0}ms the target folder still existed: {1}".error(maxWait, folder);
            }
            return folder;
        }
        /// <summary>
        /// Combines both paths and normalizes the file path.
        /// 
        /// There is a check that the rootPath is still in the final (it is not, an exception is logged and null is returned)
        /// 
        /// This simulates the System.Web MapPath functionality
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string mapPath(this string rootPath, string virtualPath)
        {
            if (rootPath.empty() || virtualPath.empty())
                return null;
            var mappedPath = rootPath.pathCombine(virtualPath).fullPath();
            if (mappedPath.starts(rootPath).isFalse())
            {
                @"[string][mapPath] the mappedPath did not contains the root path. 

                    mappedPath : {0}
                    rootPath   : {1}
                    virtualPath: {2}".error(mappedPath, rootPath, virtualPath);
                return null;
            }
            return mappedPath;
        }
   
        public static string temp_Folder(this string name, bool appendRandomStringToFolderName = true)
        {
            return name.temp_Dir(appendRandomStringToFolderName);
        }
        public static string temp_Dir(this string name, bool appendRandomStringToFolderName = true)
        {
            return name.tempDir(appendRandomStringToFolderName);
        }
    }
}