using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests
{
    public class TM_Config_InMemory
    {
        public TMConfig         tmConfig;
    
        public TM_Config_InMemory()
        {
            tmConfig = TMConfig.Current = new TMConfig();            
        }
    }
}
