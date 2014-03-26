using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    public class TM_WebServices_InMemory : TM_XmlDatabase_InMemory
    {
        public TM_WebServices  tmWebServices;        

        public TM_WebServices_InMemory()
        {
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();            
            tmWebServices = new TM_WebServices();
        }
    }
}
