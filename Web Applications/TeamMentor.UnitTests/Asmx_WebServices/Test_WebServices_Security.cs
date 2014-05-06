using System.Collections.Generic;
using System.Reflection;
using System.Security;
using NUnit.Framework;
using FluentSharp.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    public class Test_WebServices_Security : TM_WebServices_InMemory        
    {
        [Test]
        public void CheckThatObjectIsNotUsed_TMUser()
        {
            var methods = tmWebServices.type().methods();
            Assert.Less(100,methods.size());
            var returnTypeMappings = new Dictionary<string, List<MethodInfo>>();

            //Create a Mapping based on the return Type of all WebServices methods
            foreach (var method in methods)                
            {
                var returnType = method.ReturnType;
                if (returnType.IsGenericType)
                    foreach(var genericParameter in returnType.GetGenericArguments())
                        returnTypeMappings.add(genericParameter.Name, method); 
                returnTypeMappings.add(returnType.Name, method);
            }
            returnTypeMappings.Keys.toString().info();

            var typeNames = returnTypeMappings.Keys.toList();
            Assert.Less     (15, typeNames.size());

            Assert.IsTrue (typeNames.contains("TeamMentor_Article"));
            Assert.IsTrue (typeNames.contains("TM_Library"));
            Assert.IsTrue (typeNames.contains("Folder_V3"));
            Assert.IsTrue (typeNames.contains("View_V3"));
            Assert.IsTrue (typeNames.contains("Library"));
            Assert.IsTrue (typeNames.contains("Guid"));
            Assert.IsTrue (typeNames.contains("TM_User")  , "TM_User object should be used instead of TMUser");

            Assert.IsFalse(typeNames.contains("TMUser")   , "TMUser object must not be exposed on a WebService (since it contains all user info, including its passwordHash)");
            
            //returnTypeMappings["TMUser"].names().toString().info();            
        }

        [Test]
        public void TestSecurityDemandsOnWebServices()
        {
            Assert.Throws<SecurityException>(() => tmWebServices.XmlDatabase_ImportLibrary_fromZipFile(null, null));
            Assert.Throws<SecurityException>(() => tmWebServices.XmlDatabase_SetLibraryPath(null));
            Assert.Throws<SecurityException>(() => tmWebServices.XmlDatabase_ReloadData());
            Assert.Throws<SecurityException>(() => tmWebServices.XmlDatabase_GetLibraryPath());
            Assert.Throws<SecurityException>(() => tmWebServices.XmlDatabase_GetDatabasePath());                    
        }
    }
}
