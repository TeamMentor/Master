using System.Collections.Concurrent;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.NUnit;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.QA
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
        // API_Firebase workflows     

        //[Ignore("Rewrite using .Net 4.5 Async")]
        [Test] public void Test_SubmitThread_Behaviour()        
        {
            Assert.AreEqual(firebase.submitQueue_Size(), 0);
            Assert.IsFalse (firebase.submitThread_Alive());
            var submitData = new API_Firebase.SubmitData("data", API_Firebase.Submit_Type.GET);
            
            //send one submitData
            firebase.add(submitData);
            if (firebase.submitThread_Alive())
            {
                if(firebase.submitQueue_Size() > 0)
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
            firebase.SubmitThread.join();
            Assert.AreEqual(firebase.submitQueue_Size(), 0);
            Assert.IsFalse (firebase.submitThread_Alive());
        }

        //[Ignore("Rewrite using .Net 4.5 Async")]
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

            if(firebase.offlineQueue().size() >0)
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

        //[Ignore("No stable on TeamCity (rewrite using Task)")]
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
        //[Ignore("Rewrite with aync")]
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
                if (firebase.SubmitThread.notNull())
                    firebase.SubmitThread.Join();
            }
            Assert.IsNull     (firebase.SubmitThread);            

            firebase.submitThread_Start();             // will start a new thread
            Assert.IsNotNull  (firebase.SubmitThread);            
            Assert.AreNotEqual(submitThread, firebase.SubmitThread);
            25.sleep();            
            Assert.IsNull     (firebase.SubmitThread);           
        }
    }
}
