using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.Database
{
    public static class FileStorage_Utils
    {
        public static TM_Xml_Database                    useFileStorage(this TM_Xml_Database tmXmlDatabase, bool value)
        {
            if (tmXmlDatabase.tmServer().notNull())             
                tmXmlDatabase.Server = TM_Server.Load(value);           // reset these values (since it wil need to be set or reset depending on the value)
            return tmXmlDatabase;
        }
        public static TM_Xml_Database                    useFileStorage(this TM_Xml_Database tmXmlDatabase)
        {
            return tmXmlDatabase.useFileStorage(true);
        }

        public static bool                               usingFileStorage(this TM_Xml_Database tmXmlDatabase)
        {
            if (tmXmlDatabase.tmServer().notNull())
                //return tmXmlDatabase.tmServer().UseFileStorage;
                return TM_Server.UseFileStorage;
            return false;
        }
    }
}
