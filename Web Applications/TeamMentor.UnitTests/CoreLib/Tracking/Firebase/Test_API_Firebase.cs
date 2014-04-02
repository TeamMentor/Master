using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Tracking.Firebase
{
    [TestFixture]
    public class Test_API_Firebase : TM_UserData_InMemory
    {
        public API_Firebase     firebase;
        public TM_QA_Config     tmQAConfig;

        public Test_API_Firebase()              
        {         
            if (Tests_Consts.offline)
                Assert.Ignore("Ignoring Test because we are offline");   

            tmQAConfig = TM_QA_Config.Current;
            if (tmQAConfig.isNull())
                Assert.Ignore("TM_QA_ConfigFile.Current was null (so no Firebase live config values");
            
            firebase = new API_Firebase("QA_Tests");

            userData.SecretData.Firebase_Site      = tmQAConfig.Firebase_Site;
            userData.SecretData.Firebase_AuthToken = tmQAConfig.Firebase_AuthToken;                                    
        }        

        [Test] public void  site_Uri()          
        {
            var uri        = firebase.site_Uri();            

            Assert.IsNotNull (uri);            
            Assert.IsTrue    (uri.str().contains(firebase.Area));
            Assert.IsTrue    (uri.str().contains(tmQAConfig.Firebase_Site));
            Assert.IsTrue    (uri.str().contains(tmQAConfig.Firebase_AuthToken));
            
            var randomArea = 10.randomLetters();
            var randomUri  = firebase.site_Uri(randomArea);            

            Assert.IsNotNull (randomUri);
            Assert.IsTrue    (randomUri.str().contains(randomArea));
            Assert.IsTrue    (randomUri.str().contains(tmQAConfig.Firebase_Site));
            Assert.IsTrue    (randomUri.str().contains(tmQAConfig.Firebase_AuthToken));            
        }        
        [Test] public void  site_Configured()   
        {
            Assert.IsTrue(firebase.site_Configured());
        }
        [Test] public void  site_Online()       
        {
            Assert.IsTrue(firebase.site_Online());
        }

        [Test] public void GET()
        {
            //Web.Https.ignoreServerSslErrors();
            var getData     = firebase.GET();
            Assert.AreEqual(getData,"null");
        }
        [Test] public void PUT()
        {
            var data        = 10.randomString();
            var putResponse = firebase.PUT(data).json_Deserialize<string>();
            
            Assert.NotNull (putResponse);
            Assert.AreEqual(putResponse, data);

            var getResponse = firebase.GET().json_Deserialize<string>();            
                Assert.AreEqual(getResponse, data);
            Assert.AreEqual(getResponse, putResponse);


            Assert.AreEqual(firebase.PUT(null), "");                    // will not change the data
            Assert.AreEqual(firebase.GET().json_Deserialize<string>(), getResponse);
            firebase.DELETE();
        }

        [Test] public void DELETE()
        {
            firebase.DELETE();
            var data        = 10.randomLetters();
            Assert.AreEqual(firebase.GET()    , "null");
            Assert.AreEqual(firebase.PUT(null),"");
            Assert.AreEqual(firebase.PUT(data),data.json());
            Assert.AreEqual(firebase.GET()   , data.json());
            firebase.DELETE();
            Assert.AreEqual(firebase.GET()   , "null");
        }

        [Test] public void POST()
        {
            var type = 10.randomLetters();
            var text = 10.randomLetters();
            var logItem = new Log_Item(type, text);
            var responseJson = firebase.POST(logItem);
            var response = responseJson.json_Deserialize<API_Firebase.PostResponse>();
            Assert.IsNotNull(response);
            Assert.IsTrue   (response.name.valid());
            2000.sleep();
            firebase.DELETE();
            //Assert.IsTrue(response.contains("{\"name\""));
        }
    }
}
