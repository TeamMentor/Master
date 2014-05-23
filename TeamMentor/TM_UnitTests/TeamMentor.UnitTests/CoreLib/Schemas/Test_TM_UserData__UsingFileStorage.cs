using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using NUnit.Framework;
using System;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_UserData__UsingFileStorage
    {
        TM_UserData userData;
        [SetUp]    public void setup()
        {
            userData = new TM_UserData(true);
            userData.Path_UserData = "tempUser_Data".tempDir();
            Assert.IsNotNull(userData.Path_UserData);
            Assert.IsNotNull(userData.tmConfig_Location());
            Assert.IsNotNull(userData.secretData_Location());
        }
        [TearDown] public void teardown()
        {
            Assert.IsTrue (Files.deleteFolder(userData.Path_UserData, true));
            Assert.IsFalse(userData.Path_UserData.dirExists());
        }
        [Test] public void load()
        {
            
        }
        [Test] public void loadUsers__UsingFileStorage()
        {            
            userData.loadUsers();                   // when no user files exist
            Assert.IsEmpty(userData.TMUsers);       // we should get no users
            
            var tmUser1 = new TMUser { UserName = 10.randomLetters() };                           // without an ID this one will fail to load
            var tmUser2 = new TMUser { UserName = 10.randomLetters() , UserID = 10.randomNumber() };
            var tmUser3 = new TMUser { UserName = 10.randomLetters() , UserID = 10.randomNumber() };
            
            Assert.IsTrue(tmUser1.saveTmUser());            
            Assert.IsTrue(tmUser2.saveTmUser());
            Assert.IsTrue(tmUser3.saveTmUser());
            
            userData.loadUsers();                         // now
            Assert.IsNotEmpty(userData.TMUsers);          // we should get two users
            Assert.AreEqual(userData.TMUsers.size(), 2);  // which are the ones with a valid UserID 
            
        }
        [Test] public void users_XmlFile_Location()     
        {
            var location = userData.users_XmlFile_Location();
            var expectedLocation = userData.Path_UserData.pathCombine(TMConsts.USERDATA_PATH_USER_XML_FILES);

            Assert.NotNull (userData.Path_UserData);
            Assert.NotNull (location);
            Assert.AreEqual(location, expectedLocation);
            Assert.IsTrue  (location.dirExists());
        }
        [Test] public void user_XmlFile_Location()
        {
            var userName = 10.randomLetters();
            var id = Guid.NewGuid();
            var tmUser = new TMUser { UserName = userName, ID = id };
            var expectedLocation = userData.Path_UserData
                                           .pathCombine(TMConsts.USERDATA_PATH_USER_XML_FILES)
                                           .pathCombine(TMConsts.USERDATA_FORMAT_USER_XML_FILE.format(userName, id));
            var location1        = tmUser.user_XmlFile_Location();          // calling it twice should produce the same value
            var location2        = tmUser.user_XmlFile_Location();

            Assert.AreEqual(location1, location2);
            Assert.AreEqual(location1, expectedLocation);
        }
        [Test] public void user_XmlFile_Name()
        {
            // check with username with size 10 or less (so that there will be not subString or safeFileName applied) 
            var userName         = 10.randomLetters();   
            var id               =  Guid.NewGuid();
            var tmUser           = new TMUser { UserName = userName, ID = id};

            var expectedFileName = TMConsts.USERDATA_FORMAT_USER_XML_FILE.format(userName, id);
            var fileName1        = tmUser.user_XmlFile_Name();
            var fileName2        = tmUser.user_XmlFile_Name();          // calling it twice should produce the same value

            Assert.AreEqual(fileName1, fileName2);
            Assert.AreEqual(fileName1, expectedFileName);

            // check when username has weird chars
            var weirdName        = ":@^%" + 20.randomChars();
            var normalizedName   = weirdName.subString(0, 10).safeFileName();
            Assert.IsFalse(new TMUser { UserName = weirdName, ID = id }.user_XmlFile_Name().contains(weirdName));
            Assert.IsTrue (new TMUser { UserName = weirdName, ID = id }.user_XmlFile_Name().contains(normalizedName));

            //Check nulls and empty values (note that the last two work)
            Assert.IsNull   (new TMUser ()                                        .user_XmlFile_Name());
            Assert.IsNull   (new TMUser { UserName = ""                          }.user_XmlFile_Name());
            Assert.IsNull   (new TMUser { UserName = null                        }.user_XmlFile_Name());            
            Assert.IsNull   (new TMUser { UserName = "ABC" , ID = Guid.Empty     }.user_XmlFile_Name());
            Assert.IsNull   (new TMUser { UserName = ""    , ID = Guid.NewGuid() }.user_XmlFile_Name());
            Assert.IsNull   (new TMUser { UserName = null  , ID = Guid.NewGuid() }.user_XmlFile_Name());
            Assert.IsNotNull(new TMUser { UserName = "ABC" , ID = Guid.NewGuid() }.user_XmlFile_Name());
            Assert.IsNotNull(new TMUser { UserName = "ABC"                       }.user_XmlFile_Name());
            
        }
    }
}