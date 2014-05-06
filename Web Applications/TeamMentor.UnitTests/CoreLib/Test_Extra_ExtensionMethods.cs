using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentSharp.CoreLib;
using NUnit.Framework;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_Extra_ExtensionMethods
    {
        [Test] public void jsDate()
        {
            var now     = DateTime.Now;
            var jsDate  = now     .jsDate();
            var date    = jsDate  .fromJsDate();
            var jsDate2 = date    .jsDate();            // round trip test
            var date2   = jsDate2 .fromJsDate();

            Assert.NotNull (jsDate);            
            Assert.AreEqual(jsDate   , jsDate2);
            Assert.AreEqual(date     , date2);
            Assert.AreEqual(now.ToUniversalTime().str(), date.str());            
            Assert.AreEqual(now.ToUniversalTime().str(), date2.str());
            Assert.AreEqual(now.ToUniversalTime().str(), date .ToUniversalTime().str());
            Assert.AreEqual(now.ToUniversalTime().str(), date2.ToUniversalTime().str());
            
            //check null and default value handling
            Assert.AreEqual(default(DateTime),(null as string)  .fromJsDate());
            Assert.AreEqual(""               , default(DateTime).toJsDate());
            Assert.AreEqual(String.Empty     , default(DateTime).toJsDate());

        }
        [Test] public void isDouble()
        {
            Assert.IsTrue ("123".isDouble());
            Assert.IsTrue ("123123123123213".isDouble());
            Assert.IsTrue ("123123123123213123123".isDouble());
            Assert.IsTrue ("123123123123213123123123123213".isDouble());
            Assert.IsTrue ("123123123123213123123123123213123123123213123123123123211341412341234213421342134".isDouble());
            Assert.IsFalse("a".isDouble());
            Assert.IsFalse("a123".isDouble());
            Assert.IsFalse("a123".isDouble());
            Assert.IsFalse((null as string).isDouble());
        }


    }
}
