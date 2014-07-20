using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace FluentSharp.NUnit
{
    public static class Extra_IO_ExtensionMethods
    {
        /// <summary>
        /// Returns a unique list of all files inside the list of folders provided
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static List<string> files(this List<string> folders, bool recursive = false)
        {
            if (folders.isNull())
                return new List<string>();

            var allFiles = new List<string>();
            foreach(var folder in folders)
                if(folder.isFolder())
                    allFiles.add(folder.files(recursive));
            return allFiles.unique();
        }
    }
    public static class Extra_Thread_ExtensionMethods
    {
        public static Thread join(this Thread thread, int maxWait_Miliseconds = 2000)
        {
            if(thread.notNull())
            try
            {
                thread.Join(maxWait_Miliseconds);
            }
            catch(Exception ex)
            {
                ex.log("[Thread][join]");
            }
            return thread;
        }
    }
    public class Extra_NUnit_Messages
    {
        public const string ASSERT_FALSE                 = "Callback return true (on target: {0})";
        public const string ASSERT_FILE_PARENT_FOLDER_IS = "The parent folder of the the file '{0}' was not the expected value of '{1}'";
        public const string ASSERT_PARENT_FOLDER_IS      = "The parent folder of the the folder '{0}' was not the expected value of '{1}'";        
        public const string ASSERT_FOLDER_NOT_EXISTS     = "The folder was not supposed to exist: '{0}'";
        public const string ASSERT_STARTS                = "that provided string '{0}' did not start with '{1}'";
        public const string ASSERT_FOLDER_FILE_COUNT_IS  = "the folder '{0}' was expected to have {1} files, but it had {2}"; 
        public const string ASSERT_NOT_NULL              = "the value provided was null";
        public const string ASSERT_NULL                  = "the value provided was expected to be null, but it was {0}";
    }
    public static class Extra_NUnit_ExtensionMethods
    {
        public static T assert_False<T>(this T target, Func<T, bool> callback, string message = Extra_NUnit_Messages.ASSERT_FALSE)
        {
            Assert.False(callback(target), message.format(target));
            return target;
        }
        public static T assert_False<T>(this T target, Func<bool> callback, string message = Extra_NUnit_Messages.ASSERT_FALSE)
        {
            Assert.False(callback(), message.format(target));
            return target;
        }

        public static string assert_Parent_Folder_Is(this string folderPath, string folder, string message = Extra_NUnit_Messages.ASSERT_PARENT_FOLDER_IS)
        {
            folderPath.parentFolder().assert_Equal(folder, message);
            return folderPath;
        }
        public static string assert_File_Parent_Folder_Is(this string filePath, string folder, string message = Extra_NUnit_Messages.ASSERT_FILE_PARENT_FOLDER_IS)
        {
            filePath.parentFolder().assert_Equal(folder, message);
            return filePath;
        }

        public static string assert_Is_Folder(this string folderPath)
        {
            return folderPath.assert_Folder_Exists();
        }
        public static string assert_Folder_File_Count_Is(this string folderPath, int value, string message = Extra_NUnit_Messages.ASSERT_FOLDER_FILE_COUNT_IS)
        {
            var fileCount = folderPath.assert_Is_Folder().files().size();
            fileCount.assert_Is(value, message.format(folderPath , value, fileCount));
            return folderPath;
        }

        public static T assert_Equal<T>(this T target, T value, string message = NUnit_Messages.ASSERT_ARE_EQUAL)
        {
            Assert.AreEqual(target, value, message);
            return target;
        }

        public static T assert_Not_Null<T>(this T target, string message = Extra_NUnit_Messages.ASSERT_NOT_NULL) where T : class
        {
            Assert.True(target.notNull(), message);
            return target;
        }
        public static T assert_Null<T>(this T target, string message = Extra_NUnit_Messages.ASSERT_NULL) where T : class
        {
            Assert.True(target.isNull(), message);
            return target;
        }
        public static T assert_Not_Equal<T>(this T target, T value, string message = NUnit_Messages.ASSERT_ARE_NOT_EQUAL)
        {
            return target.assert_Not_Equals(value);            
        }
        public static T assert_Not_Equal_To<T>(this T target, T value, string message = NUnit_Messages.ASSERT_ARE_NOT_EQUAL)
        {
            return target.assert_Not_Equals(value);            
        }

        public static string assert_Folder_Doesnt_Exist(this string folder, string message = Extra_NUnit_Messages.ASSERT_FOLDER_NOT_EXISTS)
        {
            return new NUnitTests().assert_Dir_Not_Exists(folder, message.format(folder));
        }

        public static string assert_Starts (this string target, string value, string message = Extra_NUnit_Messages.ASSERT_STARTS)
        {
            target.starts(value).assert_True(message.format(target, value));
            return target;
        }
    }

    public static class Extra_NUnitTests_Methods
    {
        public static string assert_Dir_Not_Exists(this NUnitTests nunitTests, string folderPath, string message = Extra_NUnit_Messages.ASSERT_FOLDER_NOT_EXISTS)
        {                        
            Assert.IsFalse(folderPath.dirExists(), message);
            return folderPath;
        }
    }
}
