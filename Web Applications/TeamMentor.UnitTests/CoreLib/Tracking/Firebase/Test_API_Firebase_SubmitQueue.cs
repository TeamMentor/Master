using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
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
            var area = 10.randomLetters();
            var data = new List<object> { 10.randomLetters(),  10.randomLetters(), 10000.random()};
            var type = API_Firebase.Submit_Type.SET;

            var submitData1 = new API_Firebase.SubmitData(); 
            var submitData2 = new API_Firebase.SubmitData(area, data, type); 

            Assert.AreEqual(submitData1.Area     , null);
            Assert.AreEqual(submitData1.Data     , null);
            Assert.AreEqual(submitData1.Json_Data, null);
            Assert.AreEqual(submitData1.Type     , API_Firebase.Submit_Type.ADD);

            Assert.AreEqual(submitData2.Area     , area);
            Assert.AreEqual(submitData2.Data     , data);
            Assert.AreEqual(submitData2.Json_Data, data.json());
            Assert.AreEqual(submitData2.Type     , type);
        }
        
        // API_Firebase methods
        [Test] public void firebase_Site()      
        {
            Assert.IsNull(userData.SecretData.Firebase_AuthToken);
            Assert.IsNull(userData.SecretData.Firebase_Site     );            
            Assert.IsNull(firebase.firebase_AuthToken()         );            
            Assert.IsNull(firebase.firebase_Site()              );

            var site      = 10.randomLetters();
            var authToken = 10.randomLetters();
            userData.SecretData.Firebase_Site      = site;
            userData.SecretData.Firebase_AuthToken = authToken;

            Assert.AreEqual(userData.SecretData.Firebase_AuthToken, authToken);
            Assert.AreEqual(userData.SecretData.Firebase_Site     , site);
            Assert.AreEqual(firebase.firebase_AuthToken()         , authToken);            
            Assert.AreEqual(firebase.firebase_Site()              , site);

            //test nulls
            var _tmUserdata     = TM_UserData.Current;
            TM_UserData.Current = null;
            Assert.IsNull(firebase.firebase_AuthToken()         );            
            Assert.IsNull(firebase.firebase_Site()              );       
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
        [Test] public void submitThread_Start() 
        {                               
            Assert.IsNull     (firebase.SubmitThread);
            firebase.submitThread_Start();              // will start a new thread
            var submitThread = firebase.SubmitThread;
            Assert.IsNotNull  (firebase.SubmitThread);            
            Assert.IsNotNull  (submitThread);
            if (firebase.submitThread_Alive())
            {
                firebase.submitThread_Start();              // should not start a new thread
                Assert.AreEqual   (submitThread, firebase.SubmitThread);
                firebase.SubmitThread.Join();
            }
            Assert.IsNull     (firebase.SubmitThread);            

            firebase.submitThread_Start();             // will start a new thread
            Assert.IsNotNull  (firebase.SubmitThread);            
            Assert.AreNotEqual(submitThread, firebase.SubmitThread);
            25.sleep();            
            Assert.IsNull     (firebase.SubmitThread);           
        }
        [Test] public void submitThread_Alive() 
        {
            Assert.IsFalse(firebase.submitThread_Alive());
            firebase.submitThread_Start();
            Assert.IsTrue(firebase.submitThread_Alive());
        }
        
        // API_Firebase workflows
        [Test] public void Test_SubmitThread_Behaviour()        
        {
            Assert.AreEqual(firebase.submitQueue_Size(), 0);
            Assert.IsFalse (firebase.submitThread_Alive());
            var submitData = new API_Firebase.SubmitData("area", "data", API_Firebase.Submit_Type.GET);
            
            //send one submitData
            firebase.add(submitData);
            if (firebase.submitThread_Alive())
            {
                Assert.IsTrue(firebase.submitQueue_Size() > 0);
                firebase.SubmitThread.Join();                       //let the queue handle it
            }

            Assert.AreEqual(firebase.submitQueue_Size(), 0);
            Assert.IsFalse (firebase.submitThread_Alive());

            //send threee submitData
            firebase.add(submitData);
            firebase.add(submitData);
            firebase.add(submitData);
            if (firebase.submitQueue_Size()!=0)                 // in case they were already handled (can happen on UnitTests)
            {
                Assert.AreEqual(firebase.submitQueue_Size(), 3);
                Assert.IsTrue  (firebase.submitThread_Alive());
            }
            //let the queue handle it
            firebase.SubmitThread.Join();
            Assert.AreEqual(firebase.submitQueue_Size(), 0);
            Assert.IsFalse (firebase.submitThread_Alive());
        }
        [Test] public void Test_SubmitThread_Offline_Behaviour()
        {
            Assert.IsFalse (firebase.offline());
            firebase.Offline = true;
            Assert.IsTrue  (firebase.offline());            
            Assert.AreEqual(firebase.offlineQueue(), firebase.OfflineQueue);            

            var submitData1 = new API_Firebase.SubmitData();
            var submitData2 = new API_Firebase.SubmitData();
            var submitData3 = new API_Firebase.SubmitData();            

            Assert.AreEqual(firebase.offlineQueue().size(),0);
            
            firebase.submit(submitData1);
            firebase.submit(submitData2);
            firebase.SubmitThread.Join();

            Assert.AreEqual(firebase.offlineQueue().size(),2);

            firebase.submit(submitData3);            
            firebase.submit(submitData2);
            firebase.submit(submitData1);
            firebase.SubmitThread.Join();

            Assert.AreEqual(firebase.offlineQueue().size(), 5);
            Assert.AreEqual(firebase.offlineQueue().next(), submitData1);
            Assert.AreEqual(firebase.offlineQueue().next(), submitData2);
            Assert.AreEqual(firebase.offlineQueue().next(), submitData3);
            Assert.AreEqual(firebase.offlineQueue().next(), submitData2);
            Assert.AreEqual(firebase.offlineQueue().next(), submitData1);
            Assert.AreEqual(firebase.offlineQueue().size(), 0);
        }
        [Test] public void Test_BlockingCollection_Behaviour()  
        {
            var submitQueue = new BlockingCollection<API_Firebase.SubmitData>();
            Assert.AreEqual(0, submitQueue.size());

            var submitData1 = new API_Firebase.SubmitData();
            var submitData2 = new API_Firebase.SubmitData();
            var submitData3 = new API_Firebase.SubmitData();
            var submitData4 = new API_Firebase.SubmitData();
            submitQueue.add(submitData1);
            
            Assert.AreEqual(1, submitQueue.size());

            O2Thread.mtaThread(()=>
                {
                    submitQueue.add(submitData2);
                    10.sleep();
                    submitQueue.add(submitData3);
                    10.sleep();
                    submitQueue.add(submitData4);
                });

            //note how the next() (i.e. the Take() method) will wait for the data to be available
            5.sleep();
            Assert.AreEqual(submitQueue.size(), 2);
            Assert.AreEqual(submitQueue.next(), submitData1);
            Assert.AreEqual(submitQueue.next(), submitData2);
            Assert.AreEqual(submitQueue.next(1), null);          // with 1 ms wait  
            Assert.AreEqual(submitQueue.size(), 0);
            Assert.AreEqual(submitQueue.next(), submitData3);
            Assert.AreEqual(submitQueue.size(),0);
            Assert.AreEqual(submitQueue.next(1), null);         // with 1 ms wait
            Assert.AreEqual(submitQueue.next(), submitData4);
            
            //test nulls
            var currentCount = submitQueue.size();
            submitQueue.add(null);
            Assert.AreEqual(currentCount, submitQueue.size());
            
            Assert.IsNull((null as BlockingCollection<API_Firebase.SubmitData>).add(null));
            Assert.IsNull((null as BlockingCollection<API_Firebase.SubmitData>).add(new API_Firebase.SubmitData()));
            Assert.IsNull((null as BlockingCollection<API_Firebase.SubmitData>).next());

        }
    }
}
