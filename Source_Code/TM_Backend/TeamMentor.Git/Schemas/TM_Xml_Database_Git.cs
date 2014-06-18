using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.Git.APIs;
using TeamMentor.FileStorage;

namespace TeamMentor.CoreLib
{
    public class TM_Xml_Database_Git
    {
        public static TM_Xml_Database_Git Current_Git   { get; set;}        
        public List<API_NGit> NGits { get; set; }         // Git object, one per library that has git support

        public TM_Xml_Database_Git()
        {
            Current_Git = this;            
            NGits = new List<API_NGit>();
        }
    }
}
