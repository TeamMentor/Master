using System;
using System.IO;
using System.Security.AccessControl;

namespace FluentSharp.CoreLib
{
    public static class Extra_IO_ExtensionMethods
    {

        //DirectoryInfo
        public static bool allow_Write_Users(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.allow_Write("Users");
        }
        public static bool allow_Write(this DirectoryInfo directoryInfo, string targetUser)
        {
            return directoryInfo.setAccessControl(targetUser, FileSystemRights.Write, AccessControlType.Allow);
        }
        public static bool deny_Write_Users(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.deny_Write("Users");
        }
        public static bool deny_CreateDirectories_Users(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.deny_CreateDirectories("Users");
        }
        public static bool deny_Write(this DirectoryInfo directoryInfo, string targetUser)
        {
            return directoryInfo.setAccessControl(targetUser, FileSystemRights.Write, AccessControlType.Deny);
        }
        public static bool deny_CreateDirectories(this DirectoryInfo directoryInfo, string targetUser)
        {
            return directoryInfo.setAccessControl(targetUser, FileSystemRights.CreateDirectories, AccessControlType.Deny);
        }
        public static bool setAccessControl(this DirectoryInfo directoryInfo, string targetUser, FileSystemRights fileSystemRights, AccessControlType accessControlType)
        {
            if (directoryInfo.notNull() && targetUser.notNull())
            {
                try
                {
                    var fileSystemAccessRule = new FileSystemAccessRule(targetUser, fileSystemRights, accessControlType);
                    var directorySecurity = new DirectorySecurity();
                    directorySecurity.AddAccessRule(fileSystemAccessRule);
                    directoryInfo.SetAccessControl(directorySecurity);
                    return true;
                }
                catch(Exception ex)
                {
                    ex.log();                    
                }
            }
            return false;
        }
        public static bool setAccessControl(this FileInfo fileInfo, string targetUser, FileSystemRights fileSystemRights, AccessControlType accessControlType)
        {
            if (fileInfo.notNull() && targetUser.notNull())
            {
                try
                {
                    var fileSystemAccessRule = new FileSystemAccessRule(targetUser, fileSystemRights, accessControlType);
                    var fileSecurity = new FileSecurity();
                    fileSecurity.AddAccessRule(fileSystemAccessRule);
                    fileInfo.SetAccessControl(fileSecurity);
                    return true;
                }
                catch (Exception ex)
                {
                    ex.log();
                }
            }
            return false;
        }
        //FileInfo
    }
}