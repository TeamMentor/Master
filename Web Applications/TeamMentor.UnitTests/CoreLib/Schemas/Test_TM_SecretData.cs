﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.CoreLib
{
    [TestFixture]
    public class Test_TM_SecretData
    {
        [Test]
        public void SecretData_Ctor()
        {
            var tmSecretData = new TM_SecretData();                        

            Assert.IsNotNull(tmSecretData);
            Assert.IsNotNull(tmSecretData.Rijndael_IV);
            Assert.IsNotNull(tmSecretData.Rijndael_Key);

            Assert.IsNotNull(tmSecretData.SmtpConfig.Server);
            Assert.IsNotNull(tmSecretData.SmtpConfig.UserName);
            Assert.AreEqual(tmSecretData.SmtpConfig.Password, "");
            Assert.IsNotNull(tmSecretData.SmtpConfig.Default_From);
            Assert.IsNotNull(tmSecretData.SmtpConfig.Default_To);
            Assert.IsNotNull(tmSecretData.SmtpConfig.Server);
        }
    }
}
