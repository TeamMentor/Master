using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;

namespace TeamMentor.UnitTests.Cassini
{   
    public class Extra_NUnit_Messages
    {
        public const string ASSERT_DIR_EXISTS = "The expected directory did not exist: {0}";
    }
    public static class Extra_FluentSharp_NUnit_ExtensionMethods
    {
        public static NUnitTests nunitTests = new NUnitTests();
        public static string assert_Folder_Exists(this string folderPath, string message = Extra_NUnit_Messages.ASSERT_DIR_EXISTS)
        {
            return nunitTests.assert_Dir_Exists(folderPath, message);
        }
    }
    public static class Extra_FluentSharp_NUnit_ExtensionMethods_NUnitTests
    {
        //replace existing assert_Dir_Exists method with the one below
        public static string assert_Dir_Exists(this NUnitTests nUnitTest, string folderPath, string message = Extra_NUnit_Messages.ASSERT_DIR_EXISTS)
        {                        
            Assert.IsTrue(folderPath.dirExists(), message.format(folderPath));
            return folderPath;
        }
    }
}
