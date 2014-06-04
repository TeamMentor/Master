using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.Git.APIs;

namespace TeamMentor.CoreLib
{
    public class TM_Xml_Database_Git : TM_Xml_Database
    {
        public List<API_NGit> NGits { get; set; }         // Git object, one per library that has git support

        public TM_Xml_Database_Git()
        {
            NGits = new List<API_NGit>();
        }
    }
}
