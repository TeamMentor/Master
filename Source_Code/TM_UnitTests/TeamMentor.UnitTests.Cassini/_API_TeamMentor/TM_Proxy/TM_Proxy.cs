using FluentSharp.CassiniDev;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using TeamMentor.CoreLib;
using TeamMentor.FileStorage;

namespace TeamMentor.UnitTests.Cassini
{
    public class TM_Proxy
    {
        //Helper objects
        public API_Cassini apiCassini;
        public O2Proxy     o2Proxy;

        //TeamMentor objects
        public TMConfig         TmConfig      { get; set; }
        public TM_FileStorage   TmFileStorage { get; set; }
        public TM_Server        TmServer      { get; set; }
        public TM_Status        TmStatus      { get; set; }
        public TM_UserData      TmUserData    { get; set; }
        public TM_Xml_Database  TmXmlDatabase { get; set; }

        public TM_Proxy(API_Cassini apiCassini)
        {
            this.apiCassini = apiCassini;
            o2Proxy         = apiCassini.appDomain().o2Proxy(); 
        }
    }
}
