using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;
using TeamMentor.UnitTests;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;


namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    public class Test_TM_Xml_Database_Config
    {
        public Test_TM_Xml_Database_Config()
        {
            UserGroup.Admin.assert();
        }        

        [Test]
        public void set_Path_XmlDatabase()
        {
            var tmServer = new TM_Server()
                                {
                                    Users_Create_Default_Admin = false
                                };
            "\n\n******************* using usingFileStorage = false *******************\n".info();
            var tmXmlDatabase1 = new TM_Xml_Database(false, tmServer);             

            Assert.AreEqual(tmXmlDatabase1, tmXmlDatabase1.set_Path_XmlDatabase());
            Assert.AreEqual(tmXmlDatabase1, TM_Xml_Database.Current);
            Assert.IsNull  (tmXmlDatabase1.Path_XmlDatabase);
            Assert.IsFalse (tmXmlDatabase1.UsingFileStorage);

            "\n\n******************* using usingFileStorage = true *******************\n".info();
            var tmXmlDatabase2 = new TM_Xml_Database(true);         
            Assert.AreNotEqual(tmXmlDatabase1, tmXmlDatabase2, "A new tmXmlDatabase1 should had been created");               
            Assert.AreEqual   (tmXmlDatabase2, tmXmlDatabase2.set_Path_XmlDatabase());
            Assert.AreEqual   (tmXmlDatabase2, TM_Xml_Database.Current);
            Assert.IsTrue     (tmXmlDatabase2.UsingFileStorage);
            Assert.IsNotNull  (tmXmlDatabase2.Path_XmlDatabase);

            //tmXmlDatabase2.Path_XmlDatabase.parentFolder().startProcess();
            
            //tmXmlDatabase2.Path_XmlDatabase.info();

            tmXmlDatabase2.delete_Database();                                    
        }
    }
}
