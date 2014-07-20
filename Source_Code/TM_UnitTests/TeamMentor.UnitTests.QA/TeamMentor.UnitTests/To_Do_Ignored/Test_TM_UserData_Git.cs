using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.QA.To_Do_Ignored
{
    [TestFixture]
    public class Test_TM_UserData_Git
    {
        [Test] public void setup_UserData_Git_Support()
        {            
            var tmXmlDatabase = new TM_Xml_Database();

            Assert.Ignore("needs rewrite");
                /*
            Assert.AreEqual(tmXmlDatabase.Events.After_UserData_Ctor.size(), 1);
            tmXmlDatabase.setup_UserData_Git_Support();
            Assert.AreEqual(tmXmlDatabase.Events.After_UserData_Ctor.size(), 2);
            
            Assert.IsNull  (TM_UserData_Git.Current);
            Assert.IsNull  (tmXmlDatabase.UserData);
            
            tmXmlDatabase.userData();                       // will call the TM_UserData ctor if tmXmlDatabase.UserData is null

            Assert.NotNull  (tmXmlDatabase.UserData);        
            Assert.NotNull  (TM_UserData_Git.Current);            
            Assert.IsNotNull(TM_UserData_Git.Current.UserData);
            Assert.IsNull   (TM_UserData_Git.Current.UserData.path_UserData());                        
            Assert.IsNull   (tmXmlDatabase.path_XmlDatabase());
            Assert.AreEqual (TM_UserData_Git.Current.UserData, tmXmlDatabase.UserData);
                 * */
        }
    }
}
