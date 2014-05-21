using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMentor.CoreLib
{
    public class TM_Status
    {        
        public static TM_Status   Current                             { get; set; }
        public Database_Status    TM_Database_Status                  { get; set; }
        public bool               TM_Database_In_Setup_Workflow       { get; set; }
        public bool               TM_Database_Location_Using_AppData  { get; set; }
        static TM_Status()
        {
            Current = new TM_Status();
        }
        public TM_Status()
        {
            TM_Database_Status                 = Database_Status.Not_Initialized;
            TM_Database_In_Setup_Workflow      = false;
            TM_Database_Location_Using_AppData = false;
        }        

        public enum Database_Status
        { 
            Not_Initialized, 
            Ready,
            Loading_XmlDatabase, 
            Loading_UserData
        }
    }
}
