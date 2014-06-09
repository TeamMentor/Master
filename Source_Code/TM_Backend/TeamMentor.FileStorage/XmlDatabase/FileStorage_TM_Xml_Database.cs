using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMentor.CoreLib;
using urn.microsoft.guidanceexplorer;

namespace TeamMentor.FileStorage.XmlDatabase
{
    public static class FileStorage_TM_Xml_Database
    {
        [Admin] public static TM_FileStorage            set_TM_XmlDatabase_defaultValues(this TM_FileStorage tmFileStorage)
        {
            //var tmXmlDatabase = tmFileStorage.TMXmlDatabase;
            tmFileStorage.GuidanceItems_FileMappings  = new Dictionary<Guid, string>();
            tmFileStorage.GuidanceExplorers_Paths     = new Dictionary<guidanceExplorer, string>();            
            tmFileStorage.Path_XmlLibraries           = null;
            return tmFileStorage;
        }
        
    }
}
