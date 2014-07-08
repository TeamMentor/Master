using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentSharp.CoreLib;

namespace TeamMentor.Mcv
{
    public class ScriptCombiner_Controller : ApiController
    {        
        [Route("scriptCombiner")]
        public string Get()
        {
            return "this is the script combi...ner";
        }
    }
}
