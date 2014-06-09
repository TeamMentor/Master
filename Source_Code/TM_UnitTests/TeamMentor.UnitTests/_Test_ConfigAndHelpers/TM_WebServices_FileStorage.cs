using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    public class TM_WebServices_FileStorage : TM_XmlDatabase_FileStorage
    {
        public TM_WebServices  tmWebServices;        

        public TM_WebServices_FileStorage()
        {
            HttpContextFactory.Context = new API_Moq_HttpContext().httpContext();            
            tmWebServices = new TM_WebServices();
        }
    }
}