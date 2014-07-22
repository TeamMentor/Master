using System;
using FluentSharp.CoreLib;

namespace TeamMentor.WatiN.NUnit
{
    public static class Extra_FluentSharp_ExtensionMethods
    {
        public static string folder_Create_Folder(this string targetFolder, string subFolderName)
        {
            if(targetFolder.folderExists())
                return targetFolder.mapPath(subFolderName).createDir();
            return null;
        }
        public static string append_FileName_To(this string fullPath, string target)
        {
            return target.append(fullPath.fileName());
        }
        public static Uri append_FileName_To(this string fullPath, Uri uri)
        {
            return uri.append(fullPath.fileName());
        }
        public static string append_To(this string value, string target)
        {
            return target.append(value);
        }
        public static Uri append_To(this string value, Uri target)
        {
            return target.append(value);
        }
    }
}