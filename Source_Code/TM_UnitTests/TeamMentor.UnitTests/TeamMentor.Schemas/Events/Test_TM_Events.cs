using System;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Schemas.Events
{
    [TestFixture]
    public class Test_TM_Events
    {
        [Test]
        public void TM_Event_Ctor()
        {
            var target = 100.random();
            var tmEvent = new TM_Event<int>(target);
            Assert.AreEqual(1     , tmEvent.size());
            Assert.AreEqual(target, tmEvent.Target);
            Assert.AreEqual(0     , tmEvent.Total_Invocations);
            Assert.AreEqual(0     , tmEvent.Total_Invocations);
        }
        [Test]
        public void add_Action()
        {
            var target            = 100.random();
            var result            = 0;
            var tmEvent           = new TM_Event<int>(target);

            tmEvent.add_Action((value)=>result = value)
                   .raise();

            Assert.AreEqual  (result, target              );
            Assert.AreEqual  (tmEvent.size()            ,2);
            Assert.IsNull    (tmEvent.Last_Exception      );
            Assert.AreEqual  (tmEvent.Total_Invocations, 2);
            Assert.AreEqual  (tmEvent.Total_Exceptions , 0);

            //test nulls
            tmEvent.add_Action(null);
            
            tmEvent.Target    = 100.random();            

            tmEvent.raise();

            Assert.AreNotEqual(result, target              );
            Assert.AreEqual   (tmEvent.size()            ,2);
            Assert.IsNull     (tmEvent.Last_Exception      );
            Assert.AreEqual   (tmEvent.Total_Invocations, 4);
            Assert.AreEqual   (tmEvent.Total_Exceptions , 0);

            Assert.IsNull((null as TM_Event<int>).add_Action((value)=>result = value));
            Assert.IsNull((null as TM_Event<int>).add_Action(null));
        }
        [Test]
        public void raise()
        {
            var target            = 100.random();
            var result            = 0;
            var exceptionMessage   = 100.randomLetters();
            Action<int> action    = (value) => { result = value; };            
            Action<int> actionEx  = (value) => { throw new Exception(exceptionMessage); };            

            var tmEvent         = new TM_Event<int>(target);
            
            //Start with action that doesn't throw an exception

            tmEvent.add(action);

            Assert.IsNotEmpty(tmEvent);
            Assert.AreEqual  (tmEvent.size() ,2);
            Assert.AreEqual  (tmEvent.second(), action);            
            Assert.IsNull    (tmEvent.Last_Exception);
            Assert.AreEqual  (0, result);
            Assert.AreEqual  (0, tmEvent.Total_Invocations);
            Assert.AreEqual  (0, tmEvent.Total_Exceptions);
            
            tmEvent.raise();

            Assert.AreEqual  (result, target);
            Assert.IsNull    (tmEvent.Last_Exception);
            Assert.AreEqual  (tmEvent.Total_Invocations, 2);
            Assert.AreEqual  (tmEvent.Total_Exceptions , 0);

            //Test action that throws an exception

            tmEvent.add(actionEx);
            Assert.AreEqual  (tmEvent.size(), 3);
            Assert.AreEqual  (tmEvent.second() , action);
            Assert.AreEqual  (tmEvent.third()  , actionEx);

            tmEvent.raise();
                        
            Assert.IsNotNull (tmEvent.Last_Exception                           );
            Assert.AreEqual  (tmEvent.Last_Exception.Message , exceptionMessage);           
            Assert.AreEqual  (tmEvent.Total_Invocations      , 5               );
            Assert.AreEqual  (tmEvent.Total_Exceptions       , 1               );

            //Test Nulls

            tmEvent.add(null as Action<int>);
            Assert.AreEqual  (tmEvent.size(), 4);           
            tmEvent.raise();
            Assert.AreEqual  (tmEvent.Total_Invocations      , 8               );
            Assert.AreEqual  (tmEvent.Total_Exceptions       , 2               );

            tmEvent = null;
            Assert.IsNull(tmEvent.raise());
        }
    }
}
