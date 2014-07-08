using System.Collections;
using System.Net;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;

namespace FluentSharp.NUnit
{
    public static class Extra_FluentSharp_NUnit_ExtensionMethods
    {
        public static T              assert_Is<T>(this T source, T target)
        {
            return source.assert_Is_Equal_To(target);
        }
        public static bool           assert_True(this bool value, string message)
        {
            return value.assert_Is_True(message);
        }
        public static HttpStatusCode assert_Http_OK(this HttpStatusCode statusCode)
        {
            return statusCode.assert_HttpStatusCode(HttpStatusCode.OK);
            
        }
        public static HttpStatusCode assert_Http_NotFound(this HttpStatusCode statusCode)
        {
            return statusCode.assert_HttpStatusCode(HttpStatusCode.NotFound);            
        }
        public static HttpStatusCode assert_HttpStatusCode(this HttpStatusCode statusCode, HttpStatusCode expectedStatus)
        {
            statusCode.assert_Equal_To(expectedStatus);
            return statusCode;
        }
        /// <summary>
        /// Checks that a folder exists and has at least 1 files
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string assert_Folder_Not_Empty(this string folder)
        {
            folder.assert_Folder_Exists();
            var files = folder.files();
            files.assert_Not_Empty("on Folder {0} there where no files".format(folder));
            return folder;
        }
        /// <summary>
        /// Checks that a folder exists and has no files
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string assert_Folder_Empty(this string folder)
        {
            folder.assert_Folder_Exists();
            var files = folder.files();
            files.assert_Empty("on Folder {0} there where {1} files".format(folder, files.size()));
            return folder;
        }

        public static string assert_Folder_Has_Files(this string folder, params string[] files)
        {
            foreach(var file in files)
                folder.assert_Folder_Has_File(file);
            return folder;
        }
        public static string assert_Folder_Has_File(this string folder, string file)
        {
            folder.assert_Folder_Exists();
            folder.mapPath(file).assert_File_Exists();
            return folder;
        }

        public static T    assert_Empty<T>(this T target, string message) where  T : IEnumerable
        {
            return new NUnitTests().assert_Is_Empty(target, message);
        }
        public static T    assert_Not_Empty<T>(this T target, string message) where  T : IEnumerable
        {
            return new NUnitTests().assert_Is_Not_Empty(target, message);
        }
    }
    public static class Extra_FluentSharp_NUnit_ExtensionMethods_NUnitTests
    {
        public static T  assert_Is_Empty<T>(this NUnitTests nUnitTests, T target, string message) where  T : IEnumerable
        {
            Assert.IsEmpty(target, message);            
            return target;
        }        
        public static T  assert_Is_Not_Empty<T>(this NUnitTests nUnitTests, T target, string message) where  T : IEnumerable
        {
            Assert.IsNotEmpty(target, message);            
            return target;
        }
    }
}