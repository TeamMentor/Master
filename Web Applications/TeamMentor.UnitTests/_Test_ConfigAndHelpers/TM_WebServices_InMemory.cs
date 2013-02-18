using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.FluentSharp;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    public class TM_WebServices_InMemory : TM_XmlDatabase_InMemory
    {
        public TM_WebServices  tmWebServices;        

        public TM_WebServices_InMemory()
        {
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();
            //tmXmlDatabase    = new TM_Xml_Database();	             
            tmWebServices = new TM_WebServices();
        }
    }
}
