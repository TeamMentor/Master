using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.Web;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib 
{
    [TestFixture]
    public class Test_API_Firebase_SubmitQueue : TM_UserData_InMemory
    {
        public API_Firebase firebase;

        [SetUp] public void setup()             
        {
            firebase = new API_Firebase
                {
                        SubmitThread = null, 
                        QueueMaxWait = 1,
                        Offline      = false
                };
        }
        [Test] public void API_Firebase_Ctor()  
        {
            firebase = new API_Firebase();
            Assert.IsNotNull(firebase);            
            Assert.IsNotNull(firebase.Area);            
            Assert.IsNotNull(firebase.MessageFormat);
            Assert.IsNotNull(firebase.SubmitQueue);
            Assert.IsNotNull(firebase.OfflineQueue);
            Assert.AreEqual (firebase.QueueMaxWait, TMConsts.FIREBASE_SUBMIT_QUEUE_MAX_WAIT);            
        }
        [Test] public void SubmitData_Ctor()    
        {            
            var data = new List<object> { 10.randomLetters(),  10.randomLetters(), 10000.random()};
            var type = API_Firebase.Submit_Type.SET;

            var submitData1 = new API_Firebase.SubmitData(); 
            var submitData2 = new API_Firebase.SubmitData(data, type); 
            
            Assert.AreEqual(submitData1.Data     , null);
            Assert.AreEqual(submitData1.Json_Data, null);
            Assert.AreEqual(submitData1.Type     , API_Firebase.Submit_Type.ADD);

            Assert.AreEqual(submitData2.Data     , data);
            Assert.AreEqual(submitData2.Json_Data, data.json());
            Assert.AreEqual(submitData2.Type     , type);
        }
        
        // API_Firebase methods
        [Test] public void firebase_Site()      
        {
            var firebaseConfig = userData.SecretData.FirebaseConfig;

            Assert.AreEqual(firebaseConfig.AuthToken   , "");
            Assert.AreEqual(firebaseConfig.Site        , "");            
            Assert.AreEqual(firebase.firebase_AuthToken(), "");            
            Assert.AreEqual(firebase.firebase_Site()     , "");
            Assert.IsTrue  (firebaseConfig.Log_Activities);            
            Assert.IsTrue  (firebaseConfig.Log_DebugMsgs);
            Assert.IsFalse (firebaseConfig.Log_RequestUrls);

            var site      = 10.randomLetters();
            var authToken = 10.randomLetters();

            firebaseConfig.Site      = site;
            firebaseConfig.AuthToken = authToken;

            Assert.AreEqual(firebaseConfig.AuthToken     , authToken);
            Assert.AreEqual(firebaseConfig.Site          , site);
            Assert.AreEqual(firebase.firebase_AuthToken(), authToken);            
            Assert.AreEqual(firebase.firebase_Site()     , site);

            //test nulls
            var _tmUserdata     = TM_UserData.Current;
            TM_UserData.Current = null;
            Assert.IsNull(firebase.firebase_AuthToken()  );
            Assert.IsNull(firebase.firebase_Site()       );
            TM_UserData.Current = _tmUserdata;                          // restore or the other NCrunch tests will be affected
        }
        [Test] public void offline()            
        {
            Assert.IsFalse(firebase.offline());

            firebase.Offline = true;
            Assert.IsTrue(firebase.offline());

            firebase.Offline = false;
            Assert.IsFalse(firebase.offline());

            firebase = null;
            Assert.IsTrue(firebase.offline());
        }
        [Test] public void submitQueue()        
        {
            firebase = null;
            Assert.IsNull(firebase.submitQueue());
        }
       
        [Test] public void submitThread_Alive() 
        {
            Assert.IsFalse(firebase.submitThread_Alive());
            firebase.submitThread_Start();
            Assert.IsTrue(firebase.submitThread_Alive());
        }
        
        
    }
}
