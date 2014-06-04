using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.FileStorage.FileStorage
{
    public static class TM_Xml_Database_FileStorage_Library
    {
        public static string xmlDB_Path_Library_XmlFile(this TM_Xml_Database tmDatabase, guidanceExplorer guidanceExplorer)
        {
            if (tmDatabase.notNull())
            {
                return tmDatabase.GuidanceExplorers_Paths.value(guidanceExplorer);                
            }
            return null;
        }
    }
}
