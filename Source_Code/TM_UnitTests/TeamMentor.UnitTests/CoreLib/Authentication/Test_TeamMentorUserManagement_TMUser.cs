using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Authentication
{
    [TestFixture]
    public class Test_TeamMentorUserManagement_TMUser
    {
        [Test]
        public void userGroup()
        {
            var tmUser = new TMUser();

            Action<UserGroup,int> checkMapping = 
                    (userGroup, id)=>{
                                        tmUser.GroupID = id;
                                        Assert.AreEqual(userGroup, tmUser.userGroup());
                                      };
            checkMapping(UserGroup.None     , -1);
            checkMapping(UserGroup.Viewer   ,  0);
            checkMapping(UserGroup.Admin    ,  1);
            checkMapping(UserGroup.Reader   ,  2);
            checkMapping(UserGroup.Editor   ,  3);            
        }
        [Test]
        public void userRoles()     
        {
            var tmUser = new TMUser();

            Action<UserGroup> checkMapping = 
                    (userGroup)=>{
                                    var expectedMapping = UserRolesMappings.Mappings[userGroup];                                        
                                    tmUser.GroupID = (int)userGroup;       
                                    Assert.AreEqual(expectedMapping, tmUser.userRoles());
                                 };
            checkMapping(UserGroup.None     );
            checkMapping(UserGroup.Viewer);
            checkMapping(UserGroup.Admin    );
            checkMapping(UserGroup.Reader   );
            checkMapping(UserGroup.Editor   );            
        }

        [Test]
        public void currentUserHasRole()
        {         
            // as Anonymous
            Assert.IsFalse(UserRole.Admin.currentUserHasRole());
            Assert.IsFalse(UserRole.EditArticles.currentUserHasRole());
            Assert.IsFalse(UserRole.ManageUsers.currentUserHasRole());
            Assert.IsFalse(UserRole.ReadArticles.currentUserHasRole());            
             
            //as Reader
            UserGroup.Reader.setThreadPrincipalWithRoles();              
            Assert.IsFalse(UserRole.Admin.currentUserHasRole());
            Assert.IsFalse(UserRole.EditArticles.currentUserHasRole());
            Assert.IsFalse(UserRole.ManageUsers.currentUserHasRole());
            Assert.IsTrue (UserRole.ReadArticles.currentUserHasRole());            

            //as Editor
            UserGroup.Editor.setThreadPrincipalWithRoles();  
            Assert.IsFalse(UserRole.Admin.currentUserHasRole());            
            Assert.IsFalse(UserRole.ManageUsers.currentUserHasRole());
            Assert.IsTrue(UserRole.EditArticles.currentUserHasRole());
            Assert.IsTrue(UserRole.ReadArticles.currentUserHasRole());            
            
            //as Admin
            UserGroup.Admin.setThreadPrincipalWithRoles();  
            Assert.IsTrue(UserRole.Admin.currentUserHasRole());            
            Assert.IsTrue(UserRole.ManageUsers.currentUserHasRole());
            Assert.IsTrue(UserRole.EditArticles.currentUserHasRole());
            Assert.IsTrue(UserRole.ReadArticles.currentUserHasRole());            

        }
    }
}
