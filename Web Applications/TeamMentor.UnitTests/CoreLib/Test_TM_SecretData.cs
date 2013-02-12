using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_SecretData : TM_XmlDatabase_InMemory
    {
        [Test]
        public void SecretDataUsage()
        {
            var tmSecretData = tmXmlDatabase.UserData.SecretData;
            Assert.IsNotNull(tmSecretData);
            Assert.IsNotNull(tmSecretData.Rijndael_IV);
            Assert.IsNotNull(tmSecretData.Rijndael_Key);
            Assert.IsNotNull(tmSecretData.SMTP_Server);
            Assert.IsNotNull(tmSecretData.SMTP_UserName);
            Assert.IsNull   (tmSecretData.SMTP_Password);

            "TMSecretData xml: \n {0}".info(tmSecretData.toXml());
        }
    }
}
