using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.FileStorage
{
    [TestFixture]
    public class Test_TM_FileStorage
    {
        [Test]
        public void TM_FileStorage_Ctor_False()
        {
            var tmFileStorage = new TM_FileStorage(loadData : false); 
            Assert.NotNull(tmFileStorage.Server);
            Assert.NotNull(tmFileStorage.TMXmlDatabase);
            Assert.NotNull(tmFileStorage.UserData);
            Assert.IsEmpty(tmFileStorage.GuidanceExplorers_Paths);
            Assert.IsEmpty(tmFileStorage.GuidanceItems_FileMappings);
            Assert.IsNull (tmFileStorage.WebRoot);
            Assert.IsNull (tmFileStorage.Path_XmlDatabase);
            Assert.IsNull (tmFileStorage.Path_UserData);
            Assert.IsNull (tmFileStorage.Path_XmlLibraries);
        }

        [Test]
        public void TM_FileStorage_Ctor_True()
        {
            var tmFileStorage = new TM_FileStorage(loadData : true); 
            Assert.NotNull(tmFileStorage.Server);
            Assert.NotNull(tmFileStorage.TMXmlDatabase);
            Assert.NotNull(tmFileStorage.UserData);
            Assert.IsEmpty(tmFileStorage.GuidanceExplorers_Paths);
            Assert.IsEmpty(tmFileStorage.GuidanceItems_FileMappings);
            Assert.NotNull(tmFileStorage.WebRoot);
            Assert.NotNull(tmFileStorage.Path_XmlDatabase);
            Assert.NotNull(tmFileStorage.Path_UserData);
            Assert.NotNull(tmFileStorage.Path_XmlLibraries);
        }
    }
}
