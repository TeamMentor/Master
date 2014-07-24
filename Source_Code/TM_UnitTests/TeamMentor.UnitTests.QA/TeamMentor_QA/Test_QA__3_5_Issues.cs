using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;
using TeamMentor.UserData;

namespace TeamMentor.UnitTests.QA.TeamMentor_QA
{
    [TestFixture]
    public class Test_QA__3_5_Issues 
    {
        /// <summary>
        /// https://github.com/TeamMentor/Master/issues/821
        /// </summary>
        [Test] public void Issue_821__Old_user_accounts_no_longer_work()
        {            
            TMConfig.Current    = new TMConfig();
            TM_UserData.Current = new TM_UserData();        // the fix is now here

            Action<Guid,string, string, string> checkHashValues = 
                (id, username, password, hash) =>
                    {
                         var tmUser = new TMUser { ID = id, UserName = username};

                         tmUser.createPasswordHash(password)
                               .assert_Is         (hash);
                    };
                      
            checkHashValues("8da99a4e-b15e-46d5-a732-f7b2543d4e0f".guid(),
                            "admin",
                            "!!tmadmin",
                            "OKrlvzFvi50y0JPZJJZJfKM4qDU3KFfDXgUIZysHz7Mh3jI8WwpWvnBFcRXJcWYhscZOHjIAyUHS8b1ruXP4Xg==");

            checkHashValues("d8aac161-0e25-426c-b21c-9cd230be7dba".guid(),
                            "admin",
                            "!!tmadmin",
                            "8jesPsP9ExGeoMe/NezXqh7RWQTdawsUb0znfo6VgD46nRIbAXbcgaPYCRlfLYQK1IeQphESxjZ5EDc/ZD0yFw==");
            
            checkHashValues("403b1277-6a78-42b7-aaa2-75a985de323a".guid(),
                            "admin",
                            "!!tmadmin",
                            "7bqGsnUsst2j/rKl6/EUg0SOLX4DpKdVdfrCjeihffP/wFKqIizWTCBpuwAO0m118fpatwrZ7RhvJPAc6PJYTA==");
        }

        [Test] public void Issue_826__No_lenght_constraint_on_User_Tags()
        {
            var userData = new TM_UserData();
            var newUser  = new NewUser().with_Random_Data();
            newUser.validate().asStringList().assert_Is_Empty();
            
            var userTag_Ok   = new UserTag { Key = 254.randomLetters(), Value = 254.randomLetters()};
            var userTag_Fail = new UserTag { Key = 256.randomLetters(), Value = 256.randomLetters()};

            userTag_Ok  .validate().assert_Empty();
            userTag_Fail.validate().assert_Not_Empty();

            newUser.UserTags.add(userTag_Ok);
            userData.createTmUser(newUser).assert_Is_Not(0);

            newUser.UserTags.add(userTag_Fail);
            userData.createTmUser(newUser).assert_Is(0);
        }
    }
}
