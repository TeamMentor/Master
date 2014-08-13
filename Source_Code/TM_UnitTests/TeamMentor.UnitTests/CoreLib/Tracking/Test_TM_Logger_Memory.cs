using System;
using FluentSharp.CoreLib;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib.Tracking
{
    [TestFixture]
    public class Test_Logger_LogItem
    {
        public Logger_LogItem tmLogger;

        [SetUp] public void setup()
        {
            tmLogger = new Logger_LogItem();
        }

        [Test] public void Logger_Memory_Ctor()
        {
            Assert.IsNotNull(tmLogger);
            Assert.IsNotNull(tmLogger.LogData);
            Assert.IsNotNull(tmLogger.LogItems);
            Assert.IsNull   (tmLogger.LogRedirectionTarget);
            Assert.IsFalse  (tmLogger.alsoShowInConsole);
            Assert.IsEmpty  (tmLogger.LogItems);
        }
        [Test] public void logItem()
        {
            Assert.IsEmpty  (tmLogger.LogItems);

            var type         = 10.randomLetters();
            var text         = 10.randomLetters();
            var newItem_Type = 10.randomLetters();
            var newItem_Text = 10.randomLetters();
            var newItem      = new Log_Item(newItem_Type, newItem_Text);

            var logItem1 = tmLogger.logItem(type, text);
            var logItem2 = tmLogger.logItem(newItem);
            var logItem3 = tmLogger.logItem(logItem2);

            Assert.NotNull (logItem1);
            Assert.NotNull (logItem2);
            Assert.NotNull (logItem3);

            Assert.AreEqual(logItem1.Type, type);
            Assert.AreEqual(logItem1.Text, text);
            

            Assert.AreEqual(logItem2.Type, newItem_Type);
            Assert.AreEqual(logItem2.Text, newItem_Text);

            Assert.LessOrEqual    (logItem1.When , DateTime.Now.jsDate());
            Assert.LessOrEqual    (logItem1.When , DateTime.Now.jsDate());
            Assert.GreaterOrEqual (logItem1.When, logItem2.When);

            Assert.AreEqual(logItem2     , logItem3);
            Assert.AreEqual(logItem2.When, logItem3.When);
            

            //check tmLogger.LogItems storage
            Assert.AreEqual(tmLogger.LogItems.size(), 3);
            Assert.AreEqual(logItem1, tmLogger.LogItems.first());
            Assert.AreEqual(logItem2, tmLogger.LogItems.second());
            Assert.AreEqual(logItem3, tmLogger.LogItems.third());

        }
        [Test] public void writeMemory()
        {
            var message1 = 10.randomLetters();
            var message2 = 10.randomLetters();

            Assert.IsEmpty   (tmLogger.LogData.str());            
            Assert.AreEqual  (message1, tmLogger.writeMemory(message1));
            Assert.AreEqual  (message2, tmLogger.writeMemory(message2));
            Assert.AreEqual  (null    , tmLogger.writeMemory(null));
            Assert.AreEqual  (""      , tmLogger.writeMemory(""));
            Assert.IsNotEmpty(tmLogger.LogData.str());
            Assert.AreEqual  (2       , tmLogger.LogData.str().lines().size());
            Assert.AreEqual  (message1, tmLogger.LogData.str().lines().first());
            Assert.AreEqual  (message2, tmLogger.LogData.str().lines().second());
        }

        [Test] public void write()
        {
            var message1 = 10.randomLetters();
            var message2 = 10.randomLetters();
            var messageFormat1 = "a{0}b";
            var messageFormat2 = "{0}{1}";
            var messageFormat3 = "{1}{0}";
            
            Assert.IsEmpty   (tmLogger.LogData.str());
            
            tmLogger.write(message1);
            tmLogger.write(messageFormat1);
            tmLogger.write(messageFormat1, message1);
            tmLogger.write(messageFormat2, message1, message2);
            tmLogger.write(messageFormat3, message1, message2);
            
            var lines = tmLogger.lines();

            Assert.IsNotEmpty(tmLogger.LogData.str());
            Assert.AreEqual  (lines.first()  , message1);
            Assert.AreEqual  (lines.second() , messageFormat1);
            Assert.AreEqual  (lines.third()  , messageFormat1.format(message1));
            Assert.AreEqual  (lines.fourth() , message1 + message2);
            Assert.AreEqual  (lines.fifth()  , message2 + message1);            

            //test nulls
            Assert.DoesNotThrow(()=>tmLogger.write(null));
            Assert.DoesNotThrow(()=>tmLogger.write(null,null));
        }

        [Test] public void write_Debug_Info_Error()
        {            
            var message1  = 10.randomLetters();            
            var message2  = 10.randomLetters();            
            var msgFormat = "{0}{1}";

            Action<string> checkLog = 
                    (type)=>
                        {
                            var lines = tmLogger.lines();

                            Assert.AreEqual  (lines.size()  , 2);
                            Assert.IsTrue    (lines.first().contains(message1));
                            Assert.IsFalse   (lines.first().contains(message2));
                            Assert.AreEqual  (lines.first(), "{0}: {1}".format(type,message1));

                            Assert.IsTrue    (lines.second().contains(message1));
                            Assert.IsTrue    (lines.second().contains(message2));   
                            Assert.AreEqual  (lines.second(), "{0}: {1}{2}".format(type,message1, message2));

                            tmLogger.LogData.Clear();
                            Assert.AreEqual  (tmLogger.lines().size()  , 0);
                        };
            tmLogger.debug(message1 , message2);
            tmLogger.debug(msgFormat, message1, message2);            
            checkLog("DEBUG");

            
            tmLogger.info(message1 , message2);
            tmLogger.info(msgFormat, message1, message2);            
            checkLog("INFO");

            tmLogger.error(message1 , message2);
            tmLogger.error(msgFormat, message1, message2);            
            checkLog("ERROR");
        }
        [Test] public void write_Exception()
        {
            var type        = "EXCEPTION";
            var message1    = "m_" + 10.randomLetters(); 
            var exMessage1  = "em_"+10.randomLetters(); 
            var exception = new Exception(exMessage1);

            Assert.AreEqual  (tmLogger.lines().size()   , 0);

            tmLogger.ex(exception);
            tmLogger.ex(exception,message1);
            tmLogger.ex(exception,true);
            tmLogger.ex(exception,message1,true);

            var lines = tmLogger.lines();

            Assert.AreEqual  (lines.size()   , 4);
            Assert.AreEqual  (lines.first()  , "{0}:  {1}"   .format(type,exMessage1));
            Assert.AreEqual  (lines.second() , "{0}: {1} {2}".format(type,message1, exMessage1));
            Assert.AreEqual  (lines.third()  , "{0}:  {1}"   .format(type,exMessage1));
            Assert.AreEqual  (lines.fourth() ,   "{0}: {1} {2}".format(type,message1, exMessage1));
            Assert.IsNull    (exception.StackTrace);
            
            var stackTraceString       = "_stackTraceString".add_RandomLetters(5);
            var remoteStackTraceString = "_remoteStackTraceString".add_RandomLetters(5);
            var expectedStackTrace     = "{0}{1}".format(remoteStackTraceString,stackTraceString);
            exception.field  ("_stackTraceString", stackTraceString);
            exception.field  ("_remoteStackTraceString", remoteStackTraceString);
            Assert.IsNotNull (exception.StackTrace);

            Assert.AreEqual (exception.StackTrace, expectedStackTrace);
            tmLogger.ex(exception,message1,true);
            Assert.AreEqual  (tmLogger.lines().size() , 6);
            Assert.AreEqual  (tmLogger.lines().fifth(), "{0}: {1} {2}".format(type,message1, exMessage1));
            Assert.AreEqual  (tmLogger.lines()[5].trim()     ,  expectedStackTrace);
        }
        [Test] public void logToChache()
        {
            var message1  = 10.randomLetters();            
            var message2  = 10.randomLetters();            
            Assert.IsEmpty   (tmLogger.LogData.str());   

            tmLogger.logToChache(message1);
            tmLogger.logToChache(message2);

            var lines = tmLogger.lines();

            Assert.AreEqual  (lines.size()   , 2);
            Assert.AreEqual  (lines.first()  , message1);
            Assert.AreEqual  (lines.second() , message2);
        }
        [Test] public void lines()
        {
            Assert.IsEmpty   (tmLogger.LogData.str());   
            tmLogger.LogData.appendLine("123")
                            .appendLine("abc")
                            .appendLines("456", "def");
            var lines = tmLogger.lines();

            Assert.AreEqual  (lines.size()                             , 4 );
            Assert.AreEqual  (tmLogger.LogData.str().lines().size()    , 4 );            
            Assert.AreEqual  (tmLogger.LogData.str().lines().asString(), lines.asString());            
        }
    }
}
