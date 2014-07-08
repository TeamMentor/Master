using FluentSharp.CoreLib;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.CoreLib.Schemas
{
    [TestFixture]
    public class Test_WhoAmI : TM_UserData_InMemory
    {
        [Test] public void Test_WhoAmI_Ctor()
        {
            var newUser = userData.createUser();
            var whoAmI   = newUser.whoAmI();
            Check_WhoAmIObject_for_Reader(newUser, whoAmI);
        }

        [Test] public void whoAmI()
        {
            var newUser = userData.createUser();            
            var whoAmI  = newUser.whoAmI();
            Check_WhoAmIObject_for_Reader(newUser, whoAmI);

            var empty_WhoAmI = (null as TMUser).whoAmI();

            Assert.IsNotNull(empty_WhoAmI);

            Assert.AreEqual (empty_WhoAmI.UserName , "");
            Assert.AreEqual (empty_WhoAmI.UserId   , -1);
            Assert.AreEqual (empty_WhoAmI.GroupId  , 0);
            Assert.AreEqual (empty_WhoAmI.GroupName, "Anonymous");
            Assert.AreEqual (empty_WhoAmI.UserRoles, "");
            Assert.AreEqual (empty_WhoAmI.toXml(), new WhoAmI().toXml());
        }

        //Workflows
        [Test] public void JSON_serialization_of_WhoAmI()
        {
            var newUser         = userData.createUser();            
            var whoAmI_Obj      = newUser.whoAmI();
            var whoAmI_Json     = whoAmI_Obj.json();
            var whoAmI_Json_Obj = whoAmI_Json.json_Deserialize<WhoAmI>();
            Assert.IsNotNull  (whoAmI_Obj);
            Assert.IsNotNull  (whoAmI_Json);
            Assert.AreNotEqual(whoAmI_Json, "");
            Assert.IsNotNull  (whoAmI_Json_Obj);
            Assert.AreEqual   (whoAmI_Obj.toXml()   , whoAmI_Json_Obj.toXml());
            Assert.AreEqual   (whoAmI_Obj.UserName  , whoAmI_Json_Obj.UserName);
            Assert.AreEqual   (whoAmI_Obj.UserId    , whoAmI_Json_Obj.UserId);
            Assert.AreEqual   (whoAmI_Obj.GroupId   , whoAmI_Json_Obj.GroupId);
            Assert.AreEqual   (whoAmI_Obj.GroupName , whoAmI_Json_Obj.GroupName);
        }

        // Util Method
        public void Check_WhoAmIObject_for_Reader(TMUser tmUser, WhoAmI whoAmI)
        {            
            Assert.IsNotNull(whoAmI);
            Assert.IsNotNull(whoAmI.UserName);
            Assert.Greater  (whoAmI.UserId , 0);
            Assert.Greater  (whoAmI.GroupId, 0);
            Assert.IsNotNull(whoAmI.GroupName);
            Assert.IsNotNull(whoAmI.UserRoles);

            Assert.AreEqual (whoAmI.UserName , tmUser.UserName);
            Assert.AreEqual (whoAmI.UserId   , tmUser.UserID);
            Assert.AreEqual (whoAmI.GroupId  , tmUser.GroupID);
            Assert.AreEqual (whoAmI.GroupName, "Reader");
            Assert.AreEqual (whoAmI.UserRoles, "ReadArticlesTitles , ReadArticles");
        }

        
    }
}
